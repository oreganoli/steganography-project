using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System;
using System.IO;
namespace Oreganoli.SteganoToolkit;
/// <summary>
/// This class implements steganography for any lossless image format.
/// </summary>
public class LosslessStegano
{
    public static void Encode(Stream input, Stream output, out IImageFormat format)
    {
        var img = Image.Load(input, out format);
        if (format.Name == "JPEG" || format.Name == "GIF")
        {
            throw new FormatException($"The {format.Name} format is not supported for lossless-format steganography.");
        }
    }
}