using Assets.Grass.Scripts.Grass;
using Scripts.Core.Tools.Bezier;
using Scripts.Grass;
using UnityEditor;
using UnityEngine;
// ReSharper disable PossibleNullReferenceException

[CustomEditor(typeof(BezierCurve))]  
public class BezierEditor : Editor
{
    private Transform handleTransform;
    private Quaternion handleRotation;
    private BezierCurve curve;

    private bool delete = false;
    private bool add = false;

    /*void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnScene;
    }

    private void OnScene(SceneView sceneview)
    {
        OnSceneGUI();
    }*/

    public  void OnInitialize()
    {
        curve = target as BezierCurve;

        handleTransform = curve.transform;
        handleRotation = handleTransform.rotation;
    }

    public override void OnInspectorGUI()
    {
        OnInitialize();

        GUILayout.Label("Use A and click on curve to add point");
        GUILayout.Label("Use D and click on point to delete it");
    }

    private void OnInput()
    {
        var e = Event.current;

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.A)
            add = true;

        if (e.type == EventType.KeyUp && e.keyCode == KeyCode.A)
            add = false;

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.D)
            delete = true;

        if (e.type == EventType.KeyUp && e.keyCode == KeyCode.D)
            delete = false;
    }

    private void OnSceneGUI()
    {
        OnInitialize();
        OnInput();
        
        for (var i = 1; i < curve.Points.Length; i += 3)
        {
            var p0 = ShowPoint(i - 1);
            var p1 = ShowPoint(i);
            var p2 = ShowPoint(i + 1);
            var p3 = ShowPoint(i + 2);

            if (!delete)
            {
                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);
            }

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);

            if (add)
            {
                var distance = HandleUtility.DistancePointBezier(EditorUtils.GetMousePosition(), p0, p3, p1, p2);
                if (distance < 1f * GetSize())
                    ShowAdd(EditorUtils.GetMousePosition(), i);
            }
        }
    }

    private Vector3 ShowPoint(int index)
    {
        SetColorByIndex(index);

        var point = handleTransform.TransformPoint(curve.Points[index]);
        
        if (delete)
        {
            ShowDelete(point, index);
            return point;
        }

        ShowDrag(point, index);

        return point;
    }

    private void ShowAdd(Vector3 point, int index)
    {
        if (Handles.Button(point, handleRotation, GetSize(), GetSize(), Handles.DotCap))
        {
            Undo.RecordObject(curve, "Add Curve");
            curve.AddCurve(index, point);
            Debug.Log(index);
            EditorUtility.SetDirty(target);
        }
    }

    private void ShowDrag(Vector3 point, int index)
    {
        EditorGUI.BeginChangeCheck();
        point = Handles.FreeMoveHandle(point, handleRotation, GetSize(), Vector3.zero, Handles.DotCap);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(curve, "Move Point");
            curve.Points[index] = handleTransform.InverseTransformPoint(point);
            EditorUtility.SetDirty(target);
        }
    }

    private void ShowDelete(Vector3 point, int index)
    {
        if (!IsPoint(index))
            return;

        if (Handles.Button(point, handleRotation, GetSize(), GetSize(), Handles.DotCap) 
            && curve.Points.Length > 6)
        {
            Undo.RecordObject(curve, "Delete Curve");
            curve.RemoveCurve(index);
            Repaint();
            EditorUtility.SetDirty(target);
        }
    }

    private bool IsPoint(int index)
    {
        return index%3 == 0;
    }

    private void SetColorByIndex(int index)
    {
        Handles.color = index % 3 == 0
            ? Color.white
            : Color.grey;
    }

    private float GetSize()
    {
        return HandleUtility.GetHandleSize(Vector3.one) * 0.05f;
    }
}

