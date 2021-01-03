using System.Collections.Generic;
using EditorFunctions.Loading.ParadoxFiles.Geography;

namespace EditorFunctions.Loading.ParadoxFiles
{
    public interface IGeographicLoad<TList>
    {
        public void Generate(string filePath, List<TList> names, DistinctColorList distinctColorList);
        public void ReadCache(string cacheFolder);
    }
}