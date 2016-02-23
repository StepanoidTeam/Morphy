
using System.Collections.Generic;
using Assets.UIShapes.Scripts.Core.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Core.Tools.UIShapes
{
    public class UILine : UIShape
    {
        public List<Vector2> Positions; 
        public float Width = 10;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            vh.DrawLine(Positions, Width, color);
        }
    }
}
