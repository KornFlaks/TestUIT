using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace EditorFunctions.Loading.ParadoxFiles.Geography
{
    public struct DefinitionsLoad
    {
        public List<string> Names;
        public List<int> IDIndices;
        public List<Color32> Colors;

        public void Generate(string definitionsPath, string usedPath)
        {
            Names = new List<string>();
            Colors = new List<Color32>();
            IDIndices = new List<int>();

            // Adding clear polar and default regions
            Names.Add("Default");
            Colors.Add(new Color32(0, 0, 0, 0));
            IDIndices.Add(0);

            var secondPass = File.Exists(usedPath);
            var usedCheck = new HashSet<Color32>();

            if (secondPass)
            {
                var usedRaw = JsonConvert.DeserializeObject<Color32[]>(File.ReadAllText(usedPath));
                foreach (var usedColor in usedRaw)
                    usedCheck.Add(usedColor);

                Debug.Log($"Second pass detected. Size of used provinces: {usedRaw.Length}.");
            }

            foreach (var rawLine in File.ReadLines(definitionsPath, Encoding.GetEncoding(1252)))
            {
                if (LoadMethods.CommentDetector(rawLine, out var line))
                    continue;

                var subStringed = line.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                if (subStringed.Length < 5) throw new Exception("Definitions file not following format! " + line);
                // Num;R;G;B;Name;x
                int.TryParse(subStringed[0], out var provNum);
                byte.TryParse(subStringed[1], out var red);
                byte.TryParse(subStringed[2], out var green);
                byte.TryParse(subStringed[3], out var blue);
                var foundColor = new Color32(red, green, blue, 255);

                if (secondPass && !usedCheck.Contains(foundColor))
                    continue;

                Names.Add(subStringed[4].Trim());
                Colors.Add(foundColor);
                IDIndices.Add(provNum);
            }

            Debug.Log(IDIndices.Count);
        }

        public void ReadCache(string cacheFolder)
        {
            var areasFilePath = Path.Combine(Application.streamingAssetsPath, cacheFolder,
                "Definition.json");

            var input = JsonConvert.DeserializeObject<DefinitionsLoad>(File.ReadAllText(areasFilePath));

            Names = input.Names;
            Colors = input.Colors;
            IDIndices = input.IDIndices;
        }
    }
}