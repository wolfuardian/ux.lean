#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Eos.Ux.Lean
{
    [CustomEditor(typeof(LsLeanCameraManager))]
    [CanEditMultipleObjects]
    public class LsLeanCameraManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var leanCameraManager = (LsLeanCameraManager)target;

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
