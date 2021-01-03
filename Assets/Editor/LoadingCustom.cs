using EditorFunctions.Loading;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PlaceOnEmptyObject))]
    public class LoadingCustom : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myScript = (PlaceOnEmptyObject) target;
            if (GUILayout.Button("Generate Cache Files"))
                myScript.ConvertToCache();

            if (GUILayout.Button("Process Texture"))
                myScript.ConvertToIdTexture();

            if (GUILayout.Button("Generate Debug Area Color Map"))
                myScript.DebugGenerateColorMapBasedOffOfAreas();
        }
    }
}