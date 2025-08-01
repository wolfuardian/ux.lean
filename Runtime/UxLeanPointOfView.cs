using System.Linq;
using UnityEngine;

namespace Eos.Ux.Lean
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class UxLeanPointOfView : MonoBehaviour
    {
        [SerializeField] private string _bookmark = "View Description";
        [SerializeField] private PovSettings _settings;

        [System.NonSerialized]
        public bool IsLinkModeEnabled = false;

        public string bookmark
        {
            get => _bookmark;
            set => _bookmark = value;
        }

        private UxLeanCameraManager _leanCameraManager;
        #if UNITY_2022_1_OR_NEWER
        private UxLeanCameraManager cachedLeanCameraManager => _leanCameraManager ??= FindAnyObjectByType<UxLeanCameraManager>();
        #else
        private UxLeanCameraManager cachedLeanCameraManager => _leanCameraManager ??= FindObjectOfType<UxLeanCameraManager>();
        #endif

        public PovSettings settings => _settings;

        public static bool BookmarkExists(string baseBookmark)
        {
            #if UNITY_2022_1_OR_NEWER
            var povs = FindObjectsByType<UxLeanPointOfView>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            #else
            var povs = FindObjectsOfType<UxLeanPointOfView>();
            #endif
            return povs.Any(pov => pov.bookmark == baseBookmark);
        }

        #region ContextMenu
        [ContextMenu("Copy Settings to JSON")]
        public void CopyPropertiesAsSettingsToJson()
        {
            var povSettings = new PovSettings
            {
                _bookmark = _bookmark,
                _lookLocate = _settings._lookLocate,
                _lookEulerAngle = _settings._lookEulerAngle,
                _lookDistance = _settings._lookDistance,
                _lookZoom = _settings._lookZoom
            };

            var json = JsonUtility.ToJson(povSettings);
            GUIUtility.systemCopyBuffer = json;
        }

        [ContextMenu("Parse Settings From JSON")]
        public void PastePovSettingsFromJson()
        {
            var json = GUIUtility.systemCopyBuffer;

            try
            {
                var povSettings = JsonUtility.FromJson<PovSettings>(json);
                _settings._bookmark = povSettings._bookmark;
                _settings._lookLocate = new Vector3(povSettings._lookLocate.x, povSettings._lookLocate.y, povSettings._lookLocate.z);
                _settings._lookEulerAngle = new Vector3(povSettings._lookEulerAngle.x, povSettings._lookEulerAngle.y, povSettings._lookEulerAngle.z);
                _settings._lookDistance = povSettings._lookDistance;
                _settings._lookZoom = povSettings._lookZoom;
                ApplySettings();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to parse settings from JSON: " + ex.Message);
            }
        }
        #endregion

        private void Update()
        {
            SaveSettings();
            FormatEulerAnglesToLeanTouch();
        }

        private void LateUpdate()
        {
            if (IsLinkModeEnabled)
            {
                cachedLeanCameraManager.SyncPointOfView(this);
            }
        }

        private void ApplySettings()
        {
            _bookmark = _settings._bookmark;
            transform.position = _settings._lookLocate;
            transform.eulerAngles = _settings._lookEulerAngle;
            transform.localScale = Vector3.one * _settings._lookDistance;
        }

        private void SaveSettings()
        {
            _settings._bookmark = _bookmark;
            _settings._lookLocate = transform.position;
            _settings._lookEulerAngle = transform.eulerAngles;
            _settings._lookDistance = transform.localScale.x;
        }

        private void FormatEulerAnglesToLeanTouch()
        {
            var forward  = transform.rotation * Vector3.forward;
            var clampedY = Mathf.Clamp(forward.y, -1f, 1f);
            var yaw      = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            var pitch    = -Mathf.Asin(clampedY) * Mathf.Rad2Deg;
            _settings._lookEulerAngle = new Vector3(pitch, yaw, 0f);
        }

        #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            UnityEditor.Handles.Label(transform.position - transform.forward * _settings._lookDistance + Vector3.up * _settings._lookDistance * 0.1f, _bookmark);
            Gizmos.color = Color.red;
            Gizmos.color = UnityEditor.Selection.activeGameObject == gameObject ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 0f, 0f, 1f);
            Gizmos.DrawLine(transform.position, transform.position - transform.forward * _settings._lookDistance);
            Gizmos.DrawSphere(transform.position - transform.forward * _settings._lookDistance, _settings._lookDistance * 0.05f);
            Gizmos.color = UnityEditor.Selection.activeGameObject == gameObject ? new Color(1f, 1f, 1f, 0f) : new Color(0f, 0f, 0f, 0f);
            Gizmos.DrawSphere(transform.position, _settings._lookDistance);
        }
        #endif

        #region Settings
        [System.Serializable]
        public class PovSettings
        {
            public string _bookmark = "View Description";
            public Vector3 _lookLocate;
            public Vector3 _lookEulerAngle;
            public float _lookDistance;
            [Range(0.0001f, 179f)]
            public float _lookZoom = 45f;
        }
        #endregion
    }
}
