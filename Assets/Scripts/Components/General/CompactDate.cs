using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Unity.Mathematics;

namespace Components.General
{
    [StructLayout(LayoutKind.Explicit)]
    public struct CompactDate
    {
        [JsonProperty] [FieldOffset(0)] private int EncodedInt; // Json only value.

        [JsonIgnore] [FieldOffset(0)] private byte YearOne;
        [JsonIgnore] [FieldOffset(1)] private byte YearTwo;
        [JsonIgnore] [FieldOffset(2)] public byte Month;
        [JsonIgnore] [FieldOffset(3)] public byte Date;

        public int Year
        {
            get => YearOne << (0 + YearTwo) << 8;
            set
            {
                YearOne = (byte) (value >> 0);
                YearTwo = (byte) (value >> 8);
            }
        }

        public CompactDate(int days) : this()
        {
            if (days < 0)
                return; // Negative dates not possible.

            var years = days / 365;
            Month = (byte) math.floor((days - years * 365) / 30.5f);
            Date = (byte) (days - years * 365 - Month * 30.5f);
            Year = years;
        }

        public static explicit operator int(CompactDate date)
        {
            return (int) (date.Year * 365 + date.Month * 30.5f + date.Date);
        }

        public static CompactDate operator +(CompactDate a, CompactDate b)
        {
            return new CompactDate((int) a + (int) b);
        }

        public static CompactDate operator -(CompactDate a, CompactDate b)
        {
            return new CompactDate((int) a - (int) b);
        }
    }
}