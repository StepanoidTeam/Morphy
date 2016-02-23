using System.Collections.Generic;
using Scripts.Core.Tools.Bezier;
using Scripts.Core.Tools.DotShape;
using Scripts.Core.Tools.Render;
using UnityEngine;

namespace Scripts.DotCircle
{
    [RequireComponent(typeof(DropConfig))]
    [RequireComponent(typeof(CircleMesh))]

    public class Drop : MonoBehaviour
    {
        [SerializeField] private int totalNodes = 8;
        [SerializeField] private int totalSegments = 3;

        private CircleMesh mesh;
        private CircleBezier bezier;

        public List<Dot> Nodes { get; private set; }
        public DotCollider DotCollider { get; private set; }
        public DotRigidBody DotRigidBody { get; private set; }

        private void Awake()
        {
            Nodes = new DotFactory().CreateCircle(transform.position, 0.3f, totalNodes);

            DotCollider = new DotCollider(Nodes);
            DotRigidBody = new DotRigidBody(Nodes);

            mesh = GetComponent<CircleMesh>();
            mesh.SetVertexCount(totalNodes * totalSegments);

            bezier = new CircleBezier();
            bezier.SetVertexCount(totalNodes);
        }

        private void FixedUpdate()
        {
            ApplyPosition();
            CalculateBezier();
            ApplyMeshData();
        }

        private void ApplyPosition()
        {
            transform.position = Position;
        }

        private void CalculateBezier()
        {
            for (var i = 0; i < totalNodes; i++)
            {
                bezier.AddPosition(Nodes[i].transform.position - transform.position);
            }

            bezier.CalculateBezierPath(totalSegments);
        }

        private void ApplyMeshData()
        {
            for (var i = 0; i < totalNodes * totalSegments; i++)
            {
                mesh.SetPosition(i, bezier.Result[i]);
            }
        }

        public Vector3 Position
        {
            get { return DotRigidBody.Position; }
        }
    }
}
