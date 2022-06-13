using Microsoft.AspNetCore.Mvc;
using System;

[ApiController]
[Route("[controller]")]
public class SteganoController : ControllerBase
{
    [HttpPost]
    [Route("/encode")]
    public IActionResult Encode([FromForm] IFormCollection form)
    {
        throw new NotImplementedException();
    }
    [HttpPost]
    [Route("/encode_key")]
    public IActionResult EncodeWithKey([FromForm] IFormCollection form)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("/decode")]
    public IActionResult Decode([FromForm] IFormCollection form)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("/decode_key")]
    public IActionResult DecodeWithKey([FromForm] IFormCollection form)
    {
        throw new NotImplementedException();
    }
}