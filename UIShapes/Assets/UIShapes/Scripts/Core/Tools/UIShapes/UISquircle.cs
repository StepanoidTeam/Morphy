using System.Collections.Generic;
using System.Linq;
using Scripts.Core.Tools.Render;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Core.Tools.UIShapes
{
    public class UISquircle : UIShape
    {
        [Range(3, 100)]
        public int Sides = 10;

        [Range(0, 10)]
        public float K = 0.8f;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            var center = (BottomLeft + UpRight) * 0.5f;
            var size = new Vector2(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;

            DrawArc(vh, center, size, 0, Mathf.PI * 2, Sides, color);
        }

        public  void DrawArc(VertexHelper vh, Vector2 position, Vector2 radius, float from, float to, int steps, Color color)
        {
            vh.Clear();

            var list = new List<UIVertex>();
            var step = (to - from) / steps;

            list.Add(Vertex(position, new Vector2(0.5f, 0.5f), color));

            for (var i = 0; i < steps + 1; i++)
            {
                var a = from + i * step;
                
                var cos = Mathf.Cos(a);
                var sin = Mathf.Sin(a);

                var dirX = cos > 0 ? 1 : -1;
                var dirY = sin > 0 ? 1 : -1;

                var rx = Mathf.Pow(Mathf.Abs(cos), K) * dirX;
                var ry = Mathf.Pow(Mathf.Abs(sin), K) * dirY;

                var x = rx * radius.x + position.x;
                var y = ry * radius.y + position.y;

                var uv = new Vector2(0.5f + x / rectTransform.rect.width, 0.5f + y / rectTransform.rect.height);

                list.Add(Vertex(new Vector2(x, y), uv, color));
            }

            vh.AddUIVertexStream(list, ShapeTopology.TringleFan(steps).ToList());
        }

        private UIVertex Vertex(Vector2 position, Vector2 uv, Color color)
        {
            var vertex = UIVertex.simpleVert;
            vertex.position = position;
            vertex.uv0 = uv;
            vertex.color = color;

            return vertex;
        }
    }
}
