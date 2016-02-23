using Scripts.Core.Tools.UIShapes;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor
{
    public class MenuItems : MonoBehaviour
    {
        [MenuItem("GameObject/VectorShapes/Arc")]
        public static void CreateArc()
        {
            var obj = new GameObject("Arc");
            obj.AddComponent<UIArc>();
        }            
        
        [MenuItem("GameObject/VectorShapes/ArcBorder")]
        public static void CreateArcBorder()
        {
            var obj = new GameObject("Arc");
            obj.AddComponent<UIArcBorder>();
        }         
        
        [MenuItem("GameObject/VectorShapes/Circle")]
        public static void CreateCircle()
        {
            var obj = new GameObject("Circle");
            obj.AddComponent<UICircle>();
        }            
        
        [MenuItem("GameObject/VectorShapes/CircleBorder")]
        public static void CreateCircleBorder()
        {
            var obj = new GameObject("CircleBorder");
            obj.AddComponent<UICircleBorder>();
        }        
              
        [MenuItem("GameObject/VectorShapes/Line")]
        public static void CreateLine()
        {
            var obj = new GameObject("Line");
            obj.AddComponent<UILine>();
        }        
        
        [MenuItem("GameObject/VectorShapes/EditableLine")]
        public static void CreateEditableLine()
        {
            var obj = new GameObject("Editable line");
            obj.AddComponent<UIEditableLine>();
        }        
        
        [MenuItem("GameObject/VectorShapes/RoundRect")]
        public static void CreateRoundRect()
        {
            var obj = new GameObject("Round Rect");
            obj.AddComponent<UIRoundRect>();
        }        

        [MenuItem("GameObject/VectorShapes/RoundRectBorder")]
        public static void CreateRoundRectBorder()
        {
            var obj = new GameObject("Round Rect Border");
            obj.AddComponent<UIRoundRectBorder>();
        }
    }
}
