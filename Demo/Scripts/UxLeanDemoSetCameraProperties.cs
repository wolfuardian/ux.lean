using UnityEngine;

namespace Eos.Ux.Lean.Demo
{
    public class UxLeanDemoSetCameraProperties : MonoBehaviour
    {
        [SerializeField] private UxLeanCameraManager _manager;
        [SerializeField] private float _locateXMultiplier = 1f;
        [SerializeField] private float _locateYMultiplier = 1f;
        [SerializeField] private float _locateZMultiplier = 1f;
        [SerializeField] private float _rotateXMultiplier = 1f;
        [SerializeField] private float _rotateYMultiplier = 1f;
        [SerializeField] private float _rotateZMultiplier = 1f;
        [SerializeField] private float _dollyMin = 1f;
        [SerializeField] private float _dollyMax = 200f;
        [SerializeField] private float _zoomMin = 1f;
        [SerializeField] private float _zoomMax = 200f;

        public Vector3 Locate
        {
            get
            {
                _manager.GetCameraProperties(out var locate, out _, out _, out _);
                return locate;
            }
        }

        public Vector3 Rotate
        {
            get
            {
                _manager.GetCameraProperties(out _, out var rotate, out _, out _);
                return rotate;
            }
        }

        public float Dolly
        {
            get
            {
                _manager.GetCameraProperties(out _, out _, out var dolly, out _);
                return dolly;
            }
        }

        public float Zoom
        {
            get
            {
                _manager.GetCameraProperties(out _, out _, out _, out var zoom);
                return zoom;
            }
        }

        public void SetLocateX(float value)
        {
            _manager.SetCameraProperties(
                locate: new Vector3(
                    Remap01ToSigned(value) * _locateXMultiplier,
                    Locate.y,
                    Locate.z
                ),
                rotate: Rotate,
                dolly: Dolly,
                zoom: Zoom
            );
        }

        public void SetLocateY(float value)
        {
            _manager.SetCameraProperties(
                locate: new Vector3(
                    Locate.x,
                    Remap01ToSigned(value) * _locateYMultiplier,
                    Locate.z
                ),
                rotate: Rotate,
                dolly: Dolly,
                zoom: Zoom
            );
        }

        public void SetLocateZ(float value)
        {
            _manager.SetCameraProperties(
                locate: new Vector3(
                    Locate.x,
                    Locate.y,
                    Remap01ToSigned(value) * _locateZMultiplier
                ),
                rotate: Rotate,
                dolly: Dolly,
                zoom: Zoom
            );
        }

        public void SetRotateX(float value)
        {
            _manager.SetCameraProperties(
                locate: Locate,
                rotate: new Vector3(
                    Remap01ToSigned(value) * _rotateXMultiplier,
                    Rotate.y,
                    Rotate.z
                ),
                dolly: Dolly,
                zoom: Zoom
            );
        }

        public void SetRotateY(float value)
        {
            _manager.SetCameraProperties(
                locate: Locate,
                rotate: new Vector3(
                    Rotate.x,
                    Remap01ToSigned(value) * _rotateYMultiplier,
                    Rotate.z
                ),
                dolly: Dolly,
                zoom: Zoom
            );
        }

        public void SetRotateZ(float value)
        {
            _manager.SetCameraProperties(
                locate: Locate,
                rotate: new Vector3(
                    Rotate.x,
                    Rotate.y,
                    Remap01ToSigned(value) * _rotateZMultiplier
                ),
                dolly: Dolly,
                zoom: Zoom
            );
        }

        public void SetDolly(float value)
        {
            _manager.SetCameraProperties(
                locate: Locate,
                rotate: Rotate,
                dolly: Mathf.Lerp(
                    _dollyMin,
                    _dollyMax,
                    value
                ),
                zoom: Zoom
            );
        }

        public void SetZoom(float value)
        {
            _manager.SetCameraProperties(
                locate: Locate,
                rotate: Rotate,
                dolly: Dolly,
                zoom: Mathf.Lerp(
                    _zoomMin,
                    _zoomMax,
                    value
                )
            );
        }

        private static float Remap01ToSigned(float value)
        {
            value = (value - 0.5f) * 2f;
            return value;
        }
    }
}
