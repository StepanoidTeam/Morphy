using UnityEngine;

// ReSharper disable ConvertToConstant.Local
#pragma warning disable 649

namespace Scripts.DotCircle
{
    [RequireComponent(typeof(Drop))]
    public class DropConfig : MonoBehaviour
    {
        [SerializeField] private float mass = 1;
        [SerializeField] private float drag = 0;
        [SerializeField] private float angularDrag = 0.1f;
        [SerializeField] private float gravityScale = 1f;
        [SerializeField] private bool fixedAngle = false;
        [SerializeField] private bool isKinematic = false;
        [SerializeField] private RigidbodySleepMode2D sleepMode = RigidbodySleepMode2D.StartAwake;
        [SerializeField] private RigidbodyInterpolation2D interpolation = RigidbodyInterpolation2D.None;
        [SerializeField] private CollisionDetectionMode2D collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        [SerializeField] private bool isTrigger = false;
        [SerializeField] private PhysicsMaterial2D material;

        private void Start()
        {
            var drop = GetComponent<Drop>();

            drop.DotRigidBody.Mass = mass;
            drop.DotRigidBody.Drag = drag;
            drop.DotRigidBody.AngularDrag = angularDrag;
            drop.DotRigidBody.GravityScale = gravityScale;
            drop.DotRigidBody.FixedAngle = fixedAngle;
            drop.DotRigidBody.IsKinamatic = isKinematic;
            drop.DotRigidBody.SleepMode = sleepMode;
            drop.DotRigidBody.Interpolation = interpolation;
            drop.DotRigidBody.CollisionDetectionMode = collisionDetectionMode;
            
            drop.DotCollider.IsTrigger = isTrigger;
            drop.DotCollider.Material = material;
        }
    }
}
