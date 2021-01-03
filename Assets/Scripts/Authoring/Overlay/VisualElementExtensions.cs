using UnityEngine;
using UnityEngine.UIElements;

namespace Authoring.Overlay
{
    public static class VisualElementExtensions
    {
        public static T Set<T>(this T v,
            string name = null,
            string _class = null,
            FlexDirection? flexDirection = null,
            Justify? justifyContent = null,
            Align? alignItems = null,
            Sprite backgroundSprite = null,
            float? flexGrow = null,
            float? maxHeight = null,
            float? maxWidth = null,
            float? height = null,
            float? width = null,
            Vector2? transform = null,
            Color? color = null,
            ScaleMode? unityBackgroundScaleMode = null,
            DisplayStyle? display = null) where T : VisualElement
        {
            if (name != null)
                v.name = name;
            if (_class != null)
                v.AddToClassList(_class);
            if (flexDirection.HasValue)
                v.style.flexDirection = new StyleEnum<FlexDirection>(flexDirection.Value);
            if (alignItems.HasValue)
                v.style.alignItems = new StyleEnum<Align>(alignItems.Value);
            if (flexGrow.HasValue)
                v.style.flexGrow = new StyleFloat(flexGrow.Value);
            if (backgroundSprite != null)
                v.style.backgroundImage = Background.FromSprite(backgroundSprite);
            if (maxHeight.HasValue)
                v.style.maxHeight = maxHeight.Value;
            if (maxWidth.HasValue)
                v.style.maxWidth = maxWidth.Value;
            if (height.HasValue)
                v.style.height = height.Value;
            if (width.HasValue)
                v.style.width = width.Value;
            if (justifyContent.HasValue)
                v.style.justifyContent = new StyleEnum<Justify>(justifyContent.Value);
            if (color.HasValue)
                v.style.color = new StyleColor(color.Value);
            if (unityBackgroundScaleMode.HasValue)
                v.style.unityBackgroundScaleMode = new StyleEnum<ScaleMode>(unityBackgroundScaleMode.Value);
            if (display.HasValue)
                v.style.display = display.Value;
            if (transform.HasValue)
                v.transform.position = transform.Value;
            return v;
        }

        public static T SetText<T>(this T v, string text) where T : TextElement
        {
            v.text = text;
            return v;
        }

        public static T AssignBooleanButton<T>(this T v, BooleanButtons button, bool sizeToSprite = true) where T : VisualElement
        {
            return v.Set(transform: button.Position).SetSprite(button.On, sizeToSprite);
        }

        public static T SetSprite<T>(this T v, Sprite background, bool sizeToSprite = true)
            where T : VisualElement
        {
            v.style.backgroundImage = Background.FromSprite(background);

            if (sizeToSprite)
            {
                var size = background.rect.size;
                v.style.width = size.x;
                v.style.height = size.y;
            }

            return v;
        }

        public static T SetVariable<T>(this T v, out T reference) where T : VisualElement
        {
            reference = v;
            return v;
        }

        public static T SetParent<T>(this T v, T reference) where T : VisualElement
        {
            reference.Add(v);
            return v;
        }

        public static VisualElement AddRange(this VisualElement v, params VisualElement[] elements)
        {
            foreach (var el in elements)
                v.Add(el);
            return v;
        }
    }
}