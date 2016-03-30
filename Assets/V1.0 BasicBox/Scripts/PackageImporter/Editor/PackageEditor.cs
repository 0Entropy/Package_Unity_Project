using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Geometry;

[CustomEditor(typeof(PackageImporter))]
public class PackageEditor : Editor
{
    //Shape2D Shape;

    bool isFirst = true;

    public override void OnInspectorGUI()
    {
        if (target == null)
            return;



        if (GUILayout.Button("Reset"))
        {

            Debug.Log(imPackage.Shape);


            isFirst = false;

            var shape = imPackage.Shape;

            DrawKeyPoints();
            
            //imPackage.Shape.LogOutline();
        }

        if (dimensionSettings = EditorGUILayout.Foldout(dimensionSettings, "Dimension (mm)"))
        {
            var dimension = string.Format("Length : {0}, Width : {1}, Depth : {2}, Thickness : {3}",
                imPackage.Length, imPackage.Width, imPackage.Depth, imPackage.Thickness);

            var whatever = EditorGUILayout.TextArea(dimension);
            var whatever_0 = EditorGUILayout.TextField(whatever);

            if (GUI.changed)
            {

            }
        }
    }

    void OnSceneGUI()
    {
        if (target == null)
            return;

        //Debug.Log("Package Outline Point count : " + imPackage.Shape.OutlinePoints.Count);

        //Load handle matrix
        Handles.matrix = imPackage.transform.localToWorldMatrix;

        Handles.color = Color.white;

        //DrawKeyPoints();

        DrawLine(imPackage.Shape.OutlinePoints.Select(p=>(Vector2)p.Position), Color.red, 5);

        //DrawBleedLine();

        /*Debug.Log(imPackage.Shape.AllPoints[11]);
        Debug.Log(imPackage.Shape.AllPoints[12]);*/

        /*foreach (var e in imPackage.Shape.AllEdges)
        {
            Debug.Log(e);
        }*/



    }

    PackageImporter imPackage
    {
        get { return (PackageImporter)target; }
    }

    static bool dimensionSettings
    {
        get { return EditorPrefs.GetBool("PackageEditor_dimensionSettings", false); }
        set { EditorPrefs.SetBool("PackageEditor_dimensionSettings", value); }
    }


    void DrawKeyPoints()
    {
        int i = 0;
        foreach (var p in imPackage.Shape.Points)
        {
            //Handles.DotCap(0, p, Quaternion.identity, HandleUtility.GetHandleSize(p) * 0.03f);
            Handles.Label(p.Position, "" + i++);
        }


    }

    void DrawBleedLine()
    {

        var outline = imPackage.Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();
        var bleedline = CGAlgorithm.ScalePoly(outline, 0.0125f);

        DrawLine(bleedline, Color.blue);

    }

    void DrawLine(IEnumerable<Vector2> points, Color32 color, float DottedSpaceSize = 0)
    {
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
