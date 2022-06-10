using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
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
        var img = Image.Load<Rgba32>(input, out format);
        if (format.Name == "JPEG" || format.Name == "GIF")
        {
            throw new UnsupportedImageFormatException(format.Name);
        }
        if (message.Length * 2 > img.Width)
        {
            throw new ImageTooSmallException(message.Length, img.Width);
        }
        img.ProcessPixelRows(accessor =>
        {
            var upper = accessor.GetRowSpan(0);
            var lower = accessor.GetRowSpan(1);
            for (var i = 0; i < message.Length; i++)
            {
                var base4 = new BaseFour(message[i]);
                var upperL = upper[i * 2];
                var upperR = upper[i * 2 + 1];
                #region Decrementation
                if (upperL.R - base4.SixtyFours < 0)
                {
                    upperL.R += base4.SixtyFours;
                }
                else
                {
                    upperL.R -= base4.SixtyFours;
                }
                if (upperL.G - base4.Sixteens < 0)
                {
                    upperL.G += base4.Sixteens;
                }
                else
                {
                    upperL.G -= base4.Sixteens;
                }
                if (upperL.B - base4.Fours < 0)
                {
                    upperL.B += base4.Fours;
                }
                else
                {
                    upperL.B -= base4.Fours;
                }
                if (upperR.R - base4.Ones < 0)
                {
                    upperR.R += base4.Ones;
                }
                else
                {
                    upperR.R -= base4.Ones;
                }
                #endregion
                lower[i * 2] = upperL;
                lower[i * 2 + 1] = upperR;
            }
        });
        img.Save(output, format);
    }
}