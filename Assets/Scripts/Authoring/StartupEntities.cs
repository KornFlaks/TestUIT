using System;
using System.Collections.Generic;
using System.Linq;
using Components.Countries;
using Components.Provinces;
using Components.Singletons;
using Components.Singletons.BlobAssetConnections;
using Components.Singletons.Managed;
using EditorFunctions.Loading.ParadoxFiles.Common;
using EditorFunctions.Loading.ParadoxFiles.Geography;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class StartupEntities : SystemBase
    {
        private const string CacheFolder = "LoadCache"; // Ehh, whatever.
        private List<IDisposable> _blobAssetReferences;

        protected override void OnStartRunning()
        {
            Debug.Log("CACHE READ START!");

            _blobAssetReferences = new List<IDisposable>();

            var definitions = new DefinitionsLoad();
            definitions.ReadCache(CacheFolder);

            var areas = new AreasLoad();
            areas.ReadCache(CacheFolder);

            var regions = new RegionsLoad();
            regions.ReadCache(CacheFolder);

            var supers = new SuperRegionsLoad();
            supers.ReadCache(CacheFolder);

            var countries = new CountryColorsLoad();
            countries.ReadCache(CacheFolder);

            var provinces = new ProvinceHistoryLoad();
            provinces.ReadCache(CacheFolder);

            GenerateCountryEntities(in countries, out var countryList);
            GenerateProvinceEntities(in provinces, countryList, out var provList);

            CreateProvinceAreaLookups(in areas, provList, out var provinceToArea);
            CreateAreaRegionLookups(in regions, out var areaToRegion);
            CreateRegionSuperLookups(in supers, out var regionToSuper);

            CreateUpwardsSingleton(provinceToArea, areaToRegion, regionToSuper);

            CreateColorBlob(definitions.Colors, areas.AreaColors, regions.RegionColors, supers.Colors);

            Debug.Log("STARTUP ENTITIES CREATED!");
        }

        protected override void OnUpdate()
        {
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            Debug.Log("BLOBS DISPOSED");
            foreach (var disposable in _blobAssetReferences)
                disposable.Dispose();
        }

        private void GenerateCountryEntities(in CountryColorsLoad countries, out Entity[] countryList)
        {
            var names = new string[countries.CountryCacheInfos.Count];
            countryList = new Entity[names.Length];

            foreach (var countryCacheInfo in countries.CountryCacheInfos)
            {
                names[countryCacheInfo.Index] = countryCacheInfo.Name;

                var targetEntity = EntityManager.CreateEntity(typeof(Country));
                EntityManager.SetComponentData(targetEntity, new Country(countryCacheInfo));
                countryList[countryCacheInfo.Index] = targetEntity;
            }

            var singleton = EntityManager.CreateEntity(typeof(SingletonManagedCountryNames));
            EntityManager.SetComponentData(singleton, new SingletonManagedCountryNames(names));
        }

        private void CreateUpwardsSingleton(BlobAssetReference<UpwardsBlobArray> p2A,
            BlobAssetReference<UpwardsBlobArray> a2R, BlobAssetReference<UpwardsBlobArray> r2S)
        {
            var target = EntityManager.CreateEntity(typeof(SingletonUpwardsIndex));
            EntityManager.SetComponentData(target,
                new SingletonUpwardsIndex(p2A, a2R, r2S));
        }

        private void CreateColorBlob(IReadOnlyList<Color32> definitions, IReadOnlyList<Color32> areas,
            IReadOnlyList<Color32> regions, IReadOnlyList<Color32> supers)
        {
            CreateColorBlob(definitions, out var defineReference);
            _blobAssetReferences.Add(defineReference);

            CreateColorBlob(areas, out var areaReference);
            _blobAssetReferences.Add(areaReference);

            CreateColorBlob(regions, out var regionReference);
            _blobAssetReferences.Add(regionReference);

            CreateColorBlob(supers, out var superReference);
            _blobAssetReferences.Add(superReference);

            var target = EntityManager.CreateEntity(typeof(SingletonColorBlob));
            EntityManager.SetComponentData(target, new SingletonColorBlob(defineReference,
                areaReference, regionReference, superReference));
        }

        private void CreateColorBlob(IReadOnlyList<Color32> colors, out BlobAssetReference<ColorArray> reference)
        {
            // Literally identical to Linear Blob creation. Problem is, the types are different.
            // Blob creation requires ref variable so unfortunately interfaces don't work. Very sad.
            using var colorBuilder = new BlobBuilder(Allocator.Temp);
            ref var lookupStruct = ref colorBuilder.ConstructRoot<ColorArray>();

            var downArray = colorBuilder.Allocate(ref lookupStruct.Lookup, colors.Count);
            for (var regionIndex = 0; regionIndex < colors.Count; regionIndex++)
                downArray[regionIndex] = colors[regionIndex];

            reference = colorBuilder.CreateBlobAssetReference<ColorArray>(Allocator.Persistent);
        }

        private void CreateLinearBlob(IReadOnlyList<int> downToUpArray,
            out BlobAssetReference<UpwardsBlobArray> reference)
        {
            using var downUpBuilder = new BlobBuilder(Allocator.Temp);
            ref var lookupStruct = ref downUpBuilder.ConstructRoot<UpwardsBlobArray>();

            var downArray = downUpBuilder.Allocate(ref lookupStruct.Lookup, downToUpArray.Count);
            for (var regionIndex = 0; regionIndex < downToUpArray.Count; regionIndex++)
                downArray[regionIndex] = downToUpArray[regionIndex];

            reference = downUpBuilder.CreateBlobAssetReference<UpwardsBlobArray>(Allocator.Persistent);
        }

        private void CreateDownwardsBlob(IReadOnlyList<int> downToUpArray, int upCount,
            out BlobAssetReference<DownwardsBlobArray> reference)
        {
            var upToDownArray = new List<int>[upCount];
            for (var i = 0; i < upCount; i++)
                upToDownArray[i] = new List<int>();

            for (var downIndex = 0; downIndex < downToUpArray.Count; downIndex++)
                upToDownArray[downToUpArray[downIndex]].Add(downIndex);

            using var upDownBuilder = new BlobBuilder(Allocator.Temp);
            ref var lookupStruct = ref upDownBuilder.ConstructRoot<DownwardsBlobArray>();

            var upArray = upDownBuilder.Allocate(ref lookupStruct.Lookup, upCount);
            for (var upIndex = 0; upIndex < upCount; upIndex++)
            {
                var targetUpDownList = upToDownArray[upIndex];
                var downArray = upDownBuilder.Allocate(ref upArray[upIndex], targetUpDownList.Count);
                for (var i = 0; i < targetUpDownList.Count; i++)
                    downArray[i] = targetUpDownList[i];
            }

            reference = upDownBuilder.CreateBlobAssetReference<DownwardsBlobArray>(Allocator.Persistent);
        }

        private void CreateRegionSuperLookups(in SuperRegionsLoad supers,
            out BlobAssetReference<UpwardsBlobArray> regionToSuper)
        {
            // Region (int) -> Super Region (int)
            CreateLinearBlob(supers.RegionsToSupers, out regionToSuper);
            _blobAssetReferences.Add(regionToSuper);

            // Super Region (int) -> Region (int)
            CreateDownwardsBlob(supers.RegionsToSupers, supers.Names.Count, out var superToRegion);
            _blobAssetReferences.Add(superToRegion);

            var target = EntityManager.CreateEntity(typeof(SingletonRegionSuper));
            EntityManager.SetComponentData(target, new SingletonRegionSuper(regionToSuper, superToRegion));
        }

        private void CreateAreaRegionLookups(in RegionsLoad regions,
            out BlobAssetReference<UpwardsBlobArray> areaToRegion)
        {
            // Area (int) -> Region (int)
            CreateLinearBlob(regions.AreaToRegions, out areaToRegion);
            _blobAssetReferences.Add(areaToRegion);

            // Region (int) -> Area (int)
            CreateDownwardsBlob(regions.AreaToRegions, regions.Names.Count, out var regionToArea);
            _blobAssetReferences.Add(regionToArea);

            var target = EntityManager.CreateEntity(typeof(SingletonAreaRegion));
            EntityManager.SetComponentData(target, new SingletonAreaRegion(areaToRegion, regionToArea));
        }

        private void CreateProvinceAreaLookups(in AreasLoad areas, IReadOnlyList<Entity> provList,
            out BlobAssetReference<UpwardsBlobArray> provinceToArea)
        {
            // Province Index (int) -> Area (int)
            CreateLinearBlob(areas.ProvinceToAreas, out provinceToArea);
            _blobAssetReferences.Add(provinceToArea);

            // Area (int) -> Province (Entity)
            var areaLookups = new List<Entity>[areas.Names.Count];
            for (var i = 0; i < areaLookups.Length; i++)
                areaLookups[i] = new List<Entity>();

            for (var provIndex = 0; provIndex < areas.ProvinceToAreas.Length; provIndex++)
                areaLookups[areas.ProvinceToAreas[provIndex]].Add(provList[provIndex]);

            BlobAssetReference<AreaProvinceEntity> areaToProvince;
            using (var areaToProv = new BlobBuilder(Allocator.Temp))
            {
                ref var lookupStruct = ref areaToProv.ConstructRoot<AreaProvinceEntity>();

                var areaArray = areaToProv.Allocate(ref lookupStruct.Lookup, areaLookups.Length);
                for (var areaIndex = 0; areaIndex < areaLookups.Length; areaIndex++)
                {
                    var targetEntityList = areaLookups[areaIndex];
                    var provArray = areaToProv.Allocate(ref areaArray[areaIndex], targetEntityList.Count);
                    for (var i = 0; i < targetEntityList.Count; i++)
                        provArray[i] = targetEntityList[i];
                }

                areaToProvince = areaToProv.CreateBlobAssetReference<AreaProvinceEntity>(Allocator.Persistent);
            }

            _blobAssetReferences.Add(areaToProvince);

            var target = EntityManager.CreateEntity(typeof(SingletonProvinceArea));
            EntityManager.SetComponentData(target, new SingletonProvinceArea(provinceToArea, areaToProvince));
        }

        private void GenerateProvinceEntities(in ProvinceHistoryLoad provinceHistoryLoad,
            IReadOnlyList<Entity> countryList, out Entity[] provList)
        {
            provList = new Entity[provinceHistoryLoad.ProvinceCacheInfos.Length];

            var names = new string[provList.Length];

            var provEntityArch = EntityManager.CreateArchetype(typeof(Province),
                typeof(Ownership), typeof(Development), typeof(Core));

            foreach (var cacheInfo in provinceHistoryLoad.ProvinceCacheInfos)
            {
                if (cacheInfo.Cores == null)
                    continue;

                names[cacheInfo.Index] = cacheInfo.Name;

                var target = EntityManager.CreateEntity(provEntityArch);

                EntityManager.SetComponentData(target, new Province(cacheInfo.Index));

                EntityManager.SetComponentData(target, new Development((byte) cacheInfo.Tax,
                    (byte) cacheInfo.Production, (byte) cacheInfo.Manpower));

                EntityManager.SetComponentData(target, new Ownership(countryList[cacheInfo.Owner],
                    countryList[cacheInfo.Controller]));

                //cacheInfo.Cores.Select(countryInt => new Core(countryList[countryInt])).ToArray();
                var convertedCores = cacheInfo.Cores.Select(countryInt => new Core(countryList[countryInt])).ToArray();

                using var cores = new NativeArray<Core>(convertedCores, Allocator.Temp);
                EntityManager.GetBuffer<Core>(target).AddRange(cores);

                provList[cacheInfo.Index] = target;
            }

            var singleton = EntityManager.CreateEntity(typeof(SingletonManagedProvinceNames));
            EntityManager.SetComponentData(singleton, new SingletonManagedProvinceNames(names));
        }
    }
}