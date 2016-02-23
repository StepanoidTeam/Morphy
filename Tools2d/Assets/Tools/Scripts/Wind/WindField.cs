using UnityEngine;

namespace Assets.Tools.Scripts.Wind
{
    public class WindField : MonoBehaviour
    {
        private float wind = 0.13f;
        private float wind2 = 0.12f;
        private float wind3 = 0.17f;

        public Vector3 GetWind(Vector3 position)
        {
            return new Vector3(Mathf.Cos(wind + position.x*20) * 0.1f, 0, 0);
        }

        private void Update()
        {
            wind3 += 0.0015f;
            wind2 += Mathf.Cos(wind3)*0.001f;
            wind += Mathf.Cos(wind2)*0.05f;
        }
    }
}
