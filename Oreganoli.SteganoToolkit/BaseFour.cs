using System;

namespace Oreganoli.SteganoToolkit;

public class BaseFour
{
    public byte SixtyFours { get; }
    public byte Sixteens { get; }
    public byte Fours { get; }
    public byte Ones { get; }

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

    public BaseFour(byte sixtyFours, byte sixteens, byte fours, byte ones)
    {
        SixtyFours = sixtyFours;
        Sixteens = sixteens;
        Fours = fours;
        Ones = ones;
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