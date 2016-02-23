using Assets.UIShapes.Scripts.Core.Utils;
using Scripts.Core.Tools.UIShapes;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Core.Tools.UIShapes
{
    public class UICircleBorder : UIShape
    {
        [Range(3, 50)]
        public int Sides = 10;

        public float Width = 10;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            var center = (BottomLeft + UpRight) * 0.5f;
            var size = new Vector2(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;

            vh.DrawCone(center, size, Width, 0, Mathf.PI * 2, Sides, color);
        }
    }
}
