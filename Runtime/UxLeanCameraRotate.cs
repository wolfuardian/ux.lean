using Lean.Touch;
using UnityEngine;

namespace Eos.Ux.Lean
{
    [AddComponentMenu("UX Lean/UX Lean Camera Rotate")]
    [ExecuteInEditMode]
    public class UxLeanCameraRotate : MonoBehaviour
    {
        public bool _ignoreIfStartedOverGui = true;

        public bool _ignoreIfOverGui = true;

        public int _requiredFingerCount;

        public Camera _camera;

        [Space(10.0f)] public float _x;

        public float _xSensitivity = -0.1f;

        public bool _xClamp = true;

        public float _xMin = -90.0f;

        public float _xMax = 90.0f;

        [Space(10.0f)] public float _y;

        public float _ySensitivity = -0.1f;

        public bool _yClamp;

        public float _yMin = -45.0f;

        public float _yMax = 45.0f;

        [Space(10.0f)] public float _z;

        public bool _zClamp;

        public float _zMin = -45.0f;

        public float _zMax = 45.0f;

        protected virtual void LateUpdate()
        {
            if (_camera)
            {
                var fingers = LeanTouch.GetFingers(_ignoreIfStartedOverGui, _ignoreIfOverGui, _requiredFingerCount);

                var drag = LeanGesture.GetScaledDelta(fingers);

                var sensitivity = GetSensitivity();

                _x += drag.y * _xSensitivity * sensitivity;

                if (_xClamp)
                {
                    _x = Mathf.Clamp(_x, _xMin, _xMax);
                }

                _y -= drag.x * _ySensitivity * sensitivity;

                if (_yClamp)
                {
                    _y = Mathf.Clamp(_y, _yMin, _yMax);
                }

                if (_zClamp)
                {
                    _z = Mathf.Clamp(_z, _zMin, _zMax);
                }

                UpdateRotation();
            }
        }

        private float GetSensitivity()
        {
            if (_camera.orthographic == false)
            {
                return _camera.fieldOfView / 90.0f;
            }

            return 1.0f;
        }

        private void UpdateRotation()
        {
            transform.localRotation = Quaternion.Euler(_x, _y, _z);
        }
    }
}
