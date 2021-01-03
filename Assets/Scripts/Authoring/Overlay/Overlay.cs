using Authoring.Overlay.TopBarButtons;
using CamCode.Map;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Authoring.Overlay
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class Overlay : SystemBase
    {
        // One file to handle everything UI?
        // That sounds horrifying. Let's do it. - KornFlaks (2021).

        private ProductionVariables ProductionVariables;

        protected override void OnStartRunning()
        {
            var overlayGameObject = GameObject.Find("Overlay");
            var uiDocument = overlayGameObject.GetComponent<UIDocument>();

            var root = uiDocument.rootVisualElement;

            // Stylesheets and sprites found in inspector Mono Bootstrapped.
            var monoBootstrap = overlayGameObject.GetComponent<OverlayMonoBootstrap>();
            foreach (var styleSheet in monoBootstrap.InsertStyleSheets)
                root.styleSheets.Add(styleSheet);

            UIClickDetectionCreate(root);
            ButtonBackgrounds.Create(root, in monoBootstrap, out var topBarRoot, out var topBarButtons);
            Production.Create(topBarButtons.Production, monoBootstrap.ProductionSprites, out ProductionVariables);
        }

        private void UIClickDetectionCreate(VisualElement root)
        {
            // Used for determining if click is on UI or on game map.
            // https://forum.unity.com/threads/is-there-a-way-to-block-raycasts.943963/#post-6188745
            new VisualElement()
                .Set("ClickDetection")
                .SetParent(root)
                .RegisterCallback<MouseDownEvent>(evt 
                    => EntityManager.CreateEntity(typeof(ClickSelection.Activate)));
        }

        protected override void OnUpdate()
        {
            Enabled = false;
        }
    }
}