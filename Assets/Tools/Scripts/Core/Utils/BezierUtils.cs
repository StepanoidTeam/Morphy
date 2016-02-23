using System;
using System.Collections.Generic;
using Debug = System.Diagnostics.Debug;
using UnityEngine;

namespace Scripts.Core.Utils
{
    public class BezierUtils
    {
        public static void AddControlPoints(List<Vector2> target, List<Vector2> points)
        {
            target.Add(points[0]);
            target.Add(points[0]);

            for (var i = 0; i < points.Count - 1; i++)
            {
                target.Add((points[i] + points[i + 1]) * 0.5f);
                target.Add(points[i + 1]);
            }
            target.Add(points[points.Count - 1]);
        }

        public static void AddBezierPoints(List<Vector2> target, List<Vector2> points, int segments)
        {
            Debug.Assert(points.Count >= 3, "Line points must have at least 3 elements to draw bezier");
            for (var i = 0; i < points.Count - 2; i += 2)
                GetBezierPoints(
                    points[i], points[i + 1], points[i + 2], segments, false, target);


            target.Add(points[points.Count - 1]);
        }

        public static void AddBezierPoints(List<Vector2> target, List<Vector2> points, List<int> segmentsVector)
        {
            Debug.Assert(points.Count >= 3, "Line points must have at least 3 elements to draw bezier");
            for (var i = 0; i < points.Count - 2; i += 2)
                GetBezierPoints(points[i], points[i + 1], points[i + 2],
                                segmentsVector[i / 2], false, target);

            target.Add(points[points.Count - 1]);
        }

        public static void GetBezierPoints(Vector2 origin, Vector2 control, Vector2 destination, int segments, bool insertLast, List<Vector2> result)
        {
            var t = 0.0f;
            for (var i = 0; i < segments; i++)
            {
                var x = (float)Math.Pow(1 - t, 2) * origin.x + 2.0f * (1 - t) * t * control.x + t * t * destination.x;
                var y = (float)Math.Pow(1 - t, 2) * origin.y + 2.0f * (1 - t) * t * control.y + t * t * destination.y;
                result.Add(new Vector2(x, y));
                t += 1.0f / segments;
            }

            if (insertLast)
                result.Add(destination);
        }

        public static void CalculateBezierPath(List<Vector2> source, List<Vector2> controlPoints, List<Vector2> bezierPoints, int segments)
        {
            controlPoints.Clear();
            AddControlPoints(controlPoints, source);

            bezierPoints.Clear();
            AddBezierPoints(bezierPoints, controlPoints, segments);

            bezierPoints.RemoveRange(bezierPoints.Count - 4, 4);
            bezierPoints.RemoveRange(0, 3);
        }
    }
}
