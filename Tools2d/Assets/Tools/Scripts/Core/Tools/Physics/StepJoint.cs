using System.Collections.Generic;
using Scripts.Core.Utils;
using UnityEngine;
#pragma warning disable 649

namespace Scripts.Core.Tools.Physics
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class StepJoint : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D connectedRigidbody;

        [SerializeField] private int steps;
        [SerializeField] private float distance;

        [SerializeField] private Vector2 anchor;
        [SerializeField] private Vector2 connectedAnchor;

        private List<Rigidbody2D> bodys = new List<Rigidbody2D>();
        private LineRenderer line;

        private void Start()
        {
            var step = distance/steps;
            var direction = (connectedRigidbody.transform.position - transform.position).normalized;
            
            bodys.Add(GetComponent<Rigidbody2D>());

            for (var i = 1; i < steps; i++)
            {
                var position = transform.position + i*step*direction;
                var obj = PhysUtils.CreateCircleBody(position, 0.05f, "jointnode");
                var body = obj.GetComponent<Rigidbody2D>();
                bodys.Add(body);
            }

            bodys.Add(connectedRigidbody);

            for (var i = 1; i < bodys.Count; i++)
            {
                var body = bodys[i];

                var joint = body.gameObject.AddComponent<SpringJoint2D>();
                joint.distance = step;
                joint.frequency = 2f;
                joint.connectedBody = bodys[i - 1];
            }

            line = GetComponent<LineRenderer>();
            line.SetVertexCount(bodys.Count);
        }

        private void Update()
        {
            for (var i = 0; i < bodys.Count; i++)
            {
                var body = bodys[i];
                line.SetPosition(i, body.position);
            }
        }
    }
}
