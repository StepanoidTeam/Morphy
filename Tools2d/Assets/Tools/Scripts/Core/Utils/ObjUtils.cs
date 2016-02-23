using UnityEngine;

namespace Scripts.Core.Utils
{
    public static class ObjUtils
    {
        public static void ToForlder(this GameObject obj, string folder)
        {
            var parent = GameObject.Find(folder) ?? new GameObject(folder);
            obj.transform.SetParent(parent.transform, false);
        }
    }
}
