using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

namespace EditorFunctions.Loading.ParadoxFiles.Common
{
    public struct ProvinceHistoryLoad
    {
        public ProvinceCacheInfo[] ProvinceCacheInfos;

        public ProvinceHistoryLoad(string path, IReadOnlyList<string> provinceNames, IReadOnlyList<int> idIndex,
            IReadOnlyList<string> countryTags)
        {
            ProvinceCacheInfos = new ProvinceCacheInfo[provinceNames.Count];

            // var provinceLookup = new Dictionary<string, int>(provinceNames.Count);
            // for (var index = 0; index < provinceNames.Count; index++)
            //     provinceLookup[provinceNames[index]] = index;

            var idLookup = new Dictionary<int, int>(idIndex.Count);
            for (var index = 0; index < idIndex.Count; index++)
                idLookup[idIndex[index]] = index;

            var tagLookup = new Dictionary<string, int>(countryTags.Count);
            for (var index = 0; index < countryTags.Count; index++)
                tagLookup[countryTags[index]] = index;

            var nativeTag = tagLookup["nat"];
            var oceanTag = tagLookup["ocean"];

            foreach (var filePath in Directory.EnumerateFiles(path, "*.txt"))
            {
                var fileMatch = Regex.Match(Path.GetFileNameWithoutExtension(filePath), @"(?<index>.*?)\-(?<name>.*)");
                var name = fileMatch.Groups["name"].Value.Trim().ToLowerInvariant();
                var prevIndex = fileMatch.Groups["index"].Value.Trim();

                if (!int.TryParse(prevIndex, out var index))
                {
                    Debug.LogError($"Unknown index in file name: {filePath}");
                    continue;
                }

                if (!idLookup.TryGetValue(index, out index))
                {
                    Debug.LogError($"Unknown index lookup: {filePath}.");
                    continue;
                }

                var defineName = provinceNames[index];
                //if (!defineName.Equals(name, StringComparison.Ordinal))
                //    Debug.Log($"Definition name: {defineName} and file name: {name}. Mismatched.");

                var target = new ProvinceCacheInfo
                {
                    Cores = new List<int>(), FileName = name,
                    Name = defineName, Index = index
                };
                var isCity = false;

                foreach (var rawLine in File.ReadLines(filePath, Encoding.GetEncoding(1252)))
                {
                    if (LoadMethods.CommentDetector(rawLine, out var sliced, false))
                        continue;

                    var variable = Regex.Match(sliced, @"^(?<key>.*?)\=(?<value>.*)");
                    // Very lazy check for nested declarations. Probably shouldn't rely on this.
                    var key = variable.Groups["key"].Value;
                    if (!variable.Success || key.IndexOfAny(new[] {'\t', ' '}) == 0)
                        //Debug.Log($"Failed parsing: {sliced}");
                        continue;

                    key = key.Trim();

                    if (key.IndexOf('1') == 0)
                        break;

                    var value = variable.Groups["value"].Value.Trim();
                    switch (key)
                    {
                        case "add_core":
                            if (!tagLookup.TryGetValue(value, out var core))
                                Debug.Log($"Unknown country tag in cores: {value}.");

                            target.Cores.Add(core);
                            break;
                        case "owner":
                            if (!tagLookup.TryGetValue(value, out target.Owner))
                                Debug.Log($"Unknown country tag in owner: {value}.");
                            break;
                        case "controller":
                            if (!tagLookup.TryGetValue(value, out target.Controller))
                                Debug.Log($"Unknown country tag in controller: {value}.");
                            break;
                        case "religion":
                            target.Religion = value;
                            break;
                        case "culture":
                            target.Culture = value;
                            break;
                        case "base_tax":
                            target.Tax = int.Parse(value);
                            break;
                        case "base_production":
                            target.Production = int.Parse(value);
                            break;
                        case "base_manpower":
                            target.Manpower = int.Parse(value);
                            break;
                        case "capital":
                            target.Capital = value.Replace("\"", "");
                            break;
                        case "is_city":
                            isCity = LoadMethods.YesNoConverter(value);
                            break;
                        case "trade_goods":
                            target.Goods = value;
                            break;
                    }
                }

                if (!isCity)
                {
                    if (target.Tax == 0
                        && string.IsNullOrWhiteSpace(target.Capital)
                        && string.IsNullOrWhiteSpace(target.Goods))
                    {
                        // Ocean, splash splash.
                        target.Owner = oceanTag;
                        target.Controller = oceanTag;
                    }
                    else
                    {
                        // Native land, un colonized
                        target.Owner = nativeTag;
                        target.Controller = nativeTag;
                    }
                }

                ProvinceCacheInfos[index] = target;
            }

            for (var i = 0; i < ProvinceCacheInfos.Length; i++)
            {
                if (ProvinceCacheInfos[i].FileName != null)
                    continue;

                var name = provinceNames[i];
                Debug.Log("Orphaned id index: " + idIndex[i] + " of name: " + name + " of index: " + i);
                ProvinceCacheInfos[i] = new ProvinceCacheInfo
                {
                    FileName = name,
                    Name = name,
                    Index = i
                };
            }
        }

        public void ReadCache(string cacheFolder)
        {
            var targetFolder = Path.Combine(Application.streamingAssetsPath, cacheFolder, "Provinces");

            ProvinceCacheInfos = Directory.EnumerateFiles(targetFolder, "*.json")
                .Select(filePath => JsonConvert.DeserializeObject<ProvinceCacheInfo>(File.ReadAllText(filePath)))
                .ToArray();
        }
    }

    public struct ProvinceCacheInfo
    {
        [JsonIgnore] public string FileName;

        public List<int> Cores;
        public string Name, Capital;
        public int Index, Owner, Controller;

        public int Tax, Production, Manpower;

        // Not parsed or used.
        public string Culture, Religion, Goods;
    }
}