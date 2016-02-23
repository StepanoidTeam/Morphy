using Scripts.Core.Tools.Bezier;
using UnityEngine;
#pragma warning disable 649

namespace Scripts.Core.Tools.Physics
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(EdgeCollider2D))]
    public class CurveEdge : MonoBehaviour
    {
        [SerializeField] private Curve curve;
        private EdgeCollider2D edge;

        private void Start()
        {
            edge = GetComponent<EdgeCollider2D>();
            UpdatePoints();
        }

        private void Update()
        {
            if (!Application.isEditor || curve == null)
                return;

            UpdatePoints();
        }

        private void UpdatePoints()
        {
            var points = curve.GetCurve();
            var test = new Vector2[points.Count];

            for (var i = 0; i < points.Count; i++)
            {
                test[i] = points[i];
            }

            edge.points = test;
        }
    }
}
