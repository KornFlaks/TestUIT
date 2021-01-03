#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace EditorFunctions.InsertTexIntoMip
{
    public class InsertMono : MonoBehaviour
    {
        public Texture2D PrimaryTexture;
        public List<Texture2D> TexturesToBeInserted;

        public void Combine()
        {
            foreach (var insertTex in TexturesToBeInserted)
            {
                if (PrimaryTexture.graphicsFormat != insertTex.graphicsFormat)
                    throw new Exception("Invalid textures. Both must have same format.");

                if (PrimaryTexture.width / insertTex.width % 2 != 0
                    || PrimaryTexture.height / insertTex.height % 2 != 0)
                    throw new Exception("Invalid textures. Both textures must be a power of two.");

                var mipDestination = (int) math.log2((float) PrimaryTexture.width / insertTex.width);
                Graphics.CopyTexture(insertTex, 0, 0, PrimaryTexture, 0,
                    mipDestination);

                Debug.Log("Copied texture into primary's mip level " + mipDestination + ".");
            }

            Debug.Log("Mip override complete. Don't forget to use the created asset instead of the PNG!");

            var test = new Texture2D(PrimaryTexture.width, PrimaryTexture.height, TextureFormat.RGB24, true, true)
                {filterMode = FilterMode.Point};
            Graphics.CopyTexture(PrimaryTexture, test);

            AssetDatabase.CreateAsset(test, "Assets/Materials/Maps/ProvinceMips/ProperMipMaps.asset");
            AssetDatabase.SaveAssets();
        }
    }
}
#endif