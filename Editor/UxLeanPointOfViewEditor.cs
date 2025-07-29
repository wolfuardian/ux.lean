#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Eos.Ux.Lean
{
    [CustomEditor(typeof(UxLeanPointOfView))]
    [CanEditMultipleObjects]
    public class UxLeanPointOfViewEditor : UnityEditor.Editor
    {
        private UxLeanPointOfView _leanPointOfView;
        private Color _originalColor;

        public override void OnInspectorGUI()
        {
            _leanPointOfView = (UxLeanPointOfView)target;

            _originalColor = GUI.backgroundColor;

            if (_leanPointOfView.IsLinkModeEnabled)
            {
                GUI.backgroundColor = new Color(1f, 0.7f, 0.7f, 1f); // Light red color for link mode
            }

            DrawDefaultInspector();

            DrawLinkModePanel(this);

            if (GUILayout.Button("Copy Properties As Settings To Json"))
            {
                _leanPointOfView.CopyPropertiesAsSettingsToJson();
            }

            if (GUILayout.Button("Parse Settings From Json"))
            {
                _leanPointOfView.PastePovSettingsFromJson();
            }
        }

        private void DrawLinkModePanel(UxLeanPointOfViewEditor self)
        {
            if (self._leanPointOfView.LeanCameraManager == null)
            {
                GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Light gray color for disabled button
                EditorGUI.BeginDisabledGroup(true);
                if (GUILayout.Button("Link Mode: No Camera Manager Assigned"))
                {
                    // Do nothing, just a message
                }
                EditorGUI.EndDisabledGroup();
                GUI.backgroundColor = self._originalColor;
                return;
            }

            if (self._leanPointOfView.IsLinkModeEnabled)
            {
                GUI.backgroundColor = new Color(0.7f, 1f, 0.7f, 1f); // Light green color for disable link mode button
                if (GUILayout.Button("Complete Link Mode"))
                {
                    _leanPointOfView.IsLinkModeEnabled = false;
                }
                GUI.backgroundColor = self._originalColor;
            }
            else
            {
                GUI.backgroundColor = new Color(1f, 0.7f, 0.7f, 1f); // Light red color for enable link mode button
                if (GUILayout.Button("Enter Link Mode"))
                {
                    _leanPointOfView.IsLinkModeEnabled = true;
                }
                GUI.backgroundColor = self._originalColor;
            }
        }
    }
}
#endif
