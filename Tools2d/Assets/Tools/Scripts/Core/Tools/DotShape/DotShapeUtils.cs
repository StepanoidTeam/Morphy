using System.Collections.Generic;
using Scripts.Core.Utils;
using UnityEngine;

namespace Scripts.Core.Tools.DotShape
{
    public static class DotShapeUtils
    {
        public static List<Vector2> GetCircle(Vector2 center, float radius, int total)
        {
            var step = Math2Utils.Pi2 / total;

            var points = new List<Vector2>();

            for (var i = 0; i < total; i++)
            {
                var pos = Math2Utils.ToCartesian(center, step * i, radius);
                points.Add(pos);
            }

            return points;
        }

        public static List<GameObject> CreateNodes(List<Vector2> points, float radius)
        {
            var list = new List<GameObject>();

            foreach (var point in points)
            {
                var node = PhysUtils.CreateCircleBody(point, radius, "node");
                node.rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

                list.Add(node);
            }

            return list;
        }

        public static void Join(List<GameObject> nodes, List<int> indexes, float frequency)
        {
            for (int i = 0; i < indexes.Count; i += 2)
            {
                var objA = nodes[indexes[i]];
                var objB = nodes[indexes[i + 1]];

                PhysUtils.CreateSpringJoint(objA, objB, frequency);
            }
        }
    }
}
