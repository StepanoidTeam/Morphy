using System;
using UnityEngine;

namespace Scripts.Core.Tools.DotShape
{
    public class Dot : MonoBehaviour
    {
        public event Action<Collision2D> CollisionEnterEvent;
        public event Action<Collision2D> CollisionExitEvent;
        public event Action<Collision2D> CollisionStayEvent; 

        public void OnCollisionEnter2D(Collision2D coll)
        {
            CollisionEnterEvent(coll);
        }

        public void OnCollisionExit2D(Collision2D coll)
        {
            CollisionExitEvent(coll);
        }        
        
        public void OnCollisionStay2D(Collision2D coll)
        {
            CollisionStayEvent(coll);
        }
    }
}
