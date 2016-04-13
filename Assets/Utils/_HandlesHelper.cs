using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class _HandlesHelper {

    public static void DrawLine(IEnumerable<Vector2> points, Color32 color, float DottedSpaceSize = 0)
    {
        if (points == null || points.Count() == 0)
            return;

        Handles.color = color;

        //var first = points.First();
        //var end = points.Last();

        var current = points.Last();

        foreach (var next in points)
        {
            if (DottedSpaceSize > 0)
                Handles.DrawDottedLine(current, next, DottedSpaceSize);
            else
                Handles.DrawLine(current, next);

            current = next;
        }

        //Handles.DrawLine(end, first);
    }

    public static void DrawCircle(Vector3 position,  float size = 0.05F)
    {
        Handles.CircleCap(0, position, Quaternion.identity, HandleUtility.GetHandleSize(position) * size);
    }
}
