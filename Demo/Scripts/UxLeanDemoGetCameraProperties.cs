using UnityEngine;
using UnityEngine.Events;

namespace Eos.Ux.Lean.Demo
{
    public class UxLeanDemoGetCameraProperties : MonoBehaviour
    {
        [SerializeField] private UxLeanCameraManager _manager;

        [SerializeField] private UnityEvent<string> _onGetLocate = new UnityEvent<string>();
        [SerializeField] private UnityEvent<string> _onGetRotate = new UnityEvent<string>();
        [SerializeField] private UnityEvent<string> _onGetDolly = new UnityEvent<string>();
        [SerializeField] private UnityEvent<string> _onGetZoom = new UnityEvent<string>();

        private void Update()
        {
            _manager.GetCameraProperties(out var locate, out var rotate, out var dolly, out var zoom);
            _onGetLocate.Invoke(locate.ToString());
            _onGetRotate.Invoke(rotate.ToString());
            _onGetDolly.Invoke(dolly.ToString());
            _onGetZoom.Invoke(zoom.ToString());
        }
    }
}
