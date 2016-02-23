using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Core.Tools.UIShapes
{
    [ExecuteInEditMode]
    public class UIShape : Graphic
    {
        public Sprite Sprite;

        protected Vector2 BottomLeft;
        protected Vector2 UpRight;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            BottomLeft = new Vector2(0f, 0f);
            UpRight = new Vector2(1f, 1f);

            BottomLeft.x -= rectTransform.pivot.x;
            BottomLeft.y -= rectTransform.pivot.y;
            UpRight.x -= rectTransform.pivot.x;
            UpRight.y -= rectTransform.pivot.y;

            BottomLeft.x *= rectTransform.rect.width;
            BottomLeft.y *= rectTransform.rect.height;
            UpRight.x *= rectTransform.rect.width;
            UpRight.y *= rectTransform.rect.height;
        }

        public override Texture mainTexture
        {
            get { return Sprite == null ? s_WhiteTexture : Sprite.texture; }
        }
    }
}
