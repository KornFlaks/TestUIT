using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

namespace EditorFunctions.Loading.ParadoxFiles.Geography
{
    public struct SuperRegionsLoad : IGeographicLoad<string>
    {
        public List<string> Names;
        public List<Color32> Colors;
        public int[] RegionsToSupers;

        public void Generate(string filePath, List<string> regionNames, DistinctColorList distinctColorList)
        {
            const string blockPattern =
                @"(?<superName>(?<=\}|^)\s*?\w+?)\s*?\=\s*?{(?<regions>.*?(?=\}))";

            Names = new List<string>();
            RegionsToSupers = new int[regionNames.Count];
            Colors = new List<Color32>();

            var rawFile = File.ReadAllText(filePath);

            // Triplicate remover.
            LoadMethods.EntireAllInOneRemover(ref rawFile);

            var regionLookup = new Dictionary<string, int>(regionNames.Count);
            for (var index = 0; index < regionNames.Count; index++)
                regionLookup[regionNames[index]] = index;

            // Adding Unknown as default area.
            Names.Add("unknown");
            Colors.Add(Color.white);

            // Get blocks.
            var blocks = Regex.Match(rawFile, blockPattern);

            while (blocks.Success)
            {
                var name = blocks.Groups["superName"].Value.Trim();
                Names.Add(name);

                Colors.Add(distinctColorList.GetNextColor());

                var regionSplit = blocks.Groups["regions"].Value.Trim()
                    .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                foreach (var areaString in regionSplit)
                    if (regionLookup.TryGetValue(areaString, out var index))
                        RegionsToSupers[index] = Names.Count - 1;

                blocks = blocks.NextMatch();
            }

            distinctColorList.ResetColorList();
        }

        public void ReadCache(string cacheFolder)
        {
            var areasFilePath = Path.Combine(Application.streamingAssetsPath, cacheFolder,
                "SuperRegion.json");

            var input = JsonConvert.DeserializeObject<SuperRegionsLoad>(File.ReadAllText(areasFilePath));

            Colors = input.Colors;
            Names = input.Names;
            RegionsToSupers = input.RegionsToSupers;
        }
    }
}