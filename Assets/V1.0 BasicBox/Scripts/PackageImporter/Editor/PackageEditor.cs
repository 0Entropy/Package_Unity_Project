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

        _HandlesHelper.DrawLine(mPackage.Bleedline, new Color(0.0f, 0.4f, 0.6f));

        foreach (Transform child in mPackage.transform)
        {
            var childPanel = child.GetComponent<Panel>();
            _HandlesHelper.DrawLine(childPanel.destVertices, new Color(0, 1, 0), 4);
        }

        for (int row = -2; row <= 2; row++)
        {
            for (int col = -3; col <= 3; col++)
            {
                var rect = mPackage.SrcCarMatrix[row, col];
                _HandlesHelper.DrawLine(rect.Vector2Array(), new Color(0, 1, 0), 4);
                Handles.Label(rect.center, "[" + row + ", " + col + "]\n");// + rect.ToString());
            }
        }

        /*foreach(var face in mPackage.Shape.Faces)
        {
            DrawLine(face.BasicPoints.Select(p =>(Vector2)p.Position), new Color(0, 1, 0), 4);
        }*/

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
    
}
