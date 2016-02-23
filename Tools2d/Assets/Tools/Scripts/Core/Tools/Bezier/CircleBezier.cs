using System.Collections.Generic;
using Scripts.Core.Utils;
using UnityEngine;

namespace Scripts.Core.Tools.Bezier
{
    public class CircleBezier
    {
        private List<Vector2> vertices; 

        private readonly List<Vector2> controlPoints = new List<Vector2>();
        private readonly List<Vector2> bezierPoints = new List<Vector2>();

        public void CalculateBezierPath(int segments)
        {
            vertices.Add(vertices[0]);
            vertices.Add(vertices[1]);

            BezierUtils.CalculateBezierPath(vertices, controlPoints, bezierPoints, segments);
            
            vertices.Clear();
        }

        public void SetVertexCount(int count)
        {
            vertices = new List<Vector2>(count + 2);
        }

        public void AddPosition(Vector2 position)
        {
            vertices.Add(position);
        }

        public List<Vector2> Result
        {
            get { return bezierPoints; }
        }
    }
}
