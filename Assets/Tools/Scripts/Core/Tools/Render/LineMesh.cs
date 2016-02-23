using UnityEngine;

namespace Scripts.Core.Tools.Render
{
    [ExecuteInEditMode]
    public class LineMesh : ProceduralMesh
    {
        [SerializeField]protected Vector3[] Positions;
        [SerializeField]protected float Width;

        protected Vector3[] Vertices;

        public override void SetVertexCount(int count)
        {
            Positions = new Vector3[count];
        }

        public override void SetPosition(int index, Vector3 position)
        {
            Positions[index] = position;
        }

        protected override void CalculateVertices()
        {
            if(Positions == null)
                return;

            Vertices = new Vector3[Positions.Length*2];

            for (var i = 0; i < Positions.Length; i++)
            {
                Vertices[i*2] = Positions[i];
                Vertices[i * 2 + 1] = Positions[i] + Vector3.up * Width;
            }

            Mesh.vertices = Vertices;
        }

        protected override void CalculateNormals()
        {

        }

        protected override void CalculateUv()
        {
            var uv = new Vector2[Vertices.Length];
            var b = false;

            for (var i = 0; i < uv.Length; i += 2)
            {
                b = !b;
                var x = b ? 1 : 0;

                uv[i] = new Vector2(x, 0);
                uv[i + 1] = new Vector2(x, 1);
            }

            Mesh.uv = uv;
        }

        protected override void CalculateTringles()
        {
            Mesh.triangles = ShapeTopology.TringleStrip(Positions.Length * 2 - 2);
        }
    }
}
