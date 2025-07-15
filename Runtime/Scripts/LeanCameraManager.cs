using System.Collections;
using UnityEngine;

namespace Eos.Ux.Lean
{
    public class LeanCameraManager : MonoBehaviour
    {
        public enum ZDistMode
        {
            Zoom = 0,
            Dolly = 1
        }

        [SerializeField] private LeanCameraLocate _leanCameraLocate;
        [SerializeField] private LeanCameraRotate _leanCameraRotate;
        [SerializeField] private LsLeanCameraDolly _lsLeanCameraDolly;
        [SerializeField] private LeanCameraZoom _leanCameraZoom;
        [SerializeField] private Camera _camera;

        private int _zDistModeId = (int)ZDistMode.Zoom; // Default as zoom mode, this is the best way to view.
        private Transform _lookTarget = null;
        private Vector3 _lookLocate = new Vector3(0f, 0f, 0f);
        private float _lookDistance = 90f;
        private Vector3 _lookEulerAngle = new Vector3(15f, -30f, 0f);
        private float _lookZoom = 50f;

        private float _currentTurntableSpeed;
        private bool _isTurntableActive;

        private LeanCameraLocateSettings _storedLocateSettings = new LeanCameraLocateSettings();
        private LeanCameraRotateSettings _storedRotateSettings = new LeanCameraRotateSettings();
        private LeanCameraDollySettings _storedDollySettings = new LeanCameraDollySettings();
        private LeanCameraZoomSettings _storedZoomSettings = new LeanCameraZoomSettings();

        public int ZDistModeId
        {
            set
            {
                _zDistModeId = value;
                SwitchZoomOrDolly(_zDistModeId);
            }
        }

        [ContextMenu("Copy properties as Settings to JSON")]
        private void CopyPropertiesAsSettingsToJson()
        {
            var povSettings = new LeanPointOfView.POVSettings
            {
                _lookLocate = new Vector3(_leanCameraLocate._x, _leanCameraLocate._y, _leanCameraLocate._z),
                _lookEulerAngle = new Vector3(_leanCameraRotate._x, _leanCameraRotate._y, _leanCameraRotate._z),
                _lookDistance = -_lsLeanCameraDolly.transform.localPosition.z,
                _lookZoom = _camera.fieldOfView
            };

            var json = JsonUtility.ToJson(povSettings);
            GUIUtility.systemCopyBuffer = json;
        }

        [ContextMenu("Paste & Parse Settings From JSON")]
        private void PastePovSettingsFromJson()
        {
            var json = GUIUtility.systemCopyBuffer;

            try
            {
                var povSettings = JsonUtility.FromJson<LeanPointOfView.POVSettings>(json);

                _leanCameraLocate._x = povSettings._lookLocate.x;
                _leanCameraLocate._y = povSettings._lookLocate.y;
                _leanCameraLocate._z = povSettings._lookLocate.z;
                _leanCameraRotate._x = povSettings._lookEulerAngle.x;
                _leanCameraRotate._y = povSettings._lookEulerAngle.y;
                _leanCameraRotate._z = povSettings._lookEulerAngle.z;
                _lsLeanCameraDolly._dolly = -povSettings._lookDistance;
                _lsLeanCameraDolly.transform.localPosition = new Vector3(
                    _lsLeanCameraDolly.transform.localPosition.x,
                    _lsLeanCameraDolly.transform.localPosition.y,
                    -_lookDistance
                );
                _leanCameraZoom._zoom = povSettings._lookZoom;
                _camera.fieldOfView = povSettings._lookZoom;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to parse settings from JSON: " + ex.Message);
            }
        }

        public void LookAtTarget(LeanPointOfView pov)
        {
            _lookLocate = pov.Settings._lookLocate;
            _lookDistance = pov.Settings._lookDistance;
            _lookEulerAngle = pov.Settings._lookEulerAngle;
            _lookZoom = pov.Settings._lookZoom;
            SetLookAtMode(true);
        }

