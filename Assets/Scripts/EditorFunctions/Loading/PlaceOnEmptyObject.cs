#if UNITY_EDITOR

using System.IO;
using EditorFunctions.Loading.ParadoxFiles;
using EditorFunctions.Loading.ParadoxFiles.Geography;
using UnityEngine;

namespace EditorFunctions.Loading
{
    public class PlaceOnEmptyObject : MonoBehaviour
    {
        public string CacheFolder;

        [Header("File Paths: StreamingAssets / < Path to File >")]
        public string ProvinceMap;

        public string Definition, Area, Region, SuperRegion;

        [Header("Folder Paths: StreamingAssets / < Path To Directory >")]
        public string CountryTags;

        public string ProvinceHistories;

        // This class is basically a bus stop for inspector values and the various loading methods.

        public void ConvertToCache()
        {
            FilesLoad.Initialize(CacheFolder, Definition, Area, Region, SuperRegion,
                CountryTags, ProvinceHistories);
        }

        public void DebugGenerateColorMapBasedOffOfAreas()
        {
            var areas = new AreasLoad();
            areas.ReadCache(CacheFolder);

            var colorMap = new Texture2D(areas.ProvinceToAreas.Length, 1, TextureFormat.RGB24, false)
                {filterMode = FilterMode.Point};

            for (var index = 0; index < areas.ProvinceToAreas.Length; index++)
                colorMap.SetPixel(index, 0, areas.AreaColors[areas.ProvinceToAreas[index]]);

            colorMap.Apply();

            File.WriteAllBytes(Path.Combine(Application.dataPath, "Materials", "Maps", "DebugColorMap.png"),
                colorMap.EncodeToPNG());

            Debug.Log("Debug Color Map generated.");
        }

        public void ConvertToIdTexture()
        {
            var outputTexture = ConvertIdTexture.Process(ProvinceMap, CacheFolder);

            File.WriteAllBytes(Path.Combine("Assets", "Materials", "Maps", "ProvinceMips", "Main.png"),
                outputTexture.EncodeToPNG());

            Debug.Log("Province map generated. Don't forget to use Nomacs to generate 2 through 64 mips." +
                      " And GIMP to convert to BMP for use in clicking!");
        }
    }
}
#endif