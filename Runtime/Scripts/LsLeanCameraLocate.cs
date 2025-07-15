using Lean.Touch;
using UnityEngine;

namespace Eos.Ux.Lean
{
    [ExecuteInEditMode]
    public class LsLeanCameraLocate : MonoBehaviour
    {
        public bool _ignoreIfStartedOverGui = true;

        public bool _ignoreIfOverGui = true;

        public int _requiredFingerCount;

        public Camera _camera;

        public float _distance = 1.0f;

        [Space(10.0f)] public float _x;

        public float _xSensitivity = 10.0f;

        public bool _xClamp = true;

        public float _xMin = -90.0f;

        public float _xMax = 90.0f;

        [Space(10.0f)] public float _y;

        public float _ySensitivity = 10.0f;

        public bool _yClamp;

        public float _yMin = -45.0f;

        public float _yMax = 45.0f;

        [Space(10.0f)] public float _z;

        public float _zSensitivity = 10.0f;

        public bool _zClamp;

        public float _zMin = -45.0f;

        public float _zMax = 45.0f;

        public bool _ignoreSmooth = false;

        protected virtual void LateUpdate()
        {
            if (_camera)
            {
                var fingers = LeanTouch.GetFingers(_ignoreIfStartedOverGui, _ignoreIfOverGui, _requiredFingerCount);
                
                var worldDelta = LeanGesture.GetWorldDelta(fingers, _distance, _camera);

                _x -= worldDelta.x * _xSensitivity;
                _y -= worldDelta.y * _ySensitivity;
                _z -= worldDelta.z * _zSensitivity;

                if (_xClamp)
                {
                    _x = Mathf.Clamp(_x, _xMin, _xMax);
                }

                if (_yClamp)
                {
                    _y = Mathf.Clamp(_y, _yMin, _yMax);
                }

                if (_zClamp)
                {
                    _z = Mathf.Clamp(_z, _zMin, _zMax);
                }

                transform.localPosition = new Vector3(_x, _y, _z);
            }
        }

        public void Tick()
        {
            LateUpdate();
        }

        public void DeactivateSmooth()
        {
            _ignoreSmooth = true;
        }

        public void ActivateSmooth()
        {
            _ignoreSmooth = false;
        }
    }
}
