using System.Collections.Generic;

namespace BrainForceOne.BrainforceControls;

public static class DigitMapper
{
    public static readonly Dictionary<char, Segments> DigitMap = new()
    {
        ['0'] = Segments.A | Segments.B | Segments.C | Segments.D | Segments.E | Segments.F,
        ['1'] = Segments.B | Segments.C,
        ['2'] = Segments.A | Segments.B | Segments.G | Segments.E | Segments.D,
        ['3'] = Segments.A | Segments.B | Segments.G | Segments.C | Segments.D,
        ['4'] = Segments.F | Segments.G | Segments.B | Segments.C,
        ['5'] = Segments.A | Segments.F | Segments.G | Segments.C | Segments.D,
        ['6'] = Segments.A | Segments.F | Segments.G | Segments.C | Segments.D | Segments.E,
        ['7'] = Segments.A | Segments.B | Segments.C,
        ['8'] = Segments.A | Segments.B | Segments.C | Segments.D | Segments.E | Segments.F | Segments.G,
        ['9'] = Segments.A | Segments.B | Segments.C | Segments.D | Segments.F | Segments.G
    };
}
