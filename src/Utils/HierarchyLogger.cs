using HBS.Logging;
using UnityEngine;

namespace HeraldryPicker.Utils
{
    public static class HierarchyLogger
    {
        internal static readonly ILog Log = HBS.Logging.Logger.GetLogger("Unity", LogLevel.Debug);
        public static void LogHierarchy(GameObject root, string prefix = "")
        {
            if (root == null)
            {
                Log.LogDebug(prefix + "(null)");
                return;
            }
            Log.LogDebug(prefix + root.name);
            foreach (Transform child in root.transform)
            {
                LogHierarchy(child.gameObject, prefix + "  ");
            }
        }
    }
}