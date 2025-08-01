using Lean.Touch;
using UnityEngine;

namespace Eos.Ux.Lean
{
    [AddComponentMenu("UX Lean/UX Lean Camera Dolly")]
    [ExecuteInEditMode]
    public class UxLeanCameraDolly : MonoBehaviour
    {
        public bool _ignoreIfStartedOverGui = true;

        public bool _ignoreIfOverGui = true;

        public int _requiredFingerCount;

        public Vector3 _direction = -Vector3.forward;

        [Space(10.0f)] public float _dolly = 10.0f;

        public bool _dollyClamp;

        public float _dollyMin = 1.0f;

        public float _dollyMax = 100.0f;

        [Space(10.0f)] public bool _useWheel = false;

        [Range(-1.0f, 1.0f)] public float _wheelSensitivity;

        public LayerMask _collisionLayers;

        public float _collisionRadius = 0.1f;

        protected virtual void LateUpdate()
        {
            if (_useWheel)
            {
                var fingers = LeanTouch.GetFingers(_ignoreIfStartedOverGui, _ignoreIfOverGui, _requiredFingerCount);

                _dolly *= LeanGesture.GetPinchRatio(fingers, _wheelSensitivity);
            }

            if (_dollyClamp)
            {
                _dolly = Mathf.Clamp(_dolly, _dollyMin, _dollyMax);
            }

            transform.localPosition = Vector3.zero;

            if (_collisionLayers != 0)
            {
                var start          = transform.TransformPoint(_direction.normalized * _dollyMin);
                var direction      = transform.TransformDirection(_direction);
                var distanceSpread = _dollyMax - _dollyMin;

                if (Physics.SphereCast(start, _collisionRadius, direction, out var hit, distanceSpread, _collisionLayers) == true)
                {
                    _dolly = _dollyMin + hit.distance;
                }
            }

            transform.Translate(_direction.normalized * _dolly);
        }
    }
}
