using Unity.Entities;

namespace Components.Singletons.BlobAssetConnections
{
    public readonly struct SingletonProvinceArea : IComponentData
    {
        public readonly BlobAssetReference<UpwardsBlobArray> ProvinceToArea;
        public readonly BlobAssetReference<AreaProvinceEntity> AreaToProvince;

        public SingletonProvinceArea(BlobAssetReference<UpwardsBlobArray> provinceToArea,
            BlobAssetReference<AreaProvinceEntity> areaToProvince)
        {
            ProvinceToArea = provinceToArea;
            AreaToProvince = areaToProvince;
        }
    }

    public struct AreaProvinceEntity
    {
        public BlobArray<BlobArray<Entity>> Lookup;
    }
}