using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EditorFunctions.Loading.ParadoxFiles.Geography
{
    public class DistinctColorList
    {
        private List<Color> _colorList;
        private int _counter;

        public DistinctColorList(bool readFile)
        {
            if (!readFile)
                return;

            ReadDistinctColorList();
        }

        private void ReadDistinctColorList()
        {
            _colorList = new List<Color>();
            _counter = 0;

            var colors = File.ReadLines(Path.Combine(Application.streamingAssetsPath, "DistinctColors.txt"));

            foreach (var hexColor in colors)
            {
                if (!ColorUtility.TryParseHtmlString(hexColor, out var convertedColor))
                    continue;

                // Checking if the color is too dark.
                if (convertedColor.grayscale < 0.2)
                    continue;

                _colorList.Add(convertedColor);
            }
        }

        public void RandomizeColor()
        {
            _counter = (int) (Random.value * _colorList.Count);
        }

        public Color GetNextColor()
        {
            if (_counter == _colorList.Count)
                RandomizeColor();

            return _colorList[_counter++];
        }

        public void ResetColorList()
        {
            _counter = 0;
        }
    }
}