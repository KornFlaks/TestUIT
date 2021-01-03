using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Authoring.Overlay
{
    public class OverlayMonoBootstrap : MonoBehaviour
    {
        public List<StyleSheet> InsertStyleSheets;
        public TopBarSprites TopBar;
        public ProdButtons ProductionSprites;

        [Serializable]
        public struct TopBarSprites
        {
            public Sprite Background, Paper;

            public BooleanButtons Production,
                Budget,
                Technology,
                Politics,
                Population,
                Trade,
                Diplomacy,
                Military;
        }
    }

    [Serializable]
    public struct BooleanButtons
    {
        public Sprite On, Off;
        public Vector2 Position;
    }

    [Serializable]
    public struct ProdButtons
    {
        public BooleanButtons Building, Factory, Unemployment;
    }
}