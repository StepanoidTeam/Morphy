using Scripts.Core.Utils;
using UnityEngine;

namespace Scripts.Core.Tools.Render
{
    public class CircleMesh : ProceduralMesh
    {
        private int count;
        private Vector3[] vertices;

        public override void SetVertexCount(int count)
        {
            this.count = count;

            vertices = new Vector3[count + 2];
            vertices[0] = Vector3.zero;
        }

        public override void SetPosition(int index, Vector3 position)
        {
            vertices[index + 1] = position;
        }        

        protected override void CalculateVertices()
        {
            vertices[count + 1] = vertices[1];
            Mesh.vertices = vertices;
        }

        protected override void CalculateTringles()
        {          
            Mesh.triangles = ShapeTopology.TringleFan(count);
        }

        protected override void CalculateUv()
        {
            var uv = new Vector2[count + 2];
            uv[0] = new Vector2(0.5f, 0.5f);

            var step = Mathf.PI * 2 / count;

            for (var i = 0; i < count + 1; i++)
            {
                uv[i + 1] = Math2Utils.ToCartesian(uv[0], step * i, -0.5f);
            }

            Mesh.uv = uv;
        }

        protected override void CalculateNormals()
        {
            var normals = new Vector3[count + 2];

            for (var i = 0; i < count + 2; i++)
            {
                normals[i] = Vector3.forward;
            }

            Mesh.normals = normals;
        }
    }
}
