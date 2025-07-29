using UnityEngine;

namespace Eos.Ux.Lean.Demo
{
    public class UxLeanDemoPointOfView : MonoBehaviour
    {
        [SerializeField] private UxLeanCameraManager _manager;
        [SerializeField] private UxLeanPointOfView _pointOfView1;
        [SerializeField] private UxLeanPointOfView _pointOfView2;
        [SerializeField] private UxLeanPointOfView _pointOfView3;

        public void SetPovAs(int povIndex)
        {
            switch (povIndex)
            {
                case 1:
                    SetPointOfView1();
                    break;
                case 2:
                    SetPointOfView2();
                    break;
                case 3:
                    SetPointOfView3();
                    break;
                default:
                    Debug.LogWarning("Invalid point of view index: " + povIndex);
                    break;
            }
        }

        private void SetPointOfView1()
        {
            if (_pointOfView1)
            {
                _manager.SetCameraProperties(
                    _pointOfView1.Settings._lookLocate,
                    _pointOfView1.Settings._lookEulerAngle,
                    _pointOfView1.Settings._lookDistance,
                    _pointOfView1.Settings._lookZoom
                );
            }
        }

        private void SetPointOfView2()
        {
            if (_pointOfView2)
            {
                _manager.SetCameraProperties(
                    _pointOfView2.Settings._lookLocate,
                    _pointOfView2.Settings._lookEulerAngle,
                    _pointOfView2.Settings._lookDistance,
                    _pointOfView2.Settings._lookZoom
                );
            }
        }

        private void SetPointOfView3()
        {
            if (_pointOfView3)
            {
                _manager.SetCameraProperties(
                    _pointOfView3.Settings._lookLocate,
                    _pointOfView3.Settings._lookEulerAngle,
                    _pointOfView3.Settings._lookDistance,
                    _pointOfView3.Settings._lookZoom
                );
            }
        }
    }
}
