using System.Collections.Generic;
using Scripts.Core.Tools.Bezier;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Core.Tools.UIShapes
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BezierCurve))]
    public class UIBezierLine : Graphic
    {
        [SerializeField]private float width;

        private List<Vector3> positions;

        private void Update()
        {
            if(Application.isEditor)
                UpdateCurve();
        }

        private void UpdateCurve()
        {
            var besier = gameObject.GetComponent<BezierCurve>();
            positions = besier.GetCurve();
        }

        protected override void OnFillVBO(List<UIVertex> vbo)
        {
            vbo.Clear();

            if (positions == null)
                return;

            for (var i = 0; i < positions.Count - 1; i++)
            {
                if (i > 0)
                {
                    DrawStep(vbo, positions[i], GetOrto(i - 1));
                }

                var orto = GetOrto(i);

                DrawStep(vbo, positions[i], orto);
                DrawStep(vbo, positions[i + 1], -orto);

                if (i < positions.Count - 2)
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
