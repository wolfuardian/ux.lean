using Lean.Touch;
using UnityEditor;
using UnityEngine;

namespace Eos.Ux.Lean.Editor
{
    public static class LeanCameraMenu
    {
        [MenuItem("GameObject/Eos UX/Lean/LeanCamera (Dolly)", false, 1)]
        public static void CreateLeanCameraDollyHierarchy()
        {
            CreateLeanCamera((int)LsLeanCameraManager.CameraMode.Dolly);
        }

        [MenuItem("GameObject/Eos UX/Lean/LeanCamera (Zoom)", false, 1)]
        public static void CreateLeanCameraZoomHierarchy()
        {
            CreateLeanCamera((int)LsLeanCameraManager.CameraMode.Zoom);
        }

        public static void CreateLeanCamera(int cameraMode)
        {
            var cameraModeInfo = cameraMode == (int)LsLeanCameraManager.CameraMode.Dolly ? "(Dolly)" : "(Zoom)";
            var rootGo         = new GameObject("new LeanCamera " + cameraModeInfo);
            var locateGo       = new GameObject("Locate");
            var rotateGo       = new GameObject("Rotate");
            var zGo            = new GameObject("Z");
            var cameraGo       = new GameObject("Camera", typeof(Camera));
            var camera         = cameraGo.GetComponent<Camera>();

            locateGo.transform.SetParent(rootGo.transform);
            rotateGo.transform.SetParent(locateGo.transform);
            zGo.transform.SetParent(rotateGo.transform);
            cameraGo.transform.SetParent(zGo.transform);

            var spawnPos = SceneView.lastActiveSceneView?.camera?.transform.position ?? Vector3.zero;
            rootGo.transform.position = spawnPos + Vector3.forward * 5f;

            SetupLocate(locateGo, camera);
            SetupRotate(rotateGo, camera);
            SetupZ(zGo);
            SetupCamera(camera);
            SetupManager(rootGo, locateGo, rotateGo, zGo, cameraGo);

            Undo.RegisterCreatedObjectUndo(rootGo, "Create LeanCamera Hierarchy");

            rootGo.transform.SetParent(Selection.activeTransform, false);

            Selection.activeGameObject = rootGo;

            switch (cameraMode)
            {
                case (int)LsLeanCameraManager.CameraMode.Dolly:
                    rootGo.GetComponent<LsLeanCameraManager>().SwitchCameraMode((int)LsLeanCameraManager.CameraMode.Dolly);
                    break;
                case (int)LsLeanCameraManager.CameraMode.Zoom:
                    rootGo.GetComponent<LsLeanCameraManager>().SwitchCameraMode((int)LsLeanCameraManager.CameraMode.Zoom);
                    break;
            }
        }

        private static void SetupLocate(GameObject locate, Camera camera)
        {
            locate.AddComponent<LeanTouch>();
            var leanTouchSimulator = locate.AddComponent<LeanTouchSimulator>();
            leanTouchSimulator.MovePivotKey = KeyCode.LeftAlt;
            leanTouchSimulator.MultiDragKey = KeyCode.Mouse1;

            var lsLeanCameraLocate = locate.AddComponent<LsLeanCameraLocateSmooth>();
            lsLeanCameraLocate._requiredFingerCount = 2;
            lsLeanCameraLocate._x = 0f;
            lsLeanCameraLocate._y = 0f;
            lsLeanCameraLocate._z = 0f;
            lsLeanCameraLocate._xSensitivity = 100f;
            lsLeanCameraLocate._ySensitivity = 100f;
            lsLeanCameraLocate._zSensitivity = 100f;

            lsLeanCameraLocate._camera = camera;
        }

        private static void SetupRotate(GameObject rotate, Camera camera)
        {
            var lsLeanCameraRotate = rotate.AddComponent<LsLeanCameraRotateSmooth>();
            lsLeanCameraRotate._requiredFingerCount = 1;
            lsLeanCameraRotate._x = 0f;
            lsLeanCameraRotate._y = 0f;
            lsLeanCameraRotate._z = 0f;
            lsLeanCameraRotate._xClamp = true;
            lsLeanCameraRotate._xMin = -90f;
            lsLeanCameraRotate._xMax = 90f;
            lsLeanCameraRotate._xSensitivity = -0.25f;
            lsLeanCameraRotate._ySensitivity = -0.25f;

            lsLeanCameraRotate._camera = camera;
        }

        private static void SetupZ(GameObject z)
        {
            var lsLeanCameraDolly = z.AddComponent<LsLeanCameraDollySmooth>();
            lsLeanCameraDolly._requiredFingerCount = 1;
            lsLeanCameraDolly._dolly = 150f;
            lsLeanCameraDolly._dollyClamp = true;
            lsLeanCameraDolly._dollyMin = 1f;
            lsLeanCameraDolly._dollyMax = 200f;
            lsLeanCameraDolly._useWheel = true;
            lsLeanCameraDolly._wheelSensitivity = -0.15f;

            lsLeanCameraDolly.transform.localPosition = new Vector3(0f, 0f, -150f);
        }

        private static void SetupCamera(Camera camera)
        {
            var lsLeanCameraZoom = camera.gameObject.AddComponent<LsLeanCameraZoomSmooth>();
            lsLeanCameraZoom._requiredFingerCount = 1;
            lsLeanCameraZoom._zoom = 50f;
            lsLeanCameraZoom._zoomClamp = true;
            lsLeanCameraZoom._zoomMin = 1f;
            lsLeanCameraZoom._zoomMax = 200f;
            lsLeanCameraZoom._useWheel = true;
            lsLeanCameraZoom._wheelSensitivity = -0.15f;

            lsLeanCameraZoom._camera = camera;

            var c = camera.GetComponent<Camera>();
            c.usePhysicalProperties = true;
            c.fieldOfView = 50f;
        }

        private static void SetupManager(GameObject root, GameObject locate, GameObject rotate, GameObject z, GameObject camera)
        {
            root.transform.position = Vector3.zero;
            var manager = root.AddComponent<LsLeanCameraManager>();
            manager.LeanCameraLocate = locate.GetComponent<LsLeanCameraLocateSmooth>();
            manager.LeanCameraRotate = rotate.GetComponent<LsLeanCameraRotateSmooth>();
            manager.LeanCameraDolly = z.GetComponent<LsLeanCameraDollySmooth>();
            manager.LeanCameraZoom = camera.GetComponent<LsLeanCameraZoomSmooth>();
            manager.LeanCamera = camera.GetComponent<Camera>();
        }
    }
}
