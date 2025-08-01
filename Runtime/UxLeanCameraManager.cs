using UnityEngine;

namespace Eos.Ux.Lean
{
    [AddComponentMenu("UX Lean/UX Lean Camera Manager")]
    public class UxLeanCameraManager : MonoBehaviour
    {
        public enum CameraMode
        {
            Dolly = 0,
            Zoom = 1
        }

        [SerializeField] private UxLeanCameraLocate _uxLeanCameraLocate;
        [SerializeField] private UxLeanCameraRotate _uxLeanCameraRotate;
        [SerializeField] private UxLeanCameraDolly _uxLeanCameraDolly;
        [SerializeField] private UxLeanCameraZoom _uxLeanCameraZoom;
        [SerializeField] private Camera _camera;
        [SerializeField] private UxLeanPointOfView _uxLeanPointOfViewOnStart;

        private Vector3 _lookLocate = new Vector3(0f, 0f, 0f);
        private float _lookDistance = 90f;
        private Vector3 _lookEulerAngle = new Vector3(15f, -30f, 0f);
        private float _lookZoom = 50f;

        private readonly LeanCameraLocateSettings _storedLocateSettings = new LeanCameraLocateSettings();
        private readonly LeanCameraRotateSettings _storedRotateSettings = new LeanCameraRotateSettings();
        private readonly LeanCameraDollySettings _storedDollySettings = new LeanCameraDollySettings();
        private readonly LeanCameraZoomSettings _storedZoomSettings = new LeanCameraZoomSettings();

        public UxLeanCameraLocate leanCameraLocate { get => _uxLeanCameraLocate; set => _uxLeanCameraLocate = value; }
        public UxLeanCameraRotate leanCameraRotate { get => _uxLeanCameraRotate; set => _uxLeanCameraRotate = value; }
        public UxLeanCameraDolly leanCameraDolly { get => _uxLeanCameraDolly; set => _uxLeanCameraDolly = value; }
        public UxLeanCameraZoom leanCameraZoom { get => _uxLeanCameraZoom; set => _uxLeanCameraZoom = value; }
        public Camera leanCamera { get => _camera; set => _camera = value; }

