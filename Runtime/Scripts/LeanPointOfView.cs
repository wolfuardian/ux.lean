using UnityEngine;

namespace Eos.Ux.Lean
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class LeanPointOfView : MonoBehaviour
    {
        [SerializeField] private string _bookmark = "View Description";
        [SerializeField] private POVSettings _settings;

        internal bool onLoadSettingsFirstMode = false;

        public POVSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public void LookAt()
        {
            var cameraManager = FindObjectOfType<LeanCameraManager>();
            if (cameraManager == null)
            {
                Debug.LogError("LeanCameraManagerV3 not found in the scene.");
                return;
            }
            cameraManager.LookAtTarget(this);
        }

        [ContextMenu("Copy Settings to JSON")]
        internal void CopyPropertiesAsSettingsToJson()
        {
            var povSettings = new POVSettings
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
        internal void PastePOVSettingsFromJson()
        {
            var json = GUIUtility.systemCopyBuffer;

            try
            {
                var povSettings = JsonUtility.FromJson<POVSettings>(json);
                _settings._bookmark = povSettings._bookmark;
                _settings._lookLocate = new Vector3(povSettings._lookLocate.x, povSettings._lookLocate.y, povSettings._lookLocate.z);
                _settings._lookEulerAngle = new Vector3(povSettings._lookEulerAngle.x, povSettings._lookEulerAngle.y, povSettings._lookEulerAngle.z);
                _settings._lookDistance = povSettings._lookDistance;
                _settings._lookZoom = povSettings._lookZoom;
                LoadSettings();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to parse settings from JSON: " + ex.Message);
            }
        }

        private void OnValidate()
        {
            if (onLoadSettingsFirstMode)
            {
                LoadSettings();
            }
        }

        private void Update()
        {
            if (!onLoadSettingsFirstMode)
            {
                SaveSettings();
            }

            FormatEulerAnglesToLeanTouch();
        }

        [ContextMenu("More Options / Toggle Load Settings First Mode")]
        private void ToggleLoadSettingsFirstMode()
        {
            onLoadSettingsFirstMode = !onLoadSettingsFirstMode;
        }

        private void LoadSettings()
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

        // This method formats the Euler angles to match the LeanTouch expected format
        private void FormatEulerAnglesToLeanTouch()
        {
            var forward = transform.rotation * Vector3.forward;
            var yaw     = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            var pitch   = -Mathf.Asin(forward.y) * Mathf.Rad2Deg;
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

        [System.Serializable]
        public class POVSettings
        {
            public string _bookmark = "View Description";
            public Vector3 _lookLocate;
            public Vector3 _lookEulerAngle;
            public float _lookDistance;
            [Range(0.0001f, 179f)]
            public float _lookZoom;
        }
    }
}
