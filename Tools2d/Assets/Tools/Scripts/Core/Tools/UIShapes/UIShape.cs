using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Core.Tools.UIShapes
{
    public class UIShape : Graphic
    {
        protected List<UIVertex> Vbo { get; private set; }
        protected Vector2 Corner1;
        protected Vector2 Corner2;

        protected override void OnFillVBO(List<UIVertex> vbo)
        {
            Vbo = vbo;
            Vbo.Clear();

            CalculateCorners();
        }

        private void CalculateCorners()
        {
            Corner1 = Vector2.zero;
            Corner2 = Vector2.one;

            Corner1 -= rectTransform.pivot;
            Corner2 -= rectTransform.pivot;

            Corner1.x *= rectTransform.rect.width;
            Corner1.y *= rectTransform.rect.height;
            Corner2.x *= rectTransform.rect.width;
            Corner2.y *= rectTransform.rect.height;
        }

        protected void DrawArc(Vector2 center, float radius, float start, float angle, int steps, Color color)
        {
            var step = angle / steps;

            for (var i = 0; i < steps; i++)
            {
                var a1 = start + i * step;
                var a2 = start + (i + 1) * step;
                var p1 = center + radius*new Vector2(Mathf.Cos(a1), Mathf.Sin(a1));
                var p2 = center + radius*new Vector2(Mathf.Cos(a2), Mathf.Sin(a2));

                DrawTringle(center, p1, p2, color);
            }
        }

        protected void DrawArc2(Vector2 center, float radius1, float radius2, float start, float angle, int steps, Color color)
        {
            var step = angle / steps;

            for (var i = 0; i < steps; i++)
            {
                var a1 = start + i * step;
                var a2 = start + (i + 1) * step;
                var dir1 = new Vector2(Mathf.Cos(a1), Mathf.Sin(a1));
                var dir2 = new Vector2(Mathf.Cos(a2), Mathf.Sin(a2));
                var p1 = center + radius1 * dir1;
                var p2 = center + radius1 * dir2;
                var p3 = center + radius2 * dir2;
                var p4 = center + radius2 * dir1;

                DrawQuad(p1, p2, p3, p4, color);
            }
        }


        protected void DrawArc3(Vector2 center, float radius1, float radius2, float start, float angle, int steps, Color c1, Color c2)
        {
            var step = angle / steps;

            for (var i = 0; i < steps; i++)
            {
                var a1 = start + i * step;
                var a2 = start + (i + 1) * step;
                var dir1 = new Vector2(Mathf.Cos(a1), Mathf.Sin(a1));
                var dir2 = new Vector2(Mathf.Cos(a2), Mathf.Sin(a2));
                var p1 = center + radius1 * dir1;
                var p2 = center + radius1 * dir2;
                var p3 = center + radius2 * dir2;
                var p4 = center + radius2 * dir1;

                DrawVertex(p1, c1);
                DrawVertex(p2, c1);
                DrawVertex(p3, c2);
                DrawVertex(p4, c2);
            }
        }

        protected void DrawTringle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
        {
            DrawVertex(p1, color);
            DrawVertex(p1, color);
            DrawVertex(p2, color);
            DrawVertex(p3, color);
        }

        protected void DrawQuad(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color)
        {
            DrawVertex(p1, color);
            DrawVertex(p2, color);
            DrawVertex(p3, color);
            DrawVertex(p4, color);
        }

        protected void DrawVertex(Vector2 position)
        {
            DrawVertex(position, color);
        }

        protected void DrawVertex(Vector2 position, Color color)
        {
            var vert = UIVertex.simpleVert;
            vert.position = position;
            vert.color = color;
            Vbo.Add(vert);
        }

        protected Color ToTransparent(Color value)
        {
            var result = value;
            result.a = 0;
            return result;
        }
    }
}
