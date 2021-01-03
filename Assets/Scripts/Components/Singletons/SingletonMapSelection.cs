using Unity.Entities;
using UnityEngine;

namespace Components.Singletons
{
    public struct SingletonMapSelection : IComponentData
    {
        // Used to communicate between Map / EscapeClearSelection (which runs every frame)
        // and Map / Click system (which runs on click from UI system).
        public bool State;
        public Color PreviousColor;
        public int PreviousClicked;
    }
}