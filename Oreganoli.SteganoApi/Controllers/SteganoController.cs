using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System;
using System.Text;
using System.Security.Cryptography;
using Oreganoli.SteganoToolkit;
using SixLabors.ImageSharp;
[ApiController]
[Route("[controller]")]
public class SteganoController : ControllerBase
{
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
            using (var memStream = new MemoryStream(inputBytes))
            using (var crStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(crStream))
            {
                writer.Write(message);
                return memStream.ToArray();
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
    public IActionResult EncodeWithKey([FromForm] IFormCollection form)
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
        message = crypto.CreateEncryptor()
    }

    [HttpPost]
    [Route("decode")]
    public void Decode([FromForm] IFormCollection form)
    {
        var imageFile = form.Files["image"] ?? throw new ArgumentNullException("image", "No image file was provided.");
        var imageStream = imageFile.OpenReadStream();
        var format = Image.DetectFormat(imageStream);
        if (format.Name == "JPEG")
        {
            Response.ContentType = MediaTypeNames.Text.Plain;
            var msg = JpegStegano
            .Decode(imageStream);
            Response.BodyWriter.AsStream().Write(msg);
        }
        else if (format.Name == "GIF")
        {
            throw new UnsupportedImageFormatException("GIF");
        }
        else
        {
            Response.ContentType = MediaTypeNames.Text.Plain;
            var mem = new MemoryStream();
            LosslessStegano
            .Decode(imageStream, Response.BodyWriter.AsStream());
            // Console.WriteLine(mem.Length);
            // mem.CopyTo(Response.BodyWriter.AsStream());
        }
    }

    [HttpPost]
    [Route("decode_key")]
    public IActionResult DecodeWithKey([FromForm] IFormCollection form)
    {
        throw new NotImplementedException();
    }
}