using UnityEngine;

namespace Eos.Ux.Lean
{
    public class LsLeanCameraManager : MonoBehaviour
    {
        public enum CameraMode
        {
            Dolly = 0,
            Zoom = 1
        }

        [SerializeField] private LsLeanCameraLocate _lsLeanCameraLocate;
        [SerializeField] private LsLeanCameraRotate _lsLeanCameraRotate;
        [SerializeField] private LsLeanCameraDolly _lsLeanCameraDolly;
        [SerializeField] private LsLeanCameraZoom _lsLeanCameraZoom;
        [SerializeField] private Camera _camera;
        [SerializeField] private LsLeanPointOfView _lsLeanPointOfViewOnStart;

        private Vector3 _lookLocate = new Vector3(0f, 0f, 0f);
        private float _lookDistance = 90f;
        private Vector3 _lookEulerAngle = new Vector3(15f, -30f, 0f);
        private float _lookZoom = 50f;

        private LeanCameraLocateSettings _storedLocateSettings = new LeanCameraLocateSettings();
        private LeanCameraRotateSettings _storedRotateSettings = new LeanCameraRotateSettings();
        private LeanCameraDollySettings _storedDollySettings = new LeanCameraDollySettings();
        private LeanCameraZoomSettings _storedZoomSettings = new LeanCameraZoomSettings();

        public LsLeanCameraLocate LeanCameraLocate { get => _lsLeanCameraLocate; set => _lsLeanCameraLocate = value; }
        public LsLeanCameraRotate LeanCameraRotate { get => _lsLeanCameraRotate; set => _lsLeanCameraRotate = value; }
        public LsLeanCameraDolly LeanCameraDolly { get => _lsLeanCameraDolly; set => _lsLeanCameraDolly = value; }
        public LsLeanCameraZoom LeanCameraZoom { get => _lsLeanCameraZoom; set => _lsLeanCameraZoom = value; }
        public Camera LeanCamera { get => _camera; set => _camera = value; }

        #region ContextMenu
        [ContextMenu("Copy properties as Settings to JSON")]
        public void CopyPropertiesAsSettingsToJson()
        {
            var povSettings = new LsLeanPointOfView.PovSettings
            {
                _lookLocate = new Vector3(_lsLeanCameraLocate._x, _lsLeanCameraLocate._y, _lsLeanCameraLocate._z),
                _lookEulerAngle = new Vector3(_lsLeanCameraRotate._x, _lsLeanCameraRotate._y, _lsLeanCameraRotate._z),
                _lookDistance = -_lsLeanCameraDolly.transform.localPosition.z,
                _lookZoom = _camera.fieldOfView
            };

            var json = JsonUtility.ToJson(povSettings);
            GUIUtility.systemCopyBuffer = json;
        }

        [ContextMenu("Paste & Parse Settings From JSON")]
        public void PastePovSettingsFromJson()
        {
            var json = GUIUtility.systemCopyBuffer;

            try
            {
                var povSettings = JsonUtility.FromJson<LsLeanPointOfView.PovSettings>(json);

                _lsLeanCameraLocate._x = povSettings._lookLocate.x;
                _lsLeanCameraLocate._y = povSettings._lookLocate.y;
                _lsLeanCameraLocate._z = povSettings._lookLocate.z;
                _lsLeanCameraRotate._x = povSettings._lookEulerAngle.x;
                _lsLeanCameraRotate._y = povSettings._lookEulerAngle.y;
                _lsLeanCameraRotate._z = povSettings._lookEulerAngle.z;
                _lsLeanCameraDolly._dolly = povSettings._lookDistance;
                _lsLeanCameraDolly.transform.localPosition = new Vector3(
                    _lsLeanCameraDolly.transform.localPosition.x,
                    _lsLeanCameraDolly.transform.localPosition.y,
                    -_lookDistance
                );
                _lsLeanCameraZoom._zoom = povSettings._lookZoom;
                _camera.fieldOfView = povSettings._lookZoom;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to parse settings from JSON: " + ex.Message);
            }
        }
        #endregion

