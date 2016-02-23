using Scripts.Core.Tools.UIShapes;
using UnityEditor;
using UnityEngine;

// ReSharper disable CheckNamespace

[CustomEditor(typeof (UIEditableLine))]
public class CurveEditor : Editor
{
	private UIEditableLine curve;
    private EditorInput input = new EditorInput();

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Use A and click to add point");
        GUILayout.Label("Use D and click on point to delete it");
    
		DrawDefaultInspector ();
	}

    private void OnSceneGUI()
    {
		curve = target as UIEditableLine;
        input.Update();

        for (var i = 1; i < curve.Positions.Count; i++)
        {
            Handles.color = Color.gray;
            Handles.DrawLine(Get(i - 1), Get(i));
        }

        for (var i = 0; i < curve.Positions.Count; i++)
        {
            DrawDrag(i);

            if(input.IsKey(KeyCode.D))
                DrawDelete(i);
        }

        if(input.IsKey(KeyCode.A))
            DrawAdd();
    }

    private void DrawAdd()
    {
        var m = EditorUtils.GetMousePosition();
        int a;
        int b;

        GetNearestLine(out a, out b);

        Handles.color = Color.yellow;
        Handles.DrawLine(m, Get(a));
        Handles.DrawLine(m, Get(b));

        if (EditorUtils.Button(m, curve.transform, 1, Handles.DotCap))
        {
            Undo.RecordObject(curve, "Add Point");
            var p = curve.transform.InverseTransformPoint(m);
            curve.Positions.Insert(Mathf.Max(a, b), p);
            EditorUtility.SetDirty(curve);
            curve.SetAllDirty();
        }
    }

    private void DrawDrag(int i)
    {
        EditorGUI.BeginChangeCheck();

        var p = Get(i);
        p = EditorUtils.Move(p, curve.transform, 1, Handles.DotCap);
        Set(i, p);
    }

    private void DrawDelete(int i)
    {
        var p = Get(i);

        Handles.color = Color.red;
        if (EditorUtils.Button(p, curve.transform, 1, Handles.DotCap))
        {
            Undo.RecordObject(curve, "Delete Point");
            curve.Positions.RemoveAt(i);
            EditorUtility.SetDirty(curve);
			Repaint();
            curve.SetAllDirty();
        }
    }

    private void GetNearestLine(out int a, out int b)
    {
        var points = curve.Positions;
        var result = 0;
        var m = EditorUtils.GetMousePosition();
        var d = 1000f;

        for (int i = 1; i < points.Count; i++)
        {
            var newD = HandleUtility.DistancePointLine(m, Get(i - 1), Get(i));
            if (newD < d)
            {
                d = newD;
                result = i;
            }
        }

        a = result;
        b = result - 1;
    }

    private Vector3 Get(int i)
    {
        var p = curve.Positions[i];
        return curve.transform.TransformPoint(p);
    }

    private void Set(int i, Vector3 p)
    {
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(curve, "Move Point");
            curve.Positions[i] = curve.transform.InverseTransformPoint(p);
            EditorUtility.SetDirty(curve);
            curve.SetAllDirty();
			Repaint();
        }
    }
}

