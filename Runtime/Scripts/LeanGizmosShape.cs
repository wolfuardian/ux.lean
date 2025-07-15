#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Eos.Ux.Lean
{
    public class LeanGizmosShape
    {
        public static void DrawCircle(Vector3 center, Vector3 normal, float radius, Color color)
        {
            Handles.color = color;
            Handles.DrawWireDisc(center, normal.normalized, radius);
        }
    }
}

#endif
