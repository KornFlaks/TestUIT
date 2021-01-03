using Components.General;
using Unity.Entities;

namespace Components.Provinces
{
    public struct Core : IBufferElementData
    {
        public Entity Country;
        public CompactDate Expiration;

        public Core(Entity country) : this()
        {
            Country = country;
        }
    }
}