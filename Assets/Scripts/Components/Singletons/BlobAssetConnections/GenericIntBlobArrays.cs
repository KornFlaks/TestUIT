using Unity.Entities;

namespace Components.Singletons.BlobAssetConnections
{
    public struct UpwardsBlobArray
    {
        // Generic int - int lookup array.
        // Down -> Up
        public BlobArray<int> Lookup;
    }

    public struct DownwardsBlobArray
    {
        // Generic int - int[] lookup array.
        // Up -> Down
        public BlobArray<BlobArray<int>> Lookup;
    }
}