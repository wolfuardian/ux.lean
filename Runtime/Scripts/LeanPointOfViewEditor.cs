#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Eos.Ux.Lean
{
    [CustomEditor(typeof(LeanPointOfView))]
    [CanEditMultipleObjects]
    public class LeanPointOfViewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var leanPointOfView = (LeanPointOfView)target;

            EditorGUI.BeginChangeCheck();

            // button to reset the POV settings to default

            if (leanPointOfView.onLoadSettingsFirstMode)
            {
                var originalColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1f, 1f, 0.7f, 1f);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawDefaultInspector();
                EditorGUILayout.EndVertical();

                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);

                EditorGUILayout.EndVertical();

                EditorGUILayout.HelpBox(
                    "Load Settings First Mode is enabled.\n" +
                    "In this mode, changes in Settings will override the corresponding parameters in the scene.\n" +
                    "Also, parameter changes made through scene actions (e.g., move, rotate, scale) cannot be undone.",
                    MessageType.Warning
                );
                GUI.backgroundColor = originalColor;

                if (EditorGUI.EndChangeCheck() && leanPointOfView.onLoadSettingsFirstMode)
                {
                    EditorUtility.SetDirty(target);
                }
            }
            else
            {
                DrawDefaultInspector();
                GUILayout.Space(10);
                if (leanPointOfView.Settings._lookZoom <= 10f)
                {
                    EditorGUILayout.HelpBox(
                        "Zoom level is too low. It may cause issues with camera rendering.\n" +
                        "Consider increasing the zoom level to a minimum of 20.0.",
                        MessageType.Warning
                    );
                }
                if (leanPointOfView.Settings._lookZoom >= 90f)
                {
                    EditorGUILayout.HelpBox(
                        "Zoom level is too high. It may cause issues with camera rendering.\n" +
                        "Consider decreasing the zoom level to a maximum of 80.0.",
                        MessageType.Warning
                    );
                }
            }

            if (GUILayout.Button("Copy Properties As Settings To Json"))
            {
                leanPointOfView.CopyPropertiesAsSettingsToJson();
            }

            if (GUILayout.Button("Parse Settings From Json"))
            {
                leanPointOfView.PastePOVSettingsFromJson();
            }
        }
    }
}
#endif