        public void SwitchCameraMode(int mode)
        {
            _lsLeanCameraDolly.enabled = false;
            _lsLeanCameraZoom.enabled = false;
            switch (mode)
            {
                case (int)CameraMode.Dolly:
                    _lsLeanCameraDolly.enabled = true;
                    break;
                case (int)CameraMode.Zoom:
                    _lsLeanCameraZoom.enabled = true;
                    break;
                default:
                    Debug.LogWarning("Unknown camera mode: " + mode + "\nCamera mode code must be either 0 (Zoom) or 1 (Dolly).");
                    break;
            }
        }

        public void SyncPointOfView(LsLeanPointOfView pov)
        {
            SetCameraProperties(
                new Vector3(pov.Settings._lookLocate.x, pov.Settings._lookLocate.y, pov.Settings._lookLocate.z),
                new Vector3(pov.Settings._lookEulerAngle.x, pov.Settings._lookEulerAngle.y, pov.Settings._lookEulerAngle.z),
                pov.Settings._lookDistance,
                pov.Settings._lookZoom
            );
        }

        public void GetCameraProperties(out Vector3 locate, out Vector3 rotate, out float dolly, out float zoom)
        {
            locate = new Vector3(_lsLeanCameraLocate._x, _lsLeanCameraLocate._y, _lsLeanCameraLocate._z);
            rotate = new Vector3(_lsLeanCameraRotate._x, _lsLeanCameraRotate._y, _lsLeanCameraRotate._z);
            dolly = _lsLeanCameraDolly._dolly;
            zoom = _lsLeanCameraZoom._zoom;
        }

        public void SetCameraProperties(Vector3 locate, Vector3 rotate, float dolly = 100f, float zoom = 3f)
        {
            if (_lsLeanCameraLocate)
            {
                _lsLeanCameraLocate._x = locate.x;
                _lsLeanCameraLocate._y = locate.y;
                _lsLeanCameraLocate._z = locate.z;
            }
            if (_lsLeanCameraRotate)
            {
                _lsLeanCameraRotate._x = rotate.x;
                _lsLeanCameraRotate._y = rotate.y;
                _lsLeanCameraRotate._z = rotate.z;
            }
            if (_lsLeanCameraDolly)
            {
                _lsLeanCameraDolly._dolly = dolly;
                _lsLeanCameraDolly.transform.localPosition = new Vector3(
                    _lsLeanCameraDolly.transform.localPosition.x,
                    _lsLeanCameraDolly.transform.localPosition.y,
                    -dolly
                );
            }
            if (_lsLeanCameraZoom)
            {
                _lsLeanCameraZoom._zoom = zoom;
                _camera.fieldOfView = zoom;
            }
        }

        public void LookAtTarget(LsLeanPointOfView pov)
        {
            _lookLocate = pov.Settings._lookLocate;
            _lookDistance = pov.Settings._lookDistance;
            _lookEulerAngle = pov.Settings._lookEulerAngle;
            _lookZoom = pov.Settings._lookZoom;
            SetLookAtMode(true);
        }

        public void LookAtTarget(Transform target, float distance, Vector3 eulerAngle, float zoom)
        {
            _lookLocate = target.position;
            _lookDistance = distance;
            _lookEulerAngle = eulerAngle;
            _lookZoom = zoom;
            SetLookAtMode(true);
        }

        public void ExitLookAtMode()
        {
            SetLookAtMode(false);
        }

