using Lean.Touch;
using UnityEngine;

namespace Eos.Ux.Lean
{
    [ExecuteInEditMode]
    public class UxLeanCameraZoom : MonoBehaviour
    {
        public bool _ignoreIfStartedOverGui = true;

        public bool _ignoreIfOverGui = true;

        public int _requiredFingerCount;

        public Camera _camera;

        [Space(10.0f)] public float _zoom = 50.0f;

        public bool _zoomClamp;

        public float _zoomMin = 10.0f;

        public float _zoomMax = 60.0f;

        [Space(10.0f)] public bool _useWheel = true;

        [Range(-1.0f, 1.0f)] public float _wheelSensitivity;

        protected virtual void LateUpdate()
        {
            if (_camera)
            {
                if (_useWheel)
                {
                    var fingers = LeanTouch.GetFingers(_ignoreIfStartedOverGui, _ignoreIfOverGui, _requiredFingerCount);

                    var pinchRatio = LeanGesture.GetPinchRatio(fingers, _wheelSensitivity);

                    _zoom *= pinchRatio;
                }

                if (_zoomClamp)
                {
                    _zoom = Mathf.Clamp(_zoom, _zoomMin, _zoomMax);
                }

                SetZoom(_zoom);
            }
        }

        protected void SetZoom(float current)
        {
            if (_camera.orthographic)
            {
                _camera.orthographicSize = current;
            }
            else
            {
                _camera.fieldOfView = current;
            }
        }
    }
}
