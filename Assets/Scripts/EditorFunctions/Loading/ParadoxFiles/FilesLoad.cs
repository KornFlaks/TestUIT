using System.Collections.Generic;
using System.IO;
using EditorFunctions.Loading.ParadoxFiles.Common;
using EditorFunctions.Loading.ParadoxFiles.Geography;
using Newtonsoft.Json;
using UnityEngine;

namespace EditorFunctions.Loading.ParadoxFiles
{
    public static class FilesLoad
    {
        public static void Initialize(string cacheFolder, string definition, string area, string region,
            string superRegion, string countryTags, string provinceHistories)
        {
            var distinctColorList = new DistinctColorList(true);

            Definitions(cacheFolder, definition, out var definitionOutput);

            GeographicLoad(cacheFolder, area, definitionOutput.IDIndices, distinctColorList,
                "Area", out AreasLoad areaOutput);

            GeographicLoad(cacheFolder, region, areaOutput.Names, distinctColorList,
                "Region", out RegionsLoad regionsOutput);

            GeographicLoad(cacheFolder, superRegion, regionsOutput.Names, distinctColorList,
                "SuperRegion", out SuperRegionsLoad supersOutput);

            Country(cacheFolder, countryTags, out var countryTagsLoad);

            Provinces(cacheFolder, provinceHistories, definitionOutput.Names, definitionOutput.IDIndices,
                countryTagsLoad.Tags);

            Debug.Log("Cache files successfully generated!");
        }

        private static void ClearDirectory(string targetFolder)
        {
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);
            else
                foreach (var file in Directory.EnumerateFiles(targetFolder))
                    File.Delete(file);
        }

        private static void Provinces(string cacheFolder, string path, IReadOnlyList<string> definitionNames,
            IReadOnlyList<int> idIndex, IReadOnlyList<string> tags)
        {
            var folderPath = Path.Combine(Application.streamingAssetsPath, path);

            var provinceHistories = new ProvinceHistoryLoad(folderPath, definitionNames, idIndex, tags);

            var cacheTarget = Path.Combine(Application.streamingAssetsPath, cacheFolder, "Provinces");

            ClearDirectory(cacheTarget);

            foreach (var provinceCacheInfo in provinceHistories.ProvinceCacheInfos)
                File.WriteAllText(
                    Path.Combine(cacheTarget, $"{provinceCacheInfo.Index}-{provinceCacheInfo.FileName}.json"),
                    JsonConvert.SerializeObject(provinceCacheInfo, Formatting.Indented));
        }

        private static void Country(string cacheFolder, string path, out CountryTagsLoad tags)
        {
            var tagPath = Path.Combine(Application.streamingAssetsPath, path);

            tags = new CountryTagsLoad(tagPath);
            var countryColors = new CountryColorsLoad(in tags);

            var cacheTarget = Path.Combine(Application.streamingAssetsPath, cacheFolder, "Countries");

            ClearDirectory(cacheTarget);

            foreach (var countryCacheInfo in countryColors.CountryCacheInfos)
                File.WriteAllText(Path.Combine(cacheTarget, $"{countryCacheInfo.Index}-{countryCacheInfo.Name}.json"),
                    JsonConvert.SerializeObject(countryCacheInfo, Formatting.Indented));
        }

        private static void GeographicLoad<TList, TOutput>(string cacheFolder, string path, List<TList> names,
            DistinctColorList distinctColorList, string cacheName, out TOutput output)
            where TOutput : IGeographicLoad<TList>, new()
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, path);

            output = new TOutput();
            output.Generate(filePath, names, distinctColorList);

            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, cacheFolder,
                $"{cacheName}.json"), JsonConvert.SerializeObject(output));
        }

        private static void Definitions(string cacheFolder, string path,
            out DefinitionsLoad definitionOutput)
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, path);

            // Second run, checks if ID texture generated used definitions.
            var usedPath = Path.Combine(Application.streamingAssetsPath, cacheFolder, "usedDefinitions.json");

            definitionOutput = new DefinitionsLoad();
            definitionOutput.Generate(filePath, usedPath);

            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, cacheFolder,
                "Definition.json"), JsonConvert.SerializeObject(definitionOutput));
        }
    }
}