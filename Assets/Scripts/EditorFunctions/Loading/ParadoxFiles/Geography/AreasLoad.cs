using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

namespace EditorFunctions.Loading.ParadoxFiles.Geography
{
    public struct AreasLoad : IGeographicLoad<int>
    {
        public List<string> Names;
        public List<Color32> AreaColors;
        public int[] ProvinceToAreas;

        public void Generate(string areasPath, List<int> idIndices, DistinctColorList distinctColorList)
        {
            const string colorPattern =
                @"(?<areaName>(?<=\}|^)\w+?)\s*?\=\s*?\{\s*?color\s*?\=\s*?\{(?<color>.*?(?=\}))";
            const string blockPattern = @"(?<areaName>(?<=^|\}).*?(?=\=)).*?(?<provIds>(?<=\{).*?(?=\}))";
            const string colorRemovalPattern = @"color.*?}";

            ProvinceToAreas = new int[idIndices.Count];
            Names = new List<string>();
            AreaColors = new List<Color32>();
            var customColors = new Dictionary<string, Color32>();

            var idLookup = new Dictionary<int, int>(idIndices.Count);
            for (var index = 0; index < idIndices.Count; index++)
                idLookup[idIndices[index]] = index;

            var areaFile = File.ReadAllText(areasPath);

            // Triplicate remover.
            LoadMethods.EntireAllInOneRemover(ref areaFile);

            // Finding colors needs to be first.
            var colors = Regex.Match(areaFile, colorPattern);
            while (colors.Success)
            {
                customColors[colors.Groups["areaName"].Value.Trim()] =
                    LoadMethods.ParseColor32(colors.Groups["color"].Value);

                colors = colors.NextMatch();
            }

            // Removing colors.
            areaFile = Regex.Replace(areaFile, colorRemovalPattern, "");

            // Adding Unknown as default area.
            Names.Add("unknown");
            AreaColors.Add(Color.white);

            // Get blocks.
            var blocks = Regex.Match(areaFile, blockPattern);

            while (blocks.Success)
            {
                var name = blocks.Groups["areaName"].Value.Trim();
                Names.Add(name);

                if (customColors.TryGetValue(name, out var predefinedColor))
                    AreaColors.Add(predefinedColor);
                else
                    AreaColors.Add(distinctColorList.GetNextColor());

                var provinces = blocks.Groups["provIds"].Value.Trim()
                    .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                foreach (var idString in provinces)
                {
                    if (!int.TryParse(idString, out var id))
                        throw new Exception(
                            $"Invalid province id '{idString}' found in area '{Names.Last()}' parsing.");

                    if (idLookup.TryGetValue(id, out var index))
                        ProvinceToAreas[index] = Names.Count - 1;
                }

                blocks = blocks.NextMatch();
            }

            distinctColorList.ResetColorList();
        }

        public void ReadCache(string cacheFolder)
        {
            var areasFilePath = Path.Combine(Application.streamingAssetsPath, cacheFolder, "Area.json");

            var input = JsonConvert.DeserializeObject<AreasLoad>(File.ReadAllText(areasFilePath));

            Names = input.Names;
            AreaColors = input.AreaColors;
            ProvinceToAreas = input.ProvinceToAreas;
        }
    }
}