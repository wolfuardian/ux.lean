using UnityEditor;
using UnityEngine;

namespace Eos.Ux.Lean.Editor
{
    public static class UxLeanPointOfViewMenu
    {
        [MenuItem("GameObject/Eos/UX Lean (Camera)/LeanPointOfView", false, 3)]
        public static void CreateLeanPointOfViewHierarchy()
        {
            var povGo = new GameObject("new POV");

            SetupPointOfView(povGo);

            Undo.RegisterCreatedObjectUndo(povGo, "Create LeanPointOfView");

            povGo.transform.SetParent(Selection.activeTransform, false);

            Selection.activeGameObject = povGo;
        }

        private static void SetupPointOfView(GameObject povGo)
        {
            const string BASE_NAME = "POV";

            var lsLeanPointOfView = povGo.AddComponent<UxLeanPointOfView>();
            var index             = 1;
            var bookmark          = BASE_NAME + index;

            while (index < 1000)
            {
                if (!UxLeanPointOfView.BookmarkExists(bookmark))
                {
                    lsLeanPointOfView.Bookmark = bookmark;
                    break;
                }
                index++;
                bookmark = BASE_NAME + index;
            }

            lsLeanPointOfView.Bookmark = bookmark;
            lsLeanPointOfView.transform.localScale = Vector3.one * 100f;
            lsLeanPointOfView.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            povGo.name += index;
        }
    }
}
