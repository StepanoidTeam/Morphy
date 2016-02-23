using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core.Tools.Bezier
{
    public abstract class Curve : MonoBehaviour
    {
        public abstract List<Vector3> GetCurve();
    }
}
