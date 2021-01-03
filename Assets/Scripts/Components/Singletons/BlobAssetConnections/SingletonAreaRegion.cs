using Unity.Entities;

namespace Components.Singletons.BlobAssetConnections
{
    public readonly struct SingletonAreaRegion : IComponentData
    {
        public readonly BlobAssetReference<UpwardsBlobArray> AreaToRegion;
        public readonly BlobAssetReference<DownwardsBlobArray> RegionToArea;

        public SingletonAreaRegion(BlobAssetReference<UpwardsBlobArray> areaToRegion,
            BlobAssetReference<DownwardsBlobArray> regionToArea)
        {
            AreaToRegion = areaToRegion;
            RegionToArea = regionToArea;
        }
    }
}