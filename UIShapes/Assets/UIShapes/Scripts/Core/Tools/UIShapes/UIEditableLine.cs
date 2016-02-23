using System.Collections.Generic;
using Assets.UIShapes.Scripts.Core.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Core.Tools.UIShapes
{
    [ExecuteInEditMode]
    public class UIEditableLine : UIShape
    {
        public List<Vector2> Positions = new List<Vector2>() {new Vector3(), new Vector3(100,0,0)};
        public float Width = 10;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            vh.DrawLine(Positions, Width, color);
        }
    }
}
