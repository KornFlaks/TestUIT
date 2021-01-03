using Components.Singletons;
using Components.Singletons.Managed;
using Unity.Entities;
using UnityEngine;

namespace CamCode.Map
{
    public class EscapeClearSelection : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.Escape))
                return;

            var selectedSingleton = GetSingleton<SingletonMapSelection>();
            if (!selectedSingleton.State)
                return;
            
            var mapData = GetEntityQuery(typeof(SingletonManagedMapData))
                .GetSingleton<SingletonManagedMapData>();
            
            var resetColorMap = mapData.ColorMap;
            resetColorMap.SetPixel(selectedSingleton.PreviousClicked, 0, selectedSingleton.PreviousColor);
            resetColorMap.Apply();

            selectedSingleton.State = false;
            SetSingleton(selectedSingleton);
        }
    }
}