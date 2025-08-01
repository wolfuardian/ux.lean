using CW.Common;
using UnityEngine;

namespace Eos.Ux.Lean
{
    [AddComponentMenu("UX Lean/UX Lean Camera Zoom Smooth")]
    [ExecuteInEditMode]
    public class UxLeanCameraZoomSmooth : UxLeanCameraZoom
    {
        public float _dampening = 10.0f;

        private float _currentZoom;

        protected virtual void OnEnable()
        {
            _currentZoom = _zoom;
        }

        protected override void LateUpdate()
        {
            if (_camera)
            {
                base.LateUpdate();

                var factor = CwHelper.DampenFactor(_dampening, Time.deltaTime);

                _currentZoom = Mathf.Lerp(_currentZoom, _zoom, factor);

                SetZoom(_currentZoom);
            }
        }
    }
}
