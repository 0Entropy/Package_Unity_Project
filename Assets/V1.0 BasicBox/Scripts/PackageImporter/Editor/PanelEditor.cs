using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;

[CustomEditor(typeof(PanelImporter))]
public class PanelEditor : Editor
{

    //Shape2D Shape;

    public override void OnInspectorGUI()
    {
        if (target == null)
            return;
    }

    void OnSceneGUI()
    {
        if (target == null)
            return;



        DrawLine(ImPanel.Shape.OutlinePoints.Select(p => (Vector2)p.Position), Color.red);
    }

    public PanelImporter ImPanel
    {
        get
        {
            return (PanelImporter)target;
        }
    }

    void DrawLine(IEnumerable<Vector2> points, Color32 color, float DottedSpaceSize = 0)
    {
        Handles.color = color;

        var current = points.Last();

        foreach (var next in points)
        {
            if (DottedSpaceSize > 0)
                Handles.DrawDottedLine(current, next, DottedSpaceSize);
            else
                Handles.DrawLine(current, next);

            current = next;
        }
    }

}
