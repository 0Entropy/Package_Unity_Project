using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Geometry;

[CustomEditor(typeof(PackageImporter))]
public class PackageEditor : Editor
{
    List<Vector3> keyPoints;
    List<int> keyIndices;

    Vector3 TopLeftPoint;

    Shape2D Shape;

    public override void OnInspectorGUI()
    {
        if (target == null)
            return;
        
        

        if (GUILayout.Button("Edit"))
        {

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

        if(Shape == null)
        {

            keyPoints = new List<Vector3>(imPackage.GetComponent<MeshFilter>().sharedMesh.vertices);
            keyIndices = new List<int>(imPackage.GetComponent<MeshFilter>().sharedMesh.triangles);

            Shape = new Shape2D();
            for(int i = 0; i< keyIndices.Count; i += 3)
            {

                var p0 = new Point2D(keyPoints[keyIndices[i]], "pt_" + keyIndices[i]);
                var p1 = new Point2D(keyPoints[keyIndices[i + 1]], "pt_" + keyIndices[i + 1]);
                var p2 = new Point2D(keyPoints[keyIndices[i + 2]], "pt_" + keyIndices[i + 2]);

                var face = Shape.AddPoints(p0, p1, p2);

            }
            //var outline = Shape.Outline;
            Shape.UnitTest();
            Debug.Log(string.Format("Shape : {0}" , Shape));

            /*foreach (var p in keyPoints)
            {
                if (p.x < TopLeftPoint.x)
                    TopLeftPoint = p;
                else if(p.y < TopLeftPoint.y)
                {
                    TopLeftPoint = p;
                }
            }*/
            //keyPoints.Sort((p0, p1) => p0.z < p1.z ? 1 : (p0.z == p1.z ? (p0.x < p1.x ? 1 : (p0.x == p1.x ? 0 : -1)) : -1));
            
        }

        //Load handle matrix
        Handles.matrix = imPackage.transform.localToWorldMatrix;

        Handles.color = Color.white;
        for (int i = 0; i < keyPoints.Count; i++)
        {
            //Handles.color = nearestLine == i ? Color.green : Color.white;
            DrawSegment(i);
//             if (selectedIndices.Contains(i))
//             {
//                 Handles.color = Color.green;
//                 DrawCircle(keyPoints[i], 0.08f);
//             }
//             else
//                 Handles.color = Color.white;
            DrawKeyPoint(i);

//             if (isCurve[i])
//             {
//                 Handles.color = (draggingCurve && dragIndex == i) ? Color.white : Color.blue;
//                 DrawCurvePoint(i);
//             }
        }

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

    void DrawKeyPoint(int index)
    {
        //keyPoints[index]
        Handles.DotCap(0, Shape.TopRightPoint.Position, Quaternion.identity, HandleUtility.GetHandleSize(keyPoints[index]) * 0.03f);
        //Handles.color = Color.red;
        
        Handles.Label(keyPoints[index], " " + index);
    }



    void DrawSegment(int index)
    {
        /*var from = keyPoints[index];
        var to = keyPoints[(index + 1) % keyPoints.Count];*/
        /*if (isCurve[index])
        {
            var control = Bezier.Control(from, to, curvePoints[index]);
            var count = Mathf.Ceil(1 / polyMesh.curveDetail);
            for (int i = 0; i < count; i++)
                Handles.DrawLine(Bezier.Curve(from, control, to, i / count), Bezier.Curve(from, control, to, (i + 1) / count));
        }
        else*/
            //Handles.DrawLine(from, to);

        for(int i = 0; i < keyIndices.Count; i += 3)
        {
            var p0 = keyPoints[keyIndices[i]];
            var p1 = keyPoints[keyIndices[i + 1]];
            var p2 = keyPoints[keyIndices[i + 2]];

            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p0);
        }
    }

    List<Vector2> GetOutline()
    {
        
        var indices = new List<int>(keyIndices.GetRange(0, 3));
        keyIndices.RemoveRange(0, 3);

        while(keyIndices.Count > 0)
        {
            for(int i = 0; i<keyIndices.Count; i += 3)
            {
                var b_0 = indices.Contains(i);
                var b_1 = indices.Contains(i + 1);
                var b_2 = indices.Contains(i + 2);

                if (b_0 && b_1 && b_2)
                    throw new System.Exception("Triangle's all vertices is contained!");

                /*List<Vector2> result;
                if ((b_0 && b_1 && !b_2) || (b_1 && b_2 && !b_0) || (b_2 && b_0 && !b_1))
                {
                    PolygonAlgorithm.Operate()
                }*/





            }
        }

        return null;
    }
}
