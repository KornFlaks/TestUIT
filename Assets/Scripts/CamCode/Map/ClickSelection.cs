using System;
using System.IO;
using Components.Singletons;
using Components.Singletons.Managed;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace CamCode.Map
{
    public class ClickSelection : SystemBase
    {
        // TODO: Generate this at runtime.
        private const int Height = 4096;
        private const int Width = 8192;

        private SingletonManagedMapData _mapData;
        
        public struct Activate : IComponentData
        {
            // Empty tag required to run system.
        }

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<Activate>();
            
            // Creating singleton for map selection. Can be done in create.
            var mapSelect = EntityManager.CreateEntity(typeof(SingletonMapSelection));
            EntityManager.SetComponentData(mapSelect, new SingletonMapSelection{State = false});
        }

        protected override void OnStartRunning()
        {
            var mapData = GetEntityQuery(typeof(SingletonManagedMapData));
            if (!mapData.IsEmpty)
                _mapData = mapData.GetSingleton<SingletonManagedMapData>();
            else
            {
                // Creation of managed map data.
                _mapData = new SingletonManagedMapData();
                EntityManager.SetComponentData(
                    EntityManager.CreateEntity(typeof(SingletonManagedMapData)), _mapData);
            }
        }

        protected override void OnUpdate()
        {
            // Moved to EscapeClearSelection
            //if (_somethingSelected && Input.GetKeyDown(KeyCode.Escape))
            //{
            //}

            // Activated in Overlay with singleton.
            //if (!Input.GetMouseButtonDown(0))
            //    return;
            
            // Resetting click detection.
            EntityManager.DestroyEntity(GetSingletonEntity<Activate>());

            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo))
                return;

            var clickedIndex = (long) math.floor(hitInfo.textureCoord.x * Width)
                               + (long) (math.floor(hitInfo.textureCoord.y * Height) * Width);

            int clicked;
            using (var binaryReader = new BinaryReader(File.Open(_mapData.BinaryFilePath, FileMode.Open)))
            {
                binaryReader.BaseStream.Position = clickedIndex * sizeof(ushort);
                clicked = binaryReader.ReadUInt16();
            }

            var colorMap = _mapData.ColorMap;

            // Singleton created in escape clear.
            var selectionSingleton = GetSingleton<SingletonMapSelection>();
            if (selectionSingleton.State)
                colorMap.SetPixel(selectionSingleton.PreviousClicked, 0, selectionSingleton.PreviousColor);

            selectionSingleton.PreviousColor = colorMap.GetPixel(clicked, 0);

            colorMap.SetPixel(clicked, 0, Color.red);
            colorMap.Apply();

            selectionSingleton.PreviousClicked = clicked;
            selectionSingleton.State = true;
            SetSingleton(selectionSingleton);

            //Debug.LogError($"Clicked: {clicked}. R: {(byte) clicked}. G: {(byte) (clicked >> 8)}.");
        }
    }
}