using UnityEngine;

namespace BTHeraldryColorHelper
{
    public static class HierarchyLogger
    {
        public static void LogHierarchy(GameObject root, string prefix = "")
        {
            if (root == null)
            {
                Main.Log.LogDebug(prefix + "(null)");
                return;
            }
            Main.Log.LogDebug(prefix + root.name);
            foreach (Transform child in root.transform)
            {
                LogHierarchy(child.gameObject, prefix + "  ");
            }
        }
    }
}