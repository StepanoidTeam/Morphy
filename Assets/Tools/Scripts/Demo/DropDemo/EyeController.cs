using Scripts.DotCircle;
using UnityEngine;

namespace Scripts.Demo.DropDemo
{
    [RequireComponent(typeof(Drop))]
    public class EyeController : MonoBehaviour
    {
        private Drop drop;

        private void Awake()
        {
            drop = GetComponent<Drop>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                drop.DotRigidBody.AddForce(Vector3.up * 200, ForceMode2D.Force);
            }            
            
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                drop.DotRigidBody.AddForce(Vector3.left * 3, ForceMode2D.Force);
            }     
       
            if (Input.GetKey(KeyCode.RightArrow))
            {
                drop.DotRigidBody.AddForce(Vector3.right * 3, ForceMode2D.Force);
            }


        }
    }
}