        public void SetFreeMode(bool active)
        {
            if (active)
            {
                // Store values to settings
                _storedLocateSettings._xyzValue = new Vector3(_lsLeanCameraLocate._x, _lsLeanCameraLocate._y, _lsLeanCameraLocate._z);
                _lsLeanCameraLocate.DeactivateSmooth();
                _lsLeanCameraLocate._x = _lsLeanCameraZoom.transform.position.x;
                _lsLeanCameraLocate._y = _lsLeanCameraZoom.transform.position.y;
                _lsLeanCameraLocate._z = _lsLeanCameraZoom.transform.position.z;
                _lsLeanCameraLocate.Tick();
                _lsLeanCameraLocate.ActivateSmooth();

                _storedRotateSettings._xyzValue = new Vector3(_lsLeanCameraRotate._x, _lsLeanCameraRotate._y, _lsLeanCameraRotate._z);

                _storedDollySettings._dollyValue = _lsLeanCameraDolly._dolly;
                _lsLeanCameraDolly._dolly = 1f;
                _lsLeanCameraDolly.transform.localPosition = new Vector3(
                    _lsLeanCameraDolly.transform.localPosition.x,
                    _lsLeanCameraDolly.transform.localPosition.y,
                    1f
                );
                _storedZoomSettings._zoomValue = _lsLeanCameraZoom._zoom;

                SwitchCameraMode((int)CameraMode.Dolly);
            }
            else
            {
                // Restore values from settings
                _lsLeanCameraLocate._x = _storedLocateSettings._xyzValue.x;
                _lsLeanCameraLocate._y = _storedLocateSettings._xyzValue.y;
                _lsLeanCameraLocate._z = _storedLocateSettings._xyzValue.z;
                _lsLeanCameraRotate._x = _storedRotateSettings._xyzValue.x;
                _lsLeanCameraRotate._y = _storedRotateSettings._xyzValue.y;
                _lsLeanCameraRotate._z = _storedRotateSettings._xyzValue.z;
                _lsLeanCameraDolly._dolly = _storedDollySettings._dollyValue;
                _lsLeanCameraDolly.transform.localPosition = new Vector3(
                    _lsLeanCameraDolly.transform.localPosition.x,
                    _lsLeanCameraDolly.transform.localPosition.y,
                    -_storedDollySettings._dollyValue
                );
                _lsLeanCameraZoom._zoom = _storedZoomSettings._zoomValue;
                SwitchCameraMode((int)CameraMode.Zoom);
            }
        }

        private void Start()
        {
            if (_lsLeanPointOfViewOnStart)
            {
                LookAtTarget(_lsLeanPointOfViewOnStart);
            }
        }

        private void SetLookAtMode(bool active)
        {
            if (active)
            {
                SetFreeMode(true);
                _lsLeanCameraLocate._x = _lookLocate.x;
                _lsLeanCameraLocate._y = _lookLocate.y;
                _lsLeanCameraLocate._z = _lookLocate.z;
                _lsLeanCameraRotate._x = _lookEulerAngle.x;
                _lsLeanCameraRotate._y = _lookEulerAngle.y;
                _lsLeanCameraRotate._z = _lookEulerAngle.z;
                _lsLeanCameraDolly._dolly = _lookDistance;
                _lsLeanCameraDolly.transform.localPosition = new Vector3(
                    _lsLeanCameraDolly.transform.localPosition.x,
                    _lsLeanCameraDolly.transform.localPosition.y,
                    -_lookDistance
                );
                _lsLeanCameraZoom._zoom = _lookZoom;
                _camera.fieldOfView = _lookZoom;
            }
            else
            {
                SetFreeMode(false);
            }
        }
    }

    #region Settings
    [System.Serializable]
    public class LeanCameraLocateSettings
    {
        public Vector3 _xyzValue;
    }

    [System.Serializable]
    public class LeanCameraRotateSettings
    {
        public Vector3 _xyzValue;
    }

    [System.Serializable]
    public class LeanCameraDollySettings
    {
        public float _dollyValue;
    }

    [System.Serializable]
    public class LeanCameraZoomSettings
    {
        public float _zoomValue;
    }
    #endregion
}
