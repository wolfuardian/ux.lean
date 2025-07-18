using CW.Common;
using UnityEngine;

namespace Eos.Ux.Lean
{
    [ExecuteInEditMode]
    public class LsLeanCameraDollySmooth : LsLeanCameraDolly
    {
        public float _dampening = 6.0f;

        private float _currentDolly;

        protected virtual void OnEnable()
        {
            _currentDolly = _dolly;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            var factor = CwHelper.DampenFactor(_dampening, Time.deltaTime);

            _currentDolly = Mathf.Lerp(_currentDolly, _dolly, factor);

            transform.localPosition = Vector3.zero;

            transform.Translate(_direction.normalized * _currentDolly);
        }
    }
}
