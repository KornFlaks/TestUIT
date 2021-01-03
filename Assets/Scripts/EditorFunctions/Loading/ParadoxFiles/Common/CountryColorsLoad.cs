using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Components.Countries;
using Newtonsoft.Json;
using UnityEngine;

namespace EditorFunctions.Loading.ParadoxFiles.Common
{
    public struct CountryColorsLoad
    {
        public List<CountryCacheInfo> CountryCacheInfos;

        public CountryColorsLoad(in CountryTagsLoad countryTagsLoad)
        {
            const string colorMatch = @"^color\s*?=\s*?{(?<color>.*?)}";

            CountryCacheInfos = new List<CountryCacheInfo>(countryTagsLoad.Paths.Count + 1);

            foreach (var path in countryTagsLoad.Paths)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                var index = CountryCacheInfos.Count;
                var tag = countryTagsLoad.Tags[index];
                var color = (Color32) Color.black;

                // Getting only color. Someday I'll parse the rest.
                foreach (var line in File.ReadLines(Path.Combine(Application.streamingAssetsPath, "Common", path)))
                {
                    if (LoadMethods.CommentDetector(line, out var sliced))
                        continue;

                    var match = Regex.Match(sliced, colorMatch);
                    if (!match.Success)
                        continue;

                    color = LoadMethods.ParseColor32(match.Groups["color"].Value);
                    break;
                }

                CountryCacheInfos.Add(new CountryCacheInfo
                {
                    Name = name,
                    Tag = tag,
                    Index = index,
                    Color = color
                });
            }
        }

        public void ReadCache(string cacheFolder)
        {
            var targetFolder = Path.Combine(Application.streamingAssetsPath, cacheFolder, "Countries");

            CountryCacheInfos = Directory.EnumerateFiles(targetFolder, "*.json")
                .Select(filePath => JsonConvert.DeserializeObject<CountryCacheInfo>(File.ReadAllText(filePath)))
                .ToList();
        }
    }

    public struct CountryCacheInfo
    {
        public string Name, Tag;
        public int Index;
        public Color32 Color;

        public static explicit operator Country(CountryCacheInfo cacheInfo)
        {
            return new Country
            {
                Index = cacheInfo.Index,
                Color = cacheInfo.Color
            };
        }
    }
}