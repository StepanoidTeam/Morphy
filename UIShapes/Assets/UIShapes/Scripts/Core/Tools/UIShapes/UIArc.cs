using Assets.UIShapes.Scripts.Core.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Core.Tools.UIShapes
{
    public class UIArc : UIShape
    {
        [Range(3, 50)]
        public int Sides = 10;

        [Range(0, Mathf.PI*2)] public float From;        
        [Range(0, Mathf.PI*2)] public float To = Mathf.PI*0.5f;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            var center = (BottomLeft + UpRight) * 0.5f;
            var size = new Vector2(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;

            vh.DrawArc(center, size, To, From, Sides, color);
        }
    }
}
