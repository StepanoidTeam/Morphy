using System.Collections.Generic;
using System.Linq;
using Scripts.Core.Tools.Render;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UIShapes.Scripts.Core.Utils
{
    public static class DrawUtils
    {
        public static void DrawArc(this VertexHelper vh, Vector2 position, Vector2 radius, float from, float to, int steps, Color color)
        {
            vh.Clear();

            var list = new List<UIVertex>();
            var step = (to - from)/steps;

            list.Add(Vertex(position, new Vector2(0.5f, 0.5f), color));

            for (var i = 0; i < steps + 1; i++)
            {
                var a = from + i*step;
                
                var cos = Mathf.Cos(a);
                var sin = Mathf.Sin(a);
                
                var x = cos*radius.x + position.x;
                var y = sin*radius.y + position.y;

                var uv = new Vector2(cos, sin) * 0.5f + Vector2.one * 0.5f;

                list.Add(Vertex(new Vector2(x, y), uv, color));
            }
            
            vh.AddUIVertexStream(list, ShapeTopology.TringleFan(steps).ToList());
        }

        public static void DrawCone(this VertexHelper vh, Vector2 position, Vector2 radius, float width, float from, float to, int steps, Color color)
        {
            vh.Clear();

            var list = new List<UIVertex>();
            var step = (to - from) / steps;

            var radiusB = new Vector2(radius.x - width, radius.y - width);

            for (var i = 0; i < steps + 1; i++)
            {
                var a = from + i * step;

                var cos = Mathf.Cos(a);
                var sin = Mathf.Sin(a);

                var p = new Vector2(cos*radius.x, sin*radius.y) + position;
                var uv = new Vector2(cos, sin)*0.5f + Vector2.one*0.5f;
                list.Add(Vertex(p, uv, color));                
                
                var p2 = new Vector2(cos*radiusB.x, sin*radiusB.y) + position;
                var uvr = Vector2.one - new Vector2(width / radius.x, width / radius.y);
                var uv2 = new Vector2(cos*uvr.x, sin*uvr.y)*0.5f + Vector2.one*0.5f;
                list.Add(Vertex(p2, uv2, color));
            }

            vh.AddUIVertexStream(list, ShapeTopology.TringleStrip(steps*2).ToList());
        }

        public static void DrawLine(this VertexHelper vh, List<Vector2> positions, float width, Color color)
        {
            vh.Clear();
            var list = new List<UIVertex>();

            for (var i = 0; i < positions.Count; i++)
            {
                var point = positions[i];

                var pointPrev = positions[i > 0 ? i - 1 : i];
                var pointNext = positions[i < positions.Count - 1 ? i + 1 : i];

                var uvy = i%2 == 0 ? 1 : 0;
                var uv = new Vector2(uvy, 0);

                list.Add(Vertex(point, uv, color));

                var diff = (pointNext - pointPrev).normalized;
                var normal = new Vector2(-diff.y, diff.x) * width;
                var uv2 = new Vector2(uvy, 1);
                
                list.Add(Vertex(point + normal, uv2, color));                
            }

            vh.AddUIVertexStream(list, ShapeTopology.TringleStrip((positions.Count - 1)*2).ToList());
        }

        private static UIVertex Vertex(Vector2 position, Vector2 uv, Color color)
        {
            var vertex = UIVertex.simpleVert;
            vertex.position = position;
            vertex.uv0 = uv;
            vertex.color = color;

            return vertex;
        }
    }
}
