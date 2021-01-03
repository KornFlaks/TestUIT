using EditorFunctions.InsertTexIntoMip;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(InsertMono))]
    public class InsertCustom : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myScript = (InsertMono) target;
            if (GUILayout.Button("Override Unity's Shitty Mip Maps"))
                myScript.Combine();
        }
    }
}