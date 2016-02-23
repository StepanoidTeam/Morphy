using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
// ReSharper disable CheckNamespace

public static class EditorUtils
{
    public static Vector3 GetMousePosition()
    {
        var e = Event.current;
        var position = new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight);
        var r = Camera.current.ScreenPointToRay(position);
        return new Vector3(r.origin.x, r.origin.y);
    }

    public static void HideDefaultHandle()
    {
        var type = typeof(Tools);
        var field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
        field.SetValue(null, true);
    }

    public static float CapSize
    {
        get { return HandleUtility.GetHandleSize(Vector3.one) * 0.05f; }
    }

    public static bool Button(Vector3 point, Transform transform, int size, Action<int, Vector3, Quaternion, float> dotCap)
    {
        return Handles.Button(point, transform.rotation, CapSize * size, CapSize * size * 2, Handles.DotCap);
    }    
    
    public static Vector3 Move(Vector3 point, Transform transform, int size, Action<int, Vector3, Quaternion, float> dotCap)
    {
        return Handles.FreeMoveHandle(point, transform.rotation, CapSize * size, Vector3.zero, Handles.DotCap);
    }
}

