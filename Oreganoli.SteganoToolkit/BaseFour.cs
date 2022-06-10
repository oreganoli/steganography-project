using System;

namespace Oreganoli.SteganoToolkit;

public class BaseFour
{
    private byte SixtyFours { get; }
    private byte Sixteens { get; }
    private byte Fours { get; }
    private byte Ones { get; }

    /// <summary>
    /// Breaks down a byte into its base-4 representation.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public BaseFour(byte from)
    {
        SixtyFours = (byte)((from - from % 64) / 64);
        Sixteens = (byte)((from % 64 - from % 16) / 16);
        Fours = (byte)((from % 16 - from % 4) / 4);
        Ones = (byte)((from % 4));
    }
    public byte AsByte()
    {
        return (byte)(SixtyFours * 64 + Sixteens * 16 + Fours * 4 + Ones);
    }
    public override string ToString()
    {
        return $"({SixtyFours}, {Sixteens}, {Fours}, {Ones}) (= {AsByte()})";
    }
}