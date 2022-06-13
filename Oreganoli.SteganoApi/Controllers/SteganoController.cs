using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System;
using System.Text;
using Oreganoli.SteganoToolkit;
using SixLabors.ImageSharp;
[ApiController]
[Route("[controller]")]
public class SteganoController : ControllerBase
{
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
        var format = Image.DetectFormat(imageStream);
        if (format.Name == "JPEG")
        {
            Response.ContentType = MediaTypeNames.Image.Jpeg;
            JpegStegano
            .Encode(imageStream,
            Response.BodyWriter.AsStream(),
            Encoding.UTF8.GetBytes(message));
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
            Encoding.UTF8.GetBytes(message), out var fmt);
        }
    }
    [HttpPost]
    [Route("encode_key")]
    public IActionResult EncodeWithKey([FromForm] IFormCollection form)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("decode")]
    public IActionResult Decode([FromForm] IFormCollection form)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("decode_key")]
    public IActionResult DecodeWithKey([FromForm] IFormCollection form)
    {
        throw new NotImplementedException();
    }
}