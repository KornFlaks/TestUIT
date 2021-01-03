using System.Runtime.InteropServices;
using UnityEngine.UIElements;

namespace Authoring.Overlay.TopBarButtons
{
    [StructLayout(LayoutKind.Auto)]
    public struct ProductionVariables
    {
        // True = activated (clicked). False = not.
        public bool ButtonState, BuildingState, FactoryState, UnemploymentState;
        public VisualElement Production, Building, Factory, Unemployment;
    }

    public static class Production
    {
        public static void Create(VisualElement background, ProdButtons prodButtons,
            out ProductionVariables productionVariables)
        {
            new Label()
                .Set(_class: "TopBarButtonHeaderText")
                .SetText("Production")
                .SetParent(background);
            
            // Custom shaders not supported by UI toolkit :(
            // Will need to add in additive transparency one day. Or fix the sprites.

            new VisualElement()
                .SetVariable(out var building)
                .Set(nameof(building))
                .AssignBooleanButton(prodButtons.Building)
                .SetParent(background);

            new VisualElement()
                .SetVariable(out var factory)
                .Set(nameof(factory))
                .AssignBooleanButton(prodButtons.Factory)
                .SetParent(background);

            new VisualElement()
                .SetVariable(out var unemployment)
                .Set(nameof(unemployment))
                .AssignBooleanButton(prodButtons.Unemployment)
                .SetParent(background);

            productionVariables = new ProductionVariables
            {
                ButtonState = false,
                Production = background,
                Building = building,
                Factory = factory,
                Unemployment = unemployment
            };
        }
    }
}