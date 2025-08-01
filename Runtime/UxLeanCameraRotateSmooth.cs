using CW.Common;
using UnityEngine;

namespace Eos.Ux.Lean
{
    [AddComponentMenu("UX Lean/UX Lean Camera Rotate Smooth")]
    [ExecuteInEditMode]
    public class UxLeanCameraRotateSmooth : UxLeanCameraRotate
    {
        [Space(10.0f)] public float _dampening = 3.0f;

        private float _currentX;
        private float _currentY;
        private float _currentZ;

        protected virtual void OnEnable()
        {
            _currentX = _x;
            _currentY = _y;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            var factor = CwHelper.DampenFactor(_dampening, Time.deltaTime);

            _currentX = Mathf.Lerp(_currentX, _x, factor);
            _currentY = Mathf.Lerp(_currentY, _y, factor);
            _currentZ = Mathf.Lerp(_currentZ, _z, factor);

            transform.localRotation = Quaternion.Euler(_currentX, _currentY, _currentZ);
        }
    }
}
