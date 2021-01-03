using Unity.Entities;

namespace Components.Provinces
{
    public struct Province : IComponentData
    {
        public readonly int Index;

        public Province(int index)
        {
            Index = index;
        }
    }
}