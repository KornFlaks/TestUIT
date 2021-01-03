using System;
using System.IO;
using System.Linq;
using EditorFunctions.Loading.ParadoxFiles.Geography;
using Newtonsoft.Json;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace EditorFunctions.Loading
{
    public static class ConvertIdTexture
    {
        public static Texture2D Process(string provinceMap, string cacheFolder)
        {
            var rawProvinceMap = new Texture2D(1, 1, TextureFormat.RGB24, false, true)
                {filterMode = FilterMode.Point};

            var rawBytes = File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, provinceMap));
            rawProvinceMap.LoadImage(rawBytes);

            if (rawProvinceMap.height < 3)
                throw new Exception("No province map was inserted into the input property on inspector.");

            var definitions = new DefinitionsLoad();
            definitions.ReadCache(cacheFolder);

            if (definitions.Names.Count >= math.pow(2, 23))
                throw new Exception("Province number overflow. " +
                                    "Why the fuck do you have more than 8,388,608 provinces?");

            // Native hash map expands like list. This wont run in player so it doesn't matter about optimization.
            using var provinceLookup = new NativeHashMap<Color, int>(definitions.Colors.Count, Allocator.TempJob);

            for (var i = 0; i < definitions.Colors.Count; i++)
                provinceLookup.TryAdd(definitions.Colors[i], i);

            var outputTexture = new Texture2D(rawProvinceMap.width, rawProvinceMap.height,
                TextureFormat.RGBA32, true, true) {filterMode = FilterMode.Point};

            using var rawData = new NativeArray<Color32>(rawProvinceMap.GetPixels32(), Allocator.TempJob);
            using var provData = new NativeArray<ushort>(rawData.Length, Allocator.TempJob);
            var output = outputTexture.GetRawTextureData<Color32>();

            using var observedIDs = new NativeHashSet<Color>(provinceLookup.Count(), Allocator.TempJob);

            new EncodeID
            {
                ProvinceLookup = provinceLookup,
                RawData = rawData,
                Output = output,
                IndexOutput = provData,
                ObservedIDs = observedIDs.AsParallelWriter()
            }.Schedule(rawData.Length, 32).Complete();

            using var observedArray = observedIDs.ToNativeArray(Allocator.TempJob);

            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, cacheFolder, "usedDefinitions.json"),
                JsonConvert.SerializeObject(observedArray.ToArray().Select(color => (Color32) color)));

            var provArray = provData.ToArray();
            var byteArray = new byte[provArray.Length * sizeof(ushort)];
            Buffer.BlockCopy(provArray, 0, byteArray, 0, byteArray.Length);

            File.WriteAllBytes(Path.Combine(Application.streamingAssetsPath, cacheFolder, "ProvinceIDs.bytes"),
                byteArray);

            outputTexture.Apply();

            return outputTexture;
        }

        [BurstCompile]
        private struct EncodeID : IJobParallelFor
        {
            [ReadOnly] public NativeHashMap<Color, int> ProvinceLookup;
            //[ReadOnly] public NativeArray<int> AreasLookup;

            [ReadOnly] public NativeArray<Color32> RawData;
            [WriteOnly] public NativeArray<Color32> Output;
            [WriteOnly] public NativeArray<ushort> IndexOutput;
            [WriteOnly] public NativeHashSet<Color>.ParallelWriter ObservedIDs;

            public void Execute(int index)
            {
                if (!ProvinceLookup.TryGetValue(RawData[index], out var currentIndex))
                {
                    //Debug.Log("Unknown province color found. Colored blue in texture output.");
                    Output[index] = Color.blue;
                    return;
                }
                //throw new Exception($"X: {index % 8192}. Y: {index / 8192}. Color: {RawData[index]}.");

                ObservedIDs.Add(RawData[index]);

                IndexOutput[index] = (ushort) currentIndex;
                Output[index] = new Color32((byte) (currentIndex >> 0), (byte) (currentIndex >> 8),
                    0, 255); // Unused Blue: (byte) (currentIndex >> 16)
            }
        }
    }
}