        #region ContextMenu
        [ContextMenu("Copy properties as Settings to JSON")]
        public void CopyPropertiesAsSettingsToJson()
        {
            var povSettings = new UxLeanPointOfView.PovSettings
            {
                _lookLocate = new Vector3(_uxLeanCameraLocate._x, _uxLeanCameraLocate._y, _uxLeanCameraLocate._z),
                _lookEulerAngle = new Vector3(_uxLeanCameraRotate._x, _uxLeanCameraRotate._y, _uxLeanCameraRotate._z),
                _lookDistance = -_uxLeanCameraDolly.transform.localPosition.z,
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
                var povSettings = JsonUtility.FromJson<UxLeanPointOfView.PovSettings>(json);

                _uxLeanCameraLocate._x = povSettings._lookLocate.x;
                _uxLeanCameraLocate._y = povSettings._lookLocate.y;
                _uxLeanCameraLocate._z = povSettings._lookLocate.z;
                _uxLeanCameraRotate._x = povSettings._lookEulerAngle.x;
                _uxLeanCameraRotate._y = povSettings._lookEulerAngle.y;
                _uxLeanCameraRotate._z = povSettings._lookEulerAngle.z;
                _uxLeanCameraDolly._dolly = povSettings._lookDistance;
                _uxLeanCameraDolly.transform.localPosition = new Vector3(
                    _uxLeanCameraDolly.transform.localPosition.x,
                    _uxLeanCameraDolly.transform.localPosition.y,
                    -_lookDistance
                );
                _uxLeanCameraZoom._zoom = povSettings._lookZoom;
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
            _uxLeanCameraDolly.enabled = false;
            _uxLeanCameraZoom.enabled = false;
            switch (mode)
            {
                case (int)CameraMode.Dolly:
                    _uxLeanCameraDolly.enabled = true;
                    break;
                case (int)CameraMode.Zoom:
                    _uxLeanCameraZoom.enabled = true;
                    break;
                default:
                    Debug.LogWarning("Unknown camera mode: " + mode + "\nCamera mode code must be either 0 (Zoom) or 1 (Dolly).");
                    break;
            }
        }

        public void SyncPointOfView(UxLeanPointOfView pov)
        {
            SetCameraProperties(
                new Vector3(pov.settings._lookLocate.x, pov.settings._lookLocate.y, pov.settings._lookLocate.z),
                new Vector3(pov.settings._lookEulerAngle.x, pov.settings._lookEulerAngle.y, pov.settings._lookEulerAngle.z),
                pov.settings._lookDistance,
                pov.settings._lookZoom
            );
        }

        public void GetCameraProperties(out Vector3 locate, out Vector3 rotate, out float dolly, out float zoom)
        {
            locate = new Vector3(_uxLeanCameraLocate._x, _uxLeanCameraLocate._y, _uxLeanCameraLocate._z);
            rotate = new Vector3(_uxLeanCameraRotate._x, _uxLeanCameraRotate._y, _uxLeanCameraRotate._z);
            dolly = _uxLeanCameraDolly._dolly;
            zoom = _uxLeanCameraZoom._zoom;
        }

        public void SetCameraProperties(Vector3 locate, Vector3 rotate, float dolly = 100f, float zoom = 3f)
        {
            if (_uxLeanCameraLocate)
            {
                _uxLeanCameraLocate._x = locate.x;
                _uxLeanCameraLocate._y = locate.y;
                _uxLeanCameraLocate._z = locate.z;
            }
            if (_uxLeanCameraRotate)
            {
                _uxLeanCameraRotate._x = rotate.x;
                _uxLeanCameraRotate._y = rotate.y;
                _uxLeanCameraRotate._z = rotate.z;
            }
            if (_uxLeanCameraDolly)
            {
                _uxLeanCameraDolly._dolly = dolly;
                _uxLeanCameraDolly.transform.localPosition = new Vector3(
                    _uxLeanCameraDolly.transform.localPosition.x,
                    _uxLeanCameraDolly.transform.localPosition.y,
                    -dolly
                );
            }
            if (_uxLeanCameraZoom)
            {
                _uxLeanCameraZoom._zoom = zoom;
                _camera.fieldOfView = zoom;
            }
        }

        public void LookAtTarget(UxLeanPointOfView pov)
        {
            _lookLocate = pov.settings._lookLocate;
            _lookDistance = pov.settings._lookDistance;
            _lookEulerAngle = pov.settings._lookEulerAngle;
            _lookZoom = pov.settings._lookZoom;
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
                _storedLocateSettings._xyzValue = new Vector3(_uxLeanCameraLocate._x, _uxLeanCameraLocate._y, _uxLeanCameraLocate._z);
                _uxLeanCameraLocate.DeactivateSmooth();
                _uxLeanCameraLocate._x = _uxLeanCameraZoom.transform.position.x;
                _uxLeanCameraLocate._y = _uxLeanCameraZoom.transform.position.y;
                _uxLeanCameraLocate._z = _uxLeanCameraZoom.transform.position.z;
                _uxLeanCameraLocate.Tick();
                _uxLeanCameraLocate.ActivateSmooth();

                _storedRotateSettings._xyzValue = new Vector3(_uxLeanCameraRotate._x, _uxLeanCameraRotate._y, _uxLeanCameraRotate._z);

                _storedDollySettings._dollyValue = _uxLeanCameraDolly._dolly;
                _uxLeanCameraDolly._dolly = 1f;
                _uxLeanCameraDolly.transform.localPosition = new Vector3(
                    _uxLeanCameraDolly.transform.localPosition.x,
                    _uxLeanCameraDolly.transform.localPosition.y,
                    1f
                );
                _storedZoomSettings._zoomValue = _uxLeanCameraZoom._zoom;

                SwitchCameraMode((int)CameraMode.Dolly);
            }
            else
            {
                _uxLeanCameraLocate._x = _storedLocateSettings._xyzValue.x;
                _uxLeanCameraLocate._y = _storedLocateSettings._xyzValue.y;
                _uxLeanCameraLocate._z = _storedLocateSettings._xyzValue.z;
                _uxLeanCameraRotate._x = _storedRotateSettings._xyzValue.x;
                _uxLeanCameraRotate._y = _storedRotateSettings._xyzValue.y;
                _uxLeanCameraRotate._z = _storedRotateSettings._xyzValue.z;
                _uxLeanCameraDolly._dolly = _storedDollySettings._dollyValue;
                _uxLeanCameraDolly.transform.localPosition = new Vector3(
                    _uxLeanCameraDolly.transform.localPosition.x,
                    _uxLeanCameraDolly.transform.localPosition.y,
                    -_storedDollySettings._dollyValue
                );
                _uxLeanCameraZoom._zoom = _storedZoomSettings._zoomValue;
                SwitchCameraMode((int)CameraMode.Zoom);
            }
        }

        private void Start()
        {
            if (_uxLeanPointOfViewOnStart)
            {
                LookAtTarget(_uxLeanPointOfViewOnStart);
            }
        }

        private void SetLookAtMode(bool active)
        {
            if (active)
            {
                _uxLeanCameraLocate._x = _lookLocate.x;
                _uxLeanCameraLocate._y = _lookLocate.y;
                _uxLeanCameraLocate._z = _lookLocate.z;
                _uxLeanCameraRotate._x = _lookEulerAngle.x;
                _uxLeanCameraRotate._y = _lookEulerAngle.y;
                _uxLeanCameraRotate._z = _lookEulerAngle.z;
                _uxLeanCameraDolly._dolly = _lookDistance;
                _uxLeanCameraDolly.transform.localPosition = new Vector3(
                    _uxLeanCameraDolly.transform.localPosition.x,
                    _uxLeanCameraDolly.transform.localPosition.y,
                    -_lookDistance
                );
                _uxLeanCameraZoom._zoom = _lookZoom;
                _camera.fieldOfView = _lookZoom;
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
