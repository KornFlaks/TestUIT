using Unity.Entities;

namespace Components.Singletons.BlobAssetConnections
{
    public readonly struct SingletonUpwardsIndex : IComponentData
    {
        public readonly BlobAssetReference<UpwardsBlobArray> ProvinceToArea, AreaToRegion, RegionToSuper;

        public SingletonUpwardsIndex(BlobAssetReference<UpwardsBlobArray> provinceToArea,
            BlobAssetReference<UpwardsBlobArray> areaToRegion, BlobAssetReference<UpwardsBlobArray> regionToSuper)
        {
            ProvinceToArea = provinceToArea;
            AreaToRegion = areaToRegion;
            RegionToSuper = regionToSuper;
        }
    }
}