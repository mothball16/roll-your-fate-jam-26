using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Utility
{
    internal static class MouseUtils
    {
        public static Vector3 MouseToWorldPoint()
        {
            var mousePos = Mouse.current.position.ReadValue();
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
            return worldPos;
        }
        public static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);

                if (child.name == "Template")
                {
                    continue;
                }

                Object.Destroy(child.gameObject);
            }
        }
    }
}
