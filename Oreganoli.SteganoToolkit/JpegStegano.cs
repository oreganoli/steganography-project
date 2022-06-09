using System;
using System.IO;
namespace Oreganoli.SteganoToolkit;
/// <summary>
/// Class responsible for JPEG steganography.
/// </summary>
public static class JpegStegano
{
    public static void Encode(Stream s, byte[] message)
    {
        s.Seek(0, SeekOrigin.End);
        s.Write(message);
        // Write the length of the message upfront at the last 4 bytes of the file.
        var msgLen = BitConverter.GetBytes(message.Length);
        s.Write(msgLen);
        s.Flush();
    }

    public static byte[] Decode(Stream s)
    {
        // Read the last 4 bytes of the stream to get the length of the message proper.
        byte[] len = new byte[4];
        s.Seek(-4, SeekOrigin.End);
        s.Read(len, 0, 4);
        var length = BitConverter.ToInt32(len, 0);
        // Seek back to the beginning of the message.
        s.Seek(-(4 + length), SeekOrigin.End);
        byte[] message = new byte[length];
        s.Read(message, 0, length);
        return message;
    }
}
