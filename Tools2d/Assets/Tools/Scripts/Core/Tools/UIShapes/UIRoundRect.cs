using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core.Tools.UIShapes
{
    public class UIRoundRect : UIShape
    {
        public Color BorderColor;

        public bool ShowShape = true;
        public bool ShowBorder = true;

        public float Radius = 10;
        public float BorderWidth = 5;

        [Range(0, 10)] public int Smoothing = 1;
        [Range(1, 50)] public int Segments = 4;

        protected override void OnFillVBO(List<UIVertex> vbo)
        {
            base.OnFillVBO(vbo);

            if (ShowShape)
            {
                DrawCenter();
                DrawRoundPart();
            }

            if(ShowBorder)
                DrawBorder();
        }

        private void DrawCenter()
        {
            DrawRectMargin(Radius, Radius, Radius, Radius, color);
        }

        private void DrawRoundPart()
        {
            var rect = rectTransform.rect;

            DrawRectMargin(Radius, Radius, 0, rect.width - Radius, color);
            DrawRectMargin(Radius, Radius, rect.width - Radius, 0, color);
            DrawRectMargin(0, rect.height - Radius, Radius, Radius, color);
            DrawRectMargin(rect.height - Radius, 0, Radius, Radius, color);

            var corner1 = Corner1 + Vector2.one * Radius;
            var corner2 = Corner2 - Vector2.one * Radius;
            var corner3 = new Vector2(Corner1.x, Corner2.y) + new Vector2(Radius, -Radius);
            var corner4 = new Vector2(Corner2.x, Corner1.y) + new Vector2(-Radius, Radius);

            DrawArc(corner1, Radius, Mathf.PI);
            DrawArc(corner2, Radius, 0);
            DrawArc(corner3, Radius, Mathf.PI * 0.5f);
            DrawArc(corner4, Radius, 3 * Mathf.PI * 0.5f);

            DrawUnderLine(0, Smoothing, color, ToTransparent(color));
        }

        private void DrawArc(Vector2 center, float radius, float start)
        {
            const float angle = Mathf.PI*0.5f;
            DrawArc(center, radius, start, angle, Segments, color);
        }

        private void DrawBorder()
        {
            var transparent = ToTransparent(BorderColor);

            DrawUnderLine(0, BorderWidth, BorderColor, BorderColor);
            DrawUnderLine(0, -Smoothing, BorderColor, transparent);
            DrawUnderLine(BorderWidth, BorderWidth + Smoothing, BorderColor, transparent);
        }

        private void DrawUnderLine(float from, float to, Color c1, Color c2)
        {
            var rect = rectTransform.rect;

            DrawRectMargin(Radius, Radius, -from, rect.width + to, c1, c1, c2, c2);
            DrawRectMargin(Radius, Radius, rect.width + to, -from, c2, c2, c1, c1);
            DrawRectMargin(-from, rect.height + to, Radius, Radius, c2, c1, c1, c2);
            DrawRectMargin(rect.height + to, -from, Radius, Radius, c1, c2, c2, c1);

            const float angle = Mathf.PI*0.5f;

            var corner1 = Corner1 + Vector2.one*Radius;
            var corner2 = Corner2 - Vector2.one*Radius;
            var corner3 = new Vector2(Corner1.x, Corner2.y) + new Vector2(Radius, -Radius);
            var corner4 = new Vector2(Corner2.x, Corner1.y) + new Vector2(-Radius, Radius);

            var radiusFrom = Radius + from;
            var radiusTo = Radius + to;

            DrawArc3(corner2, radiusFrom, radiusTo, 0, angle, Segments, c1, c2);
            DrawArc3(corner3, radiusFrom, radiusTo, angle, angle, Segments, c1, c2);
            DrawArc3(corner1, radiusFrom, radiusTo, 2*angle, angle, Segments, c1, c2);
            DrawArc3(corner4, radiusFrom, radiusTo, 3*angle, angle, Segments, c1, c2);
        }

        private void DrawRectMargin(float top, float bottom, float right, float left, Color color)
        {
            DrawRectMargin(top, bottom, right, left, color, color, color, color);
        }

        private void DrawRectMargin(float top, float bottom, float right, float left, Color c1, Color c2, Color c3, Color c4)
        {
            var c1X = Corner1.x + right;
            var c1Y = Corner1.y + bottom;
            var c2X = Corner2.x - left;
            var c2Y = Corner2.y - top;

            DrawVertex(new Vector2(c1X, c1Y), c1);
            DrawVertex(new Vector2(c1X, c2Y), c2);
            DrawVertex(new Vector2(c2X, c2Y), c3);
            DrawVertex(new Vector2(c2X, c1Y), c4);
        }
    }
}
