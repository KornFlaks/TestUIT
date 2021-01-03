using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

namespace EditorFunctions.Loading.ParadoxFiles.Geography
{
    public struct RegionsLoad : IGeographicLoad<string>
    {
        public List<string> Names;
        public List<Color32> RegionColors;
        public int[] AreaToRegions;

        public void Generate(string filePath, List<string> areaNames, DistinctColorList distinctColorList)
        {
            const string monsoonPattern = @"\s*?monsoon\s*?=\s*?{.*?}\s*?";
            const string blockPattern =
                @"(?<regionName>(?<=\}|^)\s*?\w+?)\s*?\=\s*?\{\s*?areas\s*?\=\s*?\{(?<areas>.*?(?=\}\s*?\}))";

            Names = new List<string>();
            AreaToRegions = new int[areaNames.Count];
            RegionColors = new List<Color32>();

            var rawFile = File.ReadAllText(filePath);

            // Triplicate remover.
            LoadMethods.EntireAllInOneRemover(ref rawFile);

            var areaLookup = new Dictionary<string, int>(areaNames.Count);
            for (var index = 0; index < areaNames.Count; index++)
                areaLookup[areaNames[index]] = index;

            // Not using monsoon data, removing them all.
            rawFile = Regex.Replace(rawFile, monsoonPattern, "");

            // Adding Unknown as default area.
            Names.Add("unknown");
            RegionColors.Add(Color.white);

            // Get blocks.
            var blocks = Regex.Match(rawFile, blockPattern);

            while (blocks.Success)
            {
                var name = blocks.Groups["regionName"].Value.Trim();
                Names.Add(name);

                RegionColors.Add(distinctColorList.GetNextColor());

                var areaSplit = blocks.Groups["areas"].Value.Trim()
                    .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                foreach (var areaString in areaSplit)
                    if (areaLookup.TryGetValue(areaString, out var index))
                        AreaToRegions[index] = Names.Count - 1;

                blocks = blocks.NextMatch();
            }

            distinctColorList.ResetColorList();
        }

        public void ReadCache(string cacheFolder)
        {
            var areasFilePath = Path.Combine(Application.streamingAssetsPath, cacheFolder,
                "Region.json");

            var input = JsonConvert.DeserializeObject<RegionsLoad>(File.ReadAllText(areasFilePath));

            Names = input.Names;
            RegionColors = input.RegionColors;
            AreaToRegions = input.AreaToRegions;
        }
    }
}