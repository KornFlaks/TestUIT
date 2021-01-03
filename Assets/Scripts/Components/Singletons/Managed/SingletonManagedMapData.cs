using System;
using System.IO;
using Unity.Entities;
using UnityEngine;

namespace Components.Singletons.Managed
{
    public class SingletonManagedMapData : IComponentData
    {
        public readonly string BinaryFilePath;
        public readonly Material ProvinceMat;
        
        private readonly int _colorMap = Shader.PropertyToID("_ColorMap");
        public Texture2D ColorMap => (Texture2D) ProvinceMat.GetTexture(_colorMap);

        public SingletonManagedMapData()
        {
            // DEBUG
            const string cacheFolder = "LoadCache";
            
            BinaryFilePath = Path.Combine(Application.streamingAssetsPath, cacheFolder, "ProvinceIDs.bytes");
            ProvinceMat = GameObject.Find("Surface").GetComponent<MeshRenderer>().sharedMaterial;
            
            if (!File.Exists(BinaryFilePath))
                throw new Exception("Province ID binary file not found!");
        }
    }
}