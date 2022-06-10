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
        var columns = img.Width / 2;
        var rows = img.Height / 2;
        if (message.Length > columns * rows)
        {
            throw new ImageTooSmallException(message.Length, img.Width);
        }
        columns = Math.Min(img.Width / 2, message.Length);
        rows = message.Length / columns;
        img.ProcessPixelRows(accessor =>
        {
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns && row + col < message.Length; col++)
                {
                    var upper = accessor.GetRowSpan(row);
                    var lower = accessor.GetRowSpan(row + 1);
                    var base4 = new BaseFour(message[row + col]);
                    var upperL = upper[(row + col) * 2];
                    var upperR = upper[(row + col) * 2 + 1];

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

                    lower[(row + col) * 2] = upperL;
                    lower[(row + col) * 2 + 1] = upperR;
                }
            }
        });
        img.Save(output, format);
    }
}