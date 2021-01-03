using Unity.Entities;

namespace Components.Singletons.BlobAssetConnections
{
    public readonly struct SingletonRegionSuper : IComponentData
    {
        public readonly BlobAssetReference<UpwardsBlobArray> RegionToSuper;
        public readonly BlobAssetReference<DownwardsBlobArray> SuperToRegion;

        public SingletonRegionSuper(BlobAssetReference<UpwardsBlobArray> regionToSuper,
            BlobAssetReference<DownwardsBlobArray> superToRegion)
        {
            RegionToSuper = regionToSuper;
            SuperToRegion = superToRegion;
        }
    }
}