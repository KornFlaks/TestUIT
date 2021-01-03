using Unity.Entities;

namespace Components.Provinces
{
    //[StructLayout(LayoutKind.Explicit)]
    public struct Ownership : IComponentData
    {
        public Entity Owner, Controller;

        public Ownership(Entity owner, Entity controller)
        {
            Owner = owner;
            Controller = controller;
        }

        /*[JsonProperty] [FieldOffset(0)] private int EncodedInt; // Don't actually use this value.

        [JsonIgnore] [FieldOffset(0)] private byte OwnerOne;
        [JsonIgnore] [FieldOffset(1)] private byte OwnerTwo;
        [JsonIgnore] [FieldOffset(2)] private byte ControllerOne;
        [JsonIgnore] [FieldOffset(3)] private byte ControllerTwo;

        public int Owner
        {
            get => OwnerOne << (0 + OwnerTwo) << 8;
            set
            {
                OwnerOne = (byte) (value >> 0);
                OwnerTwo = (byte) (value >> 8);
            }
        }

        public int Controller
        {
            get => ControllerOne << (0 + ControllerTwo) << 8;
            set
            {
                ControllerOne = (byte) (value >> 0);
                ControllerTwo = (byte) (value >> 8);
            }
        }
        
        public Ownership(int owner, int controller) : this()
        {
            Owner = owner;
            Controller = controller;
        }

        public override string ToString()
        {
            return $"Owner: {Owner}. Controller: {Controller}";
        }*/
    }
}