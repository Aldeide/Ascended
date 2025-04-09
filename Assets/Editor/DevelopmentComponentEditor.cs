using Systems.Development;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(DevelopmentComponent))]
    public class DevelopmentComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Add health"))
            {
                target.GetComponent<DevelopmentComponent>().AddHealth();
            }
            if (GUILayout.Button("Add Effect"))
            {
                target.GetComponent<DevelopmentComponent>().AddEffect();
            }
        }
    }
}