using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Core.Tools.Bezier
{
    public class BezierCurve : Curve
    {
        public Vector3[] Points;

        public void Reset()
        {
            Points = new[]
            {
                new Vector3(1f, 0f, 0f),
                new Vector3(2f, 0f, 0f),
                new Vector3(3f, 0f, 0f),
                new Vector3(4f, 0f, 0f)
            };
        }

        public void AddCurve(int index, Vector3 newPoint)
        {
            var newItems = new List<Vector3>();
            
            newItems.Add(Vector3.Lerp(newPoint, Points[index], 0.3f));
            newItems.Add(newPoint);
            newItems.Add(Vector3.Lerp(newPoint, Points[index + 2], 0.3f));

            var list = Points.ToList();
            list.InsertRange(index + 1, newItems);
            Points = list.ToArray();
        }

        public void RemoveCurve(int index)
        {
            var startIndex = index - 1;

            if (index == 0)
                startIndex = 0;

            if (index == Points.Length - 1)
                startIndex = Points.Length - 3;

            var list = Points.ToList();
            list.RemoveRange(startIndex, 3);
            Points = list.ToArray();
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - Points[index];
                if (index > 0)
                {
                    Points[index - 1] += delta;
                }
                if (index + 1 < Points.Length)
                {
                    Points[index + 1] += delta;
                }
            }
            Points[index] = point;

        }

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        public override List<Vector3> GetCurve()
        {
            var result = new List<Vector3>();
            var i = 1;
            for (; i < Points.Length; i+= 3)
            {
                result.Add(Get(i, 0));
                result.Add(Get(i, 0.1f));
                result.Add(Get(i, 0.2f));
                result.Add(Get(i, 0.3f));
                result.Add(Get(i, 0.4f));
                result.Add(Get(i, 0.5f));
                result.Add(Get(i, 0.6f));
                result.Add(Get(i, 0.7f));
                result.Add(Get(i, 0.8f));
                result.Add(Get(i, 0.9f));
            }
            result.Add(Get(i - 3, 1f));

            return result;
        }

        private Vector3 Get(int i, float t)
        {
            return GetPoint(Points[i - 1], Points[i], Points[i + 1], Points[i + 2], t);  
        }
    }
}
