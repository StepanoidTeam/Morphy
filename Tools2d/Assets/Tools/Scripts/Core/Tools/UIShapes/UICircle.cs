using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core.Tools.UIShapes
{
    public class UICircle : UIShape
    {
        public Color BorderColor = Color.white;

        public bool ShowShape = true;
        public bool ShowBorder = true;

        public float BorderWidth = 0;
        
        [Range(0, 10)] public int Smoothing = 1;
        [Range(3, 50)] public int Sides = 10;

        protected override void OnFillVBO(List<UIVertex> vbo)
        {
            base.OnFillVBO(vbo);

            var step = Mathf.PI*2/Sides;

            for (var i = 0; i < Sides; i++)
            {
                var a1 = i*step;
                var a2 = (i + 1)*step;

                if (ShowShape)
                    DrawShape(a1, a2);

                if (ShowBorder && BorderWidth > 0)
                    DrawBorder(a1, a2);
            }
        }

        private void DrawShape(float a1, float a2)
        {
            DrawTringlePolar(a1, a2, color);

            if (Smoothing > 0)
            {
                var transparent = ToTransparent(color);
                DrawQuadPolar(a1, a2, color, transparent, 0f, Smoothing);
            }
        }

        private void DrawBorder(float a1, float a2)
        {
            DrawQuadPolar(a1, a2, BorderColor, BorderColor, 0, BorderWidth);

            if (Smoothing > 0)
            {
                var transparent = ToTransparent(BorderColor);
                DrawQuadPolar(a1, a2, transparent, BorderColor, -Smoothing, 0);
                DrawQuadPolar(a1, a2, BorderColor, transparent, BorderWidth, BorderWidth + Smoothing);
            }
        }

        private void DrawQuadPolar(float a1, float a2, Color c1, Color c2, float shift1, float shift2)
        {
            var p1 = ToCartesian(a1, shift1);
            var p2 = ToCartesian(a2, shift1);
            var p3 = ToCartesian(a2, shift2);
            var p4 = ToCartesian(a1, shift2);

            DrawQuad(p1, p2, p3, p4, c1, c2);
        }

        private void DrawTringlePolar(float a1, float a2, Color c)
        {
            var p1 = Vector2.zero;
            var p2 = ToCartesian(a1);
            var p3 = ToCartesian(a2);

            DrawQuad(p1, p1, p2, p3, c, c);
        }

        private void DrawQuad(Vector3 p1,Vector3 p2,Vector3 p3,Vector3 p4, Color c1, Color c2)
        {
            DrawVertex(p1, c1);
            DrawVertex(p2, c1);
            DrawVertex(p3, c2);
            DrawVertex(p4, c2);
        }

        private void DrawVertex(Vector3 position, Color color)
        {
            var vert = UIVertex.simpleVert;

            vert.position = position;
            vert.color = color;
            Vbo.Add(vert);
        }

        private Vector2 ToCartesian(float angle, float shift = 0)
        {
            var rect = rectTransform.rect;
            var nodeX = (rect.width*0.5f + shift) * Mathf.Cos(angle);
            var nodeY = (rect.height*0.5f + shift) * Mathf.Sin(angle);
            return new Vector2(nodeX, nodeY);
        }
    }
}
