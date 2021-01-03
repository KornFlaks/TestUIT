using UnityEngine.UIElements;

namespace Authoring.Overlay.TopBarButtons
{
    public static class ButtonBackgrounds
    {
        public static void Create(VisualElement root, in OverlayMonoBootstrap monoBootstrap,
            out VisualElement topBarRoot, out TopBarButtonElements topBarButtons)
        {
            new VisualElement()
                .SetVariable(out topBarRoot)
                .Set(nameof(topBarRoot))
                .SetSprite(monoBootstrap.TopBar.Background)
                .SetParent(root);

            topBarButtons = new TopBarButtonElements();

            new VisualElement()
                .SetVariable(out topBarButtons.Production)
                .Set(nameof(topBarButtons.Production), "TopBarButton")
                .AssignBooleanButton(monoBootstrap.TopBar.Production, false)
                .SetParent(topBarRoot);

            new VisualElement()
                .SetVariable(out topBarButtons.Budget)
                .Set(nameof(topBarButtons.Budget), "TopBarButton")
                .AssignBooleanButton(monoBootstrap.TopBar.Budget, false)
                .SetParent(topBarRoot);

            new VisualElement()
                .SetVariable(out topBarButtons.Technology)
                .Set(nameof(topBarButtons.Technology), "TopBarButton")
                .AssignBooleanButton(monoBootstrap.TopBar.Technology, false)
                .SetParent(topBarRoot);

            new VisualElement()
                .SetVariable(out topBarButtons.Politics)
                .Set(nameof(topBarButtons.Politics), "TopBarButton")
                .AssignBooleanButton(monoBootstrap.TopBar.Politics, false)
                .SetParent(topBarRoot);

            new VisualElement()
                .SetVariable(out topBarButtons.Population)
                .Set(nameof(topBarButtons.Population), "TopBarButton")
                .AssignBooleanButton(monoBootstrap.TopBar.Population, false)
                .SetParent(topBarRoot);

            new VisualElement()
                .SetVariable(out topBarButtons.Trade)
                .Set(nameof(topBarButtons.Trade), "TopBarButton")
                .AssignBooleanButton(monoBootstrap.TopBar.Trade, false)
                .SetParent(topBarRoot);

            new VisualElement()
                .SetVariable(out topBarButtons.Diplomacy)
                .Set(nameof(topBarButtons.Diplomacy), "TopBarButton")
                .AssignBooleanButton(monoBootstrap.TopBar.Diplomacy, false)
                .SetParent(topBarRoot);

            new VisualElement()
                .SetVariable(out topBarButtons.Military)
                .Set(nameof(topBarButtons.Military), "TopBarButton")
                .AssignBooleanButton(monoBootstrap.TopBar.Military, false)
                .SetParent(topBarRoot);
        }
    }

    public struct TopBarButtonElements
    {
        public VisualElement Production, Budget, Technology, Politics, Population, Trade, Diplomacy, Military;
    }
}