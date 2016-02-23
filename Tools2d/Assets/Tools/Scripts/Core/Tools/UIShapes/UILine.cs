using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

namespace Scripts.Core.Tools.UIShapes
{
    public class UILine : MaskableGraphic
    {
        [SerializeField]private Vector3[] positions;
        [SerializeField]private float width;

        public void SetVertexCount(int count)
        {
            positions = new Vector3[count];
        }

        public void SetPosition(int index, Vector3 position)
        {
            positions[index] = position;
        }

        protected override void OnFillVBO(List<UIVertex> vbo)
        {
            vbo.Clear();

            if(positions == null)
                return;

            for (var i = 0; i < positions.Length - 1; i++)
            {
                if (i > 0)
                {
                    DrawStep(vbo, positions[i], GetOrto(i - 1));
                }

                var orto = GetOrto(i);

                DrawStep(vbo, positions[i], orto);
                DrawStep(vbo, positions[i + 1], -orto);

                if (i < positions.Length - 2)
                {
                    DrawStep(vbo, positions[i + 1], -GetOrto(i + 1));
                }
            }
        }

        private void DrawStep(List<UIVertex> vbo, Vector3 v, Vector3 orto)
        {
            var vert = UIVertex.simpleVert;

            vert.position = v + orto;
            vert.color = color;
            vbo.Add(vert);

            vert.position = v - orto;
            vert.color = color;
            vbo.Add(vert);
        }

        private Vector3 GetOrto(int i)
        {
            var dir = (positions[i] - positions[i + 1]).normalized;
            return new Vector3(-dir.y, dir.x) * width;
        }
    }
}
