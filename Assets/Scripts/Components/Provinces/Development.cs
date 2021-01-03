using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Provinces
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Development : IComponentData
    {
        [JsonProperty] [FieldOffset(0)] private int EncodedInt; // Don't actually use this value.

        [JsonIgnore] [FieldOffset(0)] public byte Tax;
        [JsonIgnore] [FieldOffset(1)] public byte Production;
        [JsonIgnore] [FieldOffset(2)] public byte Manpower;
        [JsonIgnore] [FieldOffset(3)] private readonly byte NotUsed;

        public Development(byte tax, byte production, byte manpower) : this()
        {
            Tax = (byte) math.clamp(tax, 0, 255);
            Production = (byte) math.clamp(production, 0, 255);
            Manpower = (byte) math.clamp(manpower, 0, 255);
        }

        public override string ToString()
        {
            return $"Tax: {Tax}. Production: {Production}. Manpower: {Manpower}.";
        }
    }
}