        public void LookAtTarget(Transform target, float distance, Vector3 eulerAngle, float zoom)
        {
            _lookTarget = target;
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

        private void SetLookAtMode(bool active)
        {
            if (active)
            {
                SetFreeMode(true);
                _leanCameraLocate._x = _lookLocate.x;
                _leanCameraLocate._y = _lookLocate.y;
                _leanCameraLocate._z = _lookLocate.z;
                _leanCameraRotate._x = _lookEulerAngle.x;
                _leanCameraRotate._y = _lookEulerAngle.y;
                _leanCameraRotate._z = _lookEulerAngle.z;
                _lsLeanCameraDolly._dolly = _lookDistance;
                _lsLeanCameraDolly.transform.localPosition = new Vector3(
                    _lsLeanCameraDolly.transform.localPosition.x,
                    _lsLeanCameraDolly.transform.localPosition.y,
                    -_lookDistance
                );
                _leanCameraZoom._zoom = _lookZoom;
                _camera.fieldOfView = _lookZoom;
            }
            else
            {
                SetFreeMode(false);
            }
        }

        public void SetFreeMode(bool active)
        {
            if (active)
            {
                // Store values to settings
                _storedLocateSettings._xyzValue = new Vector3(_leanCameraLocate._x, _leanCameraLocate._y, _leanCameraLocate._z);
                _leanCameraLocate.DeactivateSmooth();
                _leanCameraLocate._x = _leanCameraZoom.transform.position.x;
                _leanCameraLocate._y = _leanCameraZoom.transform.position.y;
                _leanCameraLocate._z = _leanCameraZoom.transform.position.z;
                _leanCameraLocate.Tick();
                _leanCameraLocate.ActivateSmooth();

                _storedRotateSettings._xyzValue = new Vector3(_leanCameraRotate._x, _leanCameraRotate._y, _leanCameraRotate._z);

                _storedDollySettings._dollyValue = _lsLeanCameraDolly._dolly;
                _lsLeanCameraDolly._dolly = 1f;
                _lsLeanCameraDolly.transform.localPosition = new Vector3(
                    _lsLeanCameraDolly.transform.localPosition.x,
                    _lsLeanCameraDolly.transform.localPosition.y,
                    1f
                );
                _storedZoomSettings._zoomValue = _leanCameraZoom._zoom;

                SwitchToDolly();
            }
            else
            {
                // Restore values from settings
                _leanCameraLocate._x = _storedLocateSettings._xyzValue.x;
                _leanCameraLocate._y = _storedLocateSettings._xyzValue.y;
                _leanCameraLocate._z = _storedLocateSettings._xyzValue.z;
                _leanCameraRotate._x = _storedRotateSettings._xyzValue.x;
                _leanCameraRotate._y = _storedRotateSettings._xyzValue.y;
                _leanCameraRotate._z = _storedRotateSettings._xyzValue.z;
                _lsLeanCameraDolly._dolly = _storedDollySettings._dollyValue;
                _lsLeanCameraDolly.transform.localPosition = new Vector3(
                    _lsLeanCameraDolly.transform.localPosition.x,
                    _lsLeanCameraDolly.transform.localPosition.y,
                    -_storedDollySettings._dollyValue
                );
                _leanCameraZoom._zoom = _storedZoomSettings._zoomValue;
                ZDistModeId = _zDistModeId;
                SwitchToZoom();
            }
        }

        public void StartTurntable(float targetSpeed)
        {
            _isTurntableActive = true;
            StopAllCoroutines();
            StartCoroutine(AccelTurnSpeedToTarget(targetSpeed, 5f));
        }

        public void StopTurntable()
        {
            _isTurntableActive = false;
            StopAllCoroutines();
            StartCoroutine(AccelTurnSpeedToZero(5f));
        }

        private void FixedUpdate()
        {
            _leanCameraRotate._y += _currentTurntableSpeed * Time.deltaTime;
        }

        private void SwitchToZoom() => ZDistModeId = (int)ZDistMode.Zoom;
        private void SwitchToDolly() => ZDistModeId = (int)ZDistMode.Dolly;

        private void SwitchZoomOrDolly(int m)
        {
            _lsLeanCameraDolly.enabled = false;
            _leanCameraZoom.enabled = false;
            switch (m)
            {
                case (int)ZDistMode.Zoom:
                    _leanCameraZoom.enabled = true;
                    break;
                case (int)ZDistMode.Dolly:
                    _lsLeanCameraDolly.enabled = true;
                    break;
            }
        }

        private IEnumerator AccelTurnSpeedToTarget(float targetSpeed, float acceleration)
        {
            while (Mathf.Abs(_currentTurntableSpeed - targetSpeed) > 0.01f)
            {
                _currentTurntableSpeed = Mathf.MoveTowards(_currentTurntableSpeed, targetSpeed, acceleration * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator AccelTurnSpeedToZero(float acceleration)
        {
            while (Mathf.Abs(_currentTurntableSpeed) > 0.01f)
            {
                _currentTurntableSpeed = Mathf.MoveTowards(_currentTurntableSpeed, 0f, acceleration * Time.deltaTime);
                yield return null;
            }
        }
    }

    [System.Serializable]
    internal class LeanCameraLocateSettings
    {
        public Vector3 _xyzValue;
    }

    [System.Serializable]
    internal class LeanCameraRotateSettings
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
}
