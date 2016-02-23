using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Core.Tools.DotShape
{
    public class DotCollider
    {
        public event Action<Collision2D> CollisionEnterEvent;
        public event Action<Collision2D> CollisionExitEvent;
        public event Action<Collision2D> CollisionStayEvent; 

        private List<Dot> list;

        public DotCollider(List<Dot> list)
        {
            this.list = list;

            foreach (var dot in list)
            {
                dot.CollisionEnterEvent += OnCollisionEnter;
                dot.CollisionStayEvent += OnCollisionStay;
                dot.CollisionExitEvent += OnCollisionExit;
            }
        }

        private void OnCollisionEnter(Collision2D obj)
        {
            //CollisionEnterEvent(obj);
        }        
        
        private void OnCollisionStay(Collision2D obj)
        {
            //CollisionStayEvent(obj);
        }
        
        private void OnCollisionExit(Collision2D obj)
        {
            //CollisionExitEvent(obj);
        }

        public bool IsTrigger
        {
            set
            {
                foreach (var collider in list.Select(dot => dot.GetComponent<Collider2D>()))
                {
                    collider.isTrigger = value;
                }
            }
        }

        public PhysicsMaterial2D Material
        {
            set
            {
                foreach (var collider in list.Select(dot => dot.GetComponent<Collider2D>()))
                {
                    collider.sharedMaterial = value;
                }
            }
        }
    }
}
