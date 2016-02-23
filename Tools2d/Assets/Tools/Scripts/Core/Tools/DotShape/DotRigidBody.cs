using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Core.Tools.DotShape
{
    public class DotRigidBody
    {
        private List<Dot> list;

        public DotRigidBody(List<Dot> list)
        {
            this.list = list;
        }

        public Vector3 Position
        {
            get
            {
                var sum = Vector3.zero;

                foreach (var dot in list)
                    sum += dot.transform.position;

                return sum / list.Count;
            }
        }

        public float Mass
        {
            set
            {
                foreach (var body in list.Select(dot => dot.GetComponent<Rigidbody2D>()))
                {
                    body.mass = value;
                }
            }
        }

        public float Drag
        {
            set
            {
                foreach (var body in list.Select(dot => dot.GetComponent<Rigidbody2D>()))
                {
                    body.drag = value;
                }
            }
        }

        public float AngularDrag
        {
            set
            {
                foreach (var body in list.Select(dot => dot.GetComponent<Rigidbody2D>()))
                {
                    body.angularDrag = value;
                }
            }
        }

        public float GravityScale
        {
            set
            {
                foreach (var body in list.Select(dot => dot.GetComponent<Rigidbody2D>()))
                {
                    body.gravityScale = value;
                }
            }
        }


        public bool FixedAngle
        {
            set
            {
                foreach (var body in list.Select(dot => dot.GetComponent<Rigidbody2D>()))
                {
                    body.fixedAngle = value;
                }
            }
        }

        public bool IsKinamatic
        {
            set
            {
                foreach (var body in list.Select(dot => dot.GetComponent<Rigidbody2D>()))
                {
                    body.isKinematic = value;
                }
            }
        }

        public CollisionDetectionMode2D CollisionDetectionMode
        {
            set
            {
                foreach (var body in list.Select(dot => dot.GetComponent<Rigidbody2D>()))
                {
                    body.collisionDetectionMode = value;
                }
            }
        }

        public RigidbodySleepMode2D SleepMode
        {
            set
            {
                foreach (var body in list.Select(dot => dot.GetComponent<Rigidbody2D>()))
                {
                    body.sleepMode = value;
                }
            }
        }        
        
        public RigidbodyInterpolation2D Interpolation
        {
            set
            {
                foreach (var body in list.Select(dot => dot.GetComponent<Rigidbody2D>()))
                {
                    body.interpolation = value;
                }
            }
        }

        public void AddForce(Vector3 force, ForceMode2D mode)
        {
            foreach (var body in list.Select(dot => dot.GetComponent<Rigidbody2D>()))
            {
                body.AddForce(force, mode);
            }
        }
    }
}
