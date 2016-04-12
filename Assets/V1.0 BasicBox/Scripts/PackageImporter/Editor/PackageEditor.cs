using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Geometry;

[CustomEditor(typeof(Package))]
public class PackageEditor : Editor
{

    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        
    }

    void OnSceneGUI()
    {
        if (target == null)
            return;
        
        Handles.matrix = mPackage.transform.localToWorldMatrix;

        Handles.color = Color.white;

        //DrawKeyPoints();

        //DrawLine(ImPackage.Outline, new Color(0, 0.5f, 0.8f));

        DrawLine(mPackage.Bleedline, new Color(0.0f, 0.4f, 0.6f));

        foreach (Transform child in mPackage.transform)
        {
            var childPanel = child.GetComponent<Panel>();
            DrawLine(childPanel.destVertices, new Color(0, 1, 0), 4);
        }

        HandleUtility.Repaint();

    }

    Package mPackage
    {
        get { return (Package)target; }
    }

    static bool initinalSettings
    {
        get { return EditorPrefs.GetBool("PackageEditor_initinalSettings", false); }
        set { EditorPrefs.SetBool("PackageEditor_initinalSettings", value); }
    }

    static bool dimensionSettings
    {
        get { return EditorPrefs.GetBool("PackageEditor_dimensionSettings", false); }
        set { EditorPrefs.SetBool("PackageEditor_dimensionSettings", value); }
    }


    void DrawKeyPoints()
    {
        int i = 0;
        foreach (var p in mPackage.Shape.Points)
        {
            //Handles.DotCap(0, p, Quaternion.identity, HandleUtility.GetHandleSize(p) * 0.03f);
            Handles.Label(p.Position, "" + i++);
        }


    }

    void DrawBleedLine()
    {

        var outline = mPackage.Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();
        var bleedline = CGAlgorithm.ScalePoly(outline, 0.0125f);

        DrawLine(bleedline, Color.blue);

    }

    void DrawLine(IEnumerable<Vector2> points, Color32 color, float DottedSpaceSize = 0)
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

    /*void AddMesh(Mesh mesh)
    {
        
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {

            var p0 = new Point2D(mesh.vertices[mesh.triangles[i]]);
            var p1 = new Point2D(mesh.vertices[mesh.triangles[i + 1]]);
            var p2 = new Point2D(mesh.vertices[mesh.triangles[i + 2]]);

            var face = Shape.AddPoints(p0, p1, p2);
        }
    }*/
}
