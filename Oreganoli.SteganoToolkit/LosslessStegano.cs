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
    public static void Encode(Stream input, Stream output, byte[] message, out IImageFormat format)
    {
        var img = Image.Load(input, out format);
        if (format.Name == "JPEG" || format.Name == "GIF")
        {
            throw new UnsupportedImageFormatException(format.Name);
        }
        if (message.Length * 2 > img.Width)
        {
            throw new ImageTooSmallException(message.Length, img.Width);
        }
    }
}