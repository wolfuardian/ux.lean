#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Eos.Ux.Lean
{
    [CustomEditor(typeof(UxLeanCameraManager))]
    [CanEditMultipleObjects]
    public class UxLeanCameraManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var leanCameraManager = (UxLeanCameraManager)target;

            DrawDefaultInspector();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Copy Properties As Settings To Json"))
            {
                leanCameraManager.CopyPropertiesAsSettingsToJson();
            }

            if (GUILayout.Button("Parse Settings From Json"))
            {
                leanCameraManager.PastePovSettingsFromJson();
            }
        }
    }
}
#endif
