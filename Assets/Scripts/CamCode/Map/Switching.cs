using System;
using System.Linq;
using Components.Countries;
using Components.Provinces;
using Components.Singletons;
using Components.Singletons.BlobAssetConnections;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace CamCode.Map
{
    public class Switching : SystemBase
    {
        private readonly int _colorMap = Shader.PropertyToID("_ColorMap");
        private MapMode _currentMode;

        private MapMode _maxMapMode, _minMapMode;
        private Texture2D _provinceColors;
        private int _provinceCount;

        private SingletonColorBlob _singletonColors;
        private SingletonUpwardsIndex _singletonUpwards;
        private Material _surfaceMat;

        protected override void OnStartRunning()
        {
            var mapModes = Enum.GetValues(typeof(MapMode)).Cast<MapMode>().ToArray();
            _maxMapMode = mapModes.Max();
            _minMapMode = mapModes.Min();

            _currentMode = MapMode.Political;
            _surfaceMat = GameObject.Find("Surface").GetComponent<MeshRenderer>().sharedMaterial;

            _singletonColors = EntityManager.GetComponentData<SingletonColorBlob>(
                GetSingletonEntity<SingletonColorBlob>());

            _singletonUpwards = EntityManager.GetComponentData<SingletonUpwardsIndex>(
                GetSingletonEntity<SingletonUpwardsIndex>());

            _provinceCount = _singletonColors.Provinces.Value.Lookup.Length;
            _provinceColors = new Texture2D(_provinceCount, 1, TextureFormat.RGBA32, false, false)
                {filterMode = FilterMode.Point};

            // Crashes the game. RIP instant switch.
            SwitchMaps(_currentMode);
        }

        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.Space))
                return;

            if (++_currentMode > _maxMapMode)
                _currentMode = _minMapMode;

            //Debug.Log("Switching maps to " + _currentMode);

            SwitchMaps(_currentMode);
        }

        private void SwitchMaps(MapMode target)
        {
            var textureColors = _provinceColors.GetRawTextureData<Color32>();

            var mainChain = Dependency;

            switch (target)
            {
                case MapMode.Political:
                    var countryLookup = GetComponentDataFromEntity<Country>(true);
                    mainChain = Entities
                        .WithNativeDisableParallelForRestriction(textureColors)
                        .WithReadOnly(countryLookup)
                        .ForEach((in Province province, in Ownership ownership) =>
                        {
                            textureColors[province.Index] = countryLookup[ownership.Owner].Color;
                        }).ScheduleParallel(mainChain);
                    break;
                case MapMode.Province:
                case MapMode.Area:
                case MapMode.Region:
                case MapMode.SuperRegion:
                    var colorMapping = new NativeArray<int>(_provinceCount, Allocator.TempJob);
                    mainChain = new ConnectProvincesToTarget
                    {
                        CurrentMode = _currentMode,
                        UpwardsIndex = _singletonUpwards,
                        Output = colorMapping
                    }.Schedule(_provinceCount, 1, mainChain);

                    mainChain = new PopulateColorMap
                    {
                        CurrentMode = _currentMode,
                        ColorBlob = _singletonColors,
                        LookupMap = colorMapping,
                        Output = textureColors
                    }.Schedule(_provinceCount, 1, mainChain);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            mainChain.Complete();

            _provinceColors.Apply();
            _surfaceMat.SetTexture(_colorMap, _provinceColors);
        }

        [BurstCompile]
        private struct PopulateColorMap : IJobParallelFor
        {
            [ReadOnly] public MapMode CurrentMode;
            [ReadOnly] public SingletonColorBlob ColorBlob;
            [DeallocateOnJobCompletion] public NativeArray<int> LookupMap;

            [WriteOnly] public NativeArray<Color32> Output;

            public void Execute(int index)
            {
                ref var provinces = ref ColorBlob.Provinces.Value.Lookup;
                ref var areas = ref ColorBlob.Areas.Value.Lookup;
                ref var regions = ref ColorBlob.Regions.Value.Lookup;
                ref var supers = ref ColorBlob.Supers.Value.Lookup;

                var targetIndex = LookupMap[index];
                Output[index] = CurrentMode switch
                {
                    MapMode.Province => provinces[targetIndex],
                    MapMode.Area => areas[targetIndex],
                    MapMode.Region => regions[targetIndex],
                    MapMode.SuperRegion => supers[targetIndex],
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        [BurstCompile]
        private struct ConnectProvincesToTarget : IJobParallelFor
        {
            [ReadOnly] public MapMode CurrentMode;
            [ReadOnly] public SingletonUpwardsIndex UpwardsIndex;

            [WriteOnly] public NativeArray<int> Output;

            public void Execute(int index)
            {
                ref var provToArea = ref UpwardsIndex.ProvinceToArea.Value.Lookup;
                ref var areaToRegion = ref UpwardsIndex.AreaToRegion.Value.Lookup;
                ref var regionToSuper = ref UpwardsIndex.RegionToSuper.Value.Lookup;

                Output[index] = CurrentMode switch
                {
                    MapMode.Province => index,
                    MapMode.Area => provToArea[index],
                    MapMode.Region => areaToRegion[provToArea[index]],
                    MapMode.SuperRegion => regionToSuper[areaToRegion[provToArea[index]]],
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private enum MapMode
        {
            Province,
            Area,
            Region,
            SuperRegion,
            Political
        }
    }
}