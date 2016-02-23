using UnityEngine;

namespace Assets.Grass.Scripts.Grass
{
    public static class EditorUtils
    {
        public static Vector3 GetMousePosition()
        {
            var e = Event.current;
            var r = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
            return new Vector3(r.origin.x, r.origin.y);
        }        
    }
}
