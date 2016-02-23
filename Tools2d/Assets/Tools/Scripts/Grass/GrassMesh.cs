using Assets.Tools.Scripts.Wind;
using Scripts.Core.Tools.Render;
using UnityEngine;

namespace Scripts.Grass
{
    public class GrassMesh : LineMesh
    {
        public WindField Field; 

        protected override void CalculateVertices()
        {
            if (Positions == null)
                return;

            Vertices = new Vector3[Positions.Length * 2];

            for (var i = 0; i < Positions.Length; i++)
            {
                Vertices[i * 2] = Positions[i];

                var actual = Positions[i] + Vector3.up * Width;

                Vertices[i*2 + 1] = actual + Field.GetWind(Positions[i]);
            }

            Mesh.vertices = Vertices;
        }
    }
}
