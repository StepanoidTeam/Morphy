using UnityEngine;

namespace Scripts.Core.Utils
{
    public class PhysUtils
    {
        public static GameObject CreateCircleBody(Vector3 position, float radius, string name)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.position = position;
            gameObject.AddComponent<Rigidbody2D>();

            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = radius;

            return gameObject;
        }

        public static SpringJoint2D CreateSpringJoint(GameObject object1, GameObject object2, float frequency = 3, float distanceK = 1)
        {
            var joint = object1.AddComponent<SpringJoint2D>();
            joint.frequency = frequency;
            joint.connectedBody = object2.GetComponent<Rigidbody2D>();
            joint.distance = Vector3.Distance(object1.transform.position, object2.transform.position) * distanceK;

            return joint;
        }

        public static DistanceJoint2D CreateDistanceJoint(GameObject object1, GameObject object2)
        {
            var joint = object1.AddComponent<DistanceJoint2D>();
            joint.connectedBody = object2.GetComponent<Rigidbody2D>();
            joint.distance = Vector3.Distance(object1.transform.position, object2.transform.position);

            return joint;
        }

        public static HingeJoint2D CreateHingeJoint(GameObject object1, GameObject object2)
        {
            var joint = object1.AddComponent<HingeJoint2D>();
            joint.connectedBody = object2.GetComponent<Rigidbody2D>();
            joint.connectedAnchor = object1.transform.position - object2.transform.position;

            return joint;
        }

        public static SliderJoint2D CreateSliderJoint(GameObject object1, GameObject object2, float angle, bool useLimits = false, float min = 0, float max = 0)
        {
            var joint = object1.AddComponent<SliderJoint2D>();
            joint.connectedBody = object2.GetComponent<Rigidbody2D>();
            joint.connectedAnchor = object1.transform.position - object2.transform.position;
            joint.useLimits = true;
            joint.limits = new JointTranslationLimits2D { min = min, max = max };

            return joint;
        }

    }
}
