using System;
namespace Oreganoli.SteganoToolkit;
public class UnsupportedImageFormatException : ApplicationException
{
    private string Format;

    public UnsupportedImageFormatException(string format)
    {
        this.Format = format;
    }
    public override string Message => $"The {Format} format is not supported for lossless-format steganography.";
}
public class ImageTooSmallException : ApplicationException
{
    private int MessageSize;
    private int ImageWidth;

    public ImageTooSmallException(int messageSize, int imageWidth)
    {
        MessageSize = messageSize;
        ImageWidth = imageWidth;
    }

    public override string Message => $"The image was too small for a message of length {MessageSize}B at {ImageWidth}px width";
}