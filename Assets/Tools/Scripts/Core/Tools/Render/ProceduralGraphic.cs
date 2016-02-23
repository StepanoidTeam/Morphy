using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Core.Tools.Render
{
    public abstract class ProceduralGraphic : MaskableGraphic
    {
        public abstract void SetVertexCount(int count);

        public abstract void SetPosition(int index, Vector3 position);
    }
}
