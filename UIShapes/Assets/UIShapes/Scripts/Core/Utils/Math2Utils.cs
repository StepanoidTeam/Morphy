using UnityEngine;

namespace Scripts.Core.Utils
{
    public class Math2Utils
    {
        public const float Pi2 = Mathf.PI*2;

        public static Vector2 ToCartesian(Vector2 center, float angle, float radius)
        {
            var nodeX = center.x + radius * Mathf.Cos(angle);
            var nodeY = center.y + radius * Mathf.Sin(angle);
            return new Vector2(nodeX, nodeY);
        }
    }
}
