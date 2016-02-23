using Assets.Tools.Scripts.Wind;
using Scripts.Core.Tools.Bezier;
using Scripts.Core.Tools.Render;
using UnityEngine;

#pragma warning disable 649

namespace Scripts.Grass
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(GrassMesh))]
    public class Grass : MonoBehaviour
    {
        [SerializeField]private Curve curve;
        [SerializeField]private WindField wind;

        private GrassMesh line;

        private void Start()
        {
            line = GetComponent<GrassMesh>();
        }

        private void Update()
        {
            if(!Application.isEditor)
                return;

            var points = curve.GetCurve();
            line.SetVertexCount(points.Count);
            line.Field = wind;

            for (int i = 0; i < points.Count; i++)
            {
                line.SetPosition(i, points[i]);
            }
        }
    }
}
