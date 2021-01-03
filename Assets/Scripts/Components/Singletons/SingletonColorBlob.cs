using Unity.Entities;
using UnityEngine;

namespace Components.Singletons
{
    public readonly struct SingletonColorBlob : IComponentData
    {
        public readonly BlobAssetReference<ColorArray> Provinces, Areas, Regions, Supers;

        public SingletonColorBlob(BlobAssetReference<ColorArray> provinces,
            BlobAssetReference<ColorArray> areas,
            BlobAssetReference<ColorArray> regions,
            BlobAssetReference<ColorArray> supers)
        {
            Provinces = provinces;
            Areas = areas;
            Regions = regions;
            Supers = supers;
        }
    }

    public struct ColorArray
    {
        // Use ID as index.
        public BlobArray<Color32> Lookup;
    }
}