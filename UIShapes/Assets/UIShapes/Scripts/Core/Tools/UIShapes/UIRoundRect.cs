using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Core.Tools.Render;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Core.Tools.UIShapes
{
    public class UIRoundRect : UIShape
    {
        public float GroupRadius = 10;
        [HideInInspector]public float GroupRadiusInternal = 10;
        
        public float RadiusUpLeft = 10;
        public float RadiusUpRight = 10;
        public float RadiusBottomRight = 10;
        public float RadiusBottomLeft = 10;

        [Range(1,50)]public int Steps = 10;

        private List<UIVertex> list = new List<UIVertex>();

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            var center = (BottomLeft + UpRight) * 0.5f;
            var size = new Vector2(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;

            Draw(vh, center, size);
        }

        public void Draw(VertexHelper vh, Vector2 center, Vector2 size)
        {
            vh.Clear();
            list.Clear();

            AddVertex(center);

            AddArc(new Vector2(size.x - RadiusUpRight, size.y - RadiusUpRight), RadiusUpRight, 0);
            AddArc(new Vector2(-size.x + RadiusUpLeft, size.y - RadiusUpLeft), RadiusUpLeft, Mathf.PI * 0.5f);
            AddArc(new Vector2(-size.x + RadiusBottomLeft, -size.y + RadiusBottomLeft), RadiusBottomLeft, Mathf.PI);
            AddArc(new Vector2(size.x - RadiusBottomRight, -size.y + RadiusBottomRight), RadiusBottomRight, Mathf.PI*1.5f);
            
            AddVertex(new Vector2(size.x, size.y - RadiusUpRight));

            vh.AddUIVertexStream(list, ShapeTopology.TringleFan((Steps + 1)*4).ToList());
        }

        private void AddVertex(Vector2 position)
        {
            var vertex = UIVertex.simpleVert;
            vertex.position = position;
            vertex.uv0 = new Vector2(0.5f + position.x / rectTransform.rect.width, 0.5f + position.y / rectTransform.rect.height);
            vertex.color = color;

            list.Add(vertex);
        }

        private void AddArc(Vector3 center, float radius, float from)
        {
            var step = Mathf.PI*0.5f/Steps;

            for (var i = 0; i < Steps + 1; i++)
            {
                var a = from + i*step;
                AddSegment(center, radius, a);
            }
        }

        private void AddSegment(Vector3 center, float radius, float angle)
        {
            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);

            var x = cos * radius + center.x;
            var y = sin * radius + center.y;

            AddVertex(new Vector2(x, y));
        }

        private void Update()
        {
            if (Math.Abs(GroupRadiusInternal - GroupRadius) > 0.01f)
            {
                GroupRadiusInternal = GroupRadius;
                RadiusBottomLeft = GroupRadius;
                RadiusBottomRight = GroupRadius;
                RadiusUpLeft = GroupRadius;
                RadiusUpRight = GroupRadius;
            }
        }

    }
}
