using EditorFunctions.Loading.ParadoxFiles.Common;
using Unity.Entities;
using UnityEngine;

namespace Components.Countries
{
    public struct Country : IComponentData
    {
        public int Index;
        public Color32 Color; // Dynamic color, oh boy!

        public Country(CountryCacheInfo cacheInfo)
        {
            Index = cacheInfo.Index;
            Color = cacheInfo.Color;
        }
    }
}