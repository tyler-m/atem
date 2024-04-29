
namespace Atem
{
    internal enum LocationType
    {
        Register, // A
        WordRegister, // BC
        RegisterOffset, // SP+s8
        Direct, // u8
        DirectSigned, // s8
        DirectWord, // u16
        Indirect, // (u16)
        IndirectWordRegister, // (BC)
        IndirectIncrement, // (HL+)
        IndirectDecrement, // (HL-)
        IndirectOffsetRegister, // (FF00+C)
        IndirectOffset, // (FF00+u8)
    }

    internal static class LocationTypeExtensions
    {
        public static LocationType ToLocationType(this string location)
        {
            if (location.StartsWith('('))
            {
                if (location.Length == 5)
                {
                    if (location[3] == '+')
                    {
                        return LocationType.IndirectIncrement;
                    }
                    else if (location[3] == '-')
                    {
                        return LocationType.IndirectDecrement;
                    }
                    else
                    {
                        return LocationType.Indirect;
                    }
                }
                else if (location.Length == 4)
                {
                    return LocationType.IndirectWordRegister;
                }
                else if (location.Length == 8)
                {
                    return LocationType.IndirectOffsetRegister;
                }
                else
                {
                    return LocationType.IndirectOffset;
                }
            }
            else if (location.StartsWith('u'))
            {
                if (location[1] == '8')
                {
                    return LocationType.Direct;
                }
                else
                {
                    return LocationType.DirectWord;
                }
            }
            else if (location.StartsWith("s"))
            {
                return LocationType.DirectSigned;
            }
            else
            {
                if (location.Length == 1)
                {
                    return LocationType.Register;
                }
                else if (location.Length == 2)
                {
                    return LocationType.WordRegister;
                }
                else
                {
                    return LocationType.RegisterOffset;
                }
            }
        }
    }
}
