using CW.Common;
using UnityEngine;

namespace Eos.Ux.Lean
{
    [AddComponentMenu("UX Lean/UX Lean Camera Locate Smooth")]
    public class UxLeanCameraLocateSmooth : UxLeanCameraLocate
    {
        [Space(10.0f)] public float _dampening = 6.0f;

        private Vector3 _currentLocate;

        protected override void LateUpdate()
        {
            var targetPosition = new Vector3(_x, _y, _z);

            base.LateUpdate();

            if (_ignoreSmooth)
            {
                _currentLocate = targetPosition;
                transform.localPosition = _currentLocate;
            }
            else
            {
                var factor = CwHelper.DampenFactor(_dampening, Time.deltaTime);

                _currentLocate = Vector3.Lerp(_currentLocate, targetPosition, factor);

                transform.localPosition = new Vector3(_currentLocate.x, _currentLocate.y, _currentLocate.z);
            }
        }
    }
}
