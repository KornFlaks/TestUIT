using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EditorFunctions.Loading.ParadoxFiles.Common
{
    public struct CountryTagsLoad
    {
        public List<string> Tags, Paths;

        public CountryTagsLoad(string folderPath)
        {
            Tags = new List<string>();
            Paths = new List<string>();

            foreach (var filePath in Directory.EnumerateFiles(folderPath, "*.txt"))
            foreach (var rawCommonLine in File.ReadLines(filePath))
            {
                if (LoadMethods.CommentDetector(rawCommonLine, out var line))
                    continue;

                var tag = Regex.Match(line, @"^.+?(?=\=)").Value.Trim();
                var targetFile = Regex.Match(line, "(?<=\").+(?=\")").Value.Trim();

                if (!File.Exists(Path.Combine(Application.streamingAssetsPath, "Common", targetFile)))
                    Debug.Log("Not found: " + targetFile);

                Tags.Add(tag);
                Paths.Add(targetFile);
            }
        }
    }
}