using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EditorFunctions.Loading.ParadoxFiles
{
    public static class LoadMethods
    {
        //public static Dictionary<string, string> LocalizedReplacer;
        //private const bool UseLocalization = true;

        // Thanks War Thunder Wiki Bot. 2015 - 2018. My first scripting project. RIP.
        // public static string NameCleaning(string rawName)
        // {
        //     if (UseLocalization && LocalizedReplacer.TryGetValue(rawName, out var localization))
        //         return localization;
        //
        //     return System.Globalization.CultureInfo.CurrentCulture
        //         .TextInfo.ToTitleCase(rawName.Replace('_', ' ')).Trim();
        // }

        public static string AddSpacesToSentence(string text, bool preserveAcronyms = false)
        {
            // https://stackoverflow.com/questions/272633/add-spaces-before-capital-letters
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            var newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (var i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if (text[i - 1] != ' ' && !char.IsUpper(text[i - 1]) ||
                        preserveAcronyms && char.IsUpper(text[i - 1]) &&
                        i < text.Length - 1 && !char.IsUpper(text[i + 1]))
                        newText.Append(' ');
                newText.Append(text[i]);
            }

            return newText.ToString();
        }

        public static bool YesNoConverter(string word)
        {
            return word.Trim() switch
            {
                "yes" => true,
                "no" => false,
                _ => throw new Exception("Unknown yes/no. " + word)
            };
        }

        public static Color32 ParseColor32(string line)
        {
            line = Regex.Replace(line, @"\t", " ");
            var subColor = Regex.Match(line, @"\d.*\d").Value
                .Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            if (subColor.Length != 3)
                throw new Exception("Color invalid. { R G B }. Found: " + line);

            if (FloatDetector())
            {
                byte.TryParse(subColor[0], out var r);
                byte.TryParse(subColor[1], out var g);
                byte.TryParse(subColor[2], out var b);
                return new Color32(r, g, b, 255);
            }
            else
            {
                float.TryParse(subColor[0], out var r);
                float.TryParse(subColor[1], out var g);
                float.TryParse(subColor[2], out var b);
                return new Color(r, g, b, 1);
            }

            bool FloatDetector()
            {
                var points = line.Count(x => x.Equals('.'));
                switch (points)
                {
                    case 3:
                    case 2:
                    case 1:
                        return false;
                    case 0:
                        return true;
                    default:
                        throw new Exception("Color invalid. { R G B }: " + line);
                }
            }
        }

        public static bool CommentDetector(string line, out string sliced, bool toTrim = true)
        {
            // Comment Detector. Will also lowercase everything. Throwing away comments.
            sliced = line.ToLowerInvariant().Split(new[] {"#"}, StringSplitOptions.None)[0];
            if (toTrim)
                sliced = sliced.Trim();
            return sliced.Length == 0;
        }

        public static void EntireAllInOneRemover(ref string text, string commentReplacement = "",
            string newLineReplacement = "", string tabReplacement = " ")
        {
            // Remove comments.
            EntireCommentRemover(ref text, commentReplacement);

            // Remove new lines.
            EntireNewLineRemover(ref text, newLineReplacement);

            // Replace Tabs with Space.
            EntireTabRemover(ref text, tabReplacement);

            // Lowercase everything.
            EntireLowercase(ref text);

            // Just moved these inside.
            static void EntireLowercase(ref string text)
            {
                text = text.ToLowerInvariant();
            }

            static void EntireCommentRemover(ref string text, string replacement)
            {
                text = new Regex("#.*?$", RegexOptions.Multiline).Replace(text, replacement);
            }

            static void EntireNewLineRemover(ref string text, string replacement)
            {
                text = text.Replace("\n", replacement).Replace("\r", replacement);
            }

            static void EntireTabRemover(ref string text, string replacement)
            {
                text = text.Replace("\t", replacement);
            }
        }
    }
}