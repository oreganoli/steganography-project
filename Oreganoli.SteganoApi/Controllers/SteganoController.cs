using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System;
using System.Text;
using System.Security.Cryptography;
using Oreganoli.SteganoToolkit;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

[ApiController]
[Route("[controller]")]
public class SteganoController : ControllerBase
{
    private string Decrypt(string key, byte[] message)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        if (keyBytes.Length > 32)
        {
            keyBytes = keyBytes.Take(32).ToArray();
        }
        else if (keyBytes.Length < 32)
        {
            keyBytes = keyBytes.Concat(Enumerable.Repeat<byte>(0, 32 - keyBytes.Length)).ToArray();
        }
        byte[] output;
        using (var crypto = Aes.Create())
        {
            crypto.Key = keyBytes;
            using (var decryptor = crypto.CreateDecryptor())
            using (var outStream = new MemoryStream())
            using (var crStream = new CryptoStream(outStream, decryptor, CryptoStreamMode.Write))
            {
                crStream.Write(message, 0, message.Length);
                output = outStream.ToArray();
            }
        }
        return Encoding.UTF8.GetString(output);
    }
    private byte[] Encrypt(string key, string message)
    {
        var inputBytes = Encoding.UTF8.GetBytes(message);
        var keyBytes = Encoding.UTF8.GetBytes(key);
        if (keyBytes.Length > 32)
        {
            keyBytes = keyBytes.Take(32).ToArray();
        }
        else if (keyBytes.Length < 32)
        {
            keyBytes = keyBytes.Concat(Enumerable.Repeat<byte>(0, 32 - keyBytes.Length)).ToArray();
        }

        using (var crypto = Aes.Create())
        {
            crypto.Key = keyBytes;
            using (var encryptor = crypto.CreateEncryptor())
            using (var outStream = new MemoryStream())
            using (var crStream = new CryptoStream(outStream, encryptor, CryptoStreamMode.Write))
            {
                crStream.Write(inputBytes, 0, inputBytes.Length);
                return outStream.ToArray();
            }
        }
    }
    private void EncodeMessage(byte[] message, Stream imageStream)
    {
        var format = Image.DetectFormat(imageStream);
        if (format.Name == "JPEG")
        {
            Response.ContentType = MediaTypeNames.Image.Jpeg;
            JpegStegano
            .Encode(imageStream, Response.BodyWriter.AsStream(),
            message);
        }
        else if (format.Name == "GIF")
        {
            throw new UnsupportedImageFormatException("GIF");
        }
        else
        {
            Response.ContentType = format.DefaultMimeType;
            LosslessStegano
            .Encode(imageStream,
            Response.BodyWriter.AsStream(),
            message, out var _fmt);
        }
    }
    private byte[] DecodeMessage(Stream imageStream, out IImageFormat format)
    {
        format = Image.DetectFormat(imageStream);
        var memStream = new MemoryStream();
        if (format.Name == "JPEG")
        {
            Response.ContentType = MediaTypeNames.Image.Jpeg;
            var msg = JpegStegano.Decode(imageStream);
            memStream.Write(msg);
        }
        else if (format.Name == "GIF")
        {
            throw new UnsupportedImageFormatException("GIF");
        }
        else
        {
            Response.ContentType = MediaTypeNames.Text.Plain;
            LosslessStegano.Decode(imageStream, memStream);
        }
        return memStream.ToArray();
    }
    [HttpPost]
    [Route("encode")]
    public void Encode([FromForm] IFormCollection form)
    {
        var message = form["message"];
        if (message == Microsoft.Extensions.Primitives.StringValues.Empty)
        {
            throw new ArgumentNullException("message", "No message was provided.");
        }
        var imageFile = form.Files["image"] ?? throw new ArgumentNullException("image", "No image file was provided.");
        var imageStream = imageFile.OpenReadStream();
        EncodeMessage(Encoding.UTF8.GetBytes(message), imageStream);
    }
    [HttpPost]
    [Route("encode_key")]
    public void EncodeWithKey([FromForm] IFormCollection form)
    {
        var key = form["key"];
        if (key == Microsoft.Extensions.Primitives.StringValues.Empty)
        {
            throw new ArgumentNullException("key", "No encryption key was provided.");
        }

        var message = form["message"];
        if (message == Microsoft.Extensions.Primitives.StringValues.Empty)
        {
            throw new ArgumentNullException("message", "No message was provided.");
        }
        var messageBytes = Encrypt(key, message);
        var imageFile = form.Files["image"] ?? throw new ArgumentNullException("image", "No image file was provided.");
        var imageStream = imageFile.OpenReadStream();
        EncodeMessage(messageBytes, imageStream);
    }

    [HttpPost]
    [Route("decode")]
    public void Decode([FromForm] IFormCollection form)
    {
        var imageFile = form.Files["image"] ?? throw new ArgumentNullException("image", "No image file was provided.");
        var imageStream = imageFile.OpenReadStream();
        var msg = DecodeMessage(imageStream, out var _format);
        Response.BodyWriter.AsStream().Write(msg, 0, msg.Length);
    }

    [HttpPost]
    [Route("decode_key")]
    public IActionResult DecodeWithKey([FromForm] IFormCollection form)
    {
        var key = form["key"];
        if (key == Microsoft.Extensions.Primitives.StringValues.Empty)
        {
            throw new ArgumentNullException("key", "No encryption key was provided.");
        }
        var imageFile = form.Files["image"] ?? throw new ArgumentNullException("image", "No image file was provided.");
        var imageStream = imageFile.OpenReadStream();
        var msg = DecodeMessage(imageStream, out var _format);
        return Ok(Decrypt(key, msg));
    }
}