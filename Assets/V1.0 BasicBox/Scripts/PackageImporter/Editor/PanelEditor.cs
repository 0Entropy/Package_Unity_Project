using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;

[CustomEditor(typeof(PanelImporter))]
public class PanelEditor : Editor
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
        
        if(keyPoints == null)
        {
            keyPoints = new List<Vector3>(ImPanel.BorderArray);
        }



        Handles.matrix = ImPanel.transform.localToWorldMatrix;

        for (int i=0; i < keyPoints.Count; i++)
        {
            Handles.color = nearestLine == i ? Color.green : Color.white;
            DrawSegment(i);
        }

        //Handles.DrawSolidRectangleWithOutline(ImPanel.BorderArray, new Color(0.2f, 0.2f, 0.2f, 0.2f), new Color(0.0f, 0.4f, 0.6f));

        //Handles.color = new Color(0, 0.5f, 0.8f);

        foreach(var p in ImPanel.BorderArray)
        {
            Handles.CubeCap(0, p, Quaternion.identity, HandleUtility.GetHandleSize(p) * 0.08f);
        }

        //Debug.Log(string.Format("width : {0}, height : {1}", ImPanel.Width, ImPanel.Height));

        worldToLocal = ImPanel.transform.worldToLocalMatrix;
        inverseRotation = Quaternion.Inverse(ImPanel.transform.rotation) * Camera.current.transform.rotation;
        
        screenMousePosition = new Vector3(e.mousePosition.x, Camera.current.pixelHeight - e.mousePosition.y);
        var plane = new Plane(-ImPanel.transform.forward, ImPanel.transform.position);
        var ray = Camera.current.ScreenPointToRay(screenMousePosition);
        float hit;
        if (plane.Raycast(ray, out hit))
            mousePosition = worldToLocal.MultiplyPoint(ray.GetPoint(hit));
        else
            return;

        //Update nearest line and split position
        nearestLine = NearestLine(out nearestPosition);

        var index = -1;
        TryHoverSegment(out index);
        //e.Use();
        HandleUtility.Repaint();
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

    const float clickRadius = 0.12f;

    List<Vector3> keyPoints;

    Matrix4x4 worldToLocal;
    Quaternion inverseRotation;

    Vector3 screenMousePosition;

    Vector3 clickPosition;
    Vector3 mousePosition;

    int nearestLine;
    List<int> selectedIndices = new List<int>();
    Vector3 nearestPosition;

    MouseCursor mouseCorsor;

    Event e
    {
        get { return Event.current; }
    }

    void DrawCircle(Vector3 position, float size)
    {
        Handles.CircleCap(0, position, inverseRotation, HandleUtility.GetHandleSize(position) * size);
    }

    void DrawSegment(int index)
    {
        var from = keyPoints[index];
        var to = keyPoints[(index + 1) % keyPoints.Count];

        Handles.DrawLine(from, to);
    }

    void DrawKeyPoint(int index)
    {
        Handles.DotCap(0, keyPoints[index], Quaternion.identity, HandleUtility.GetHandleSize(keyPoints[index]) * 0.03f);
    }

    int NearestLine(out Vector3 position)
    {
        var near = -1;
        var nearDist = float.MaxValue;
        position = keyPoints[0];

        for (int i = 0; i < keyPoints.Count; i++)
        {

            var j = (i + 1) % keyPoints.Count;

            Vector2 nearPos;
            var distance = CGAlgorithm.PointToSegementDistance(mousePosition, keyPoints[i], keyPoints[j], out nearPos);

            if (distance < nearDist)
            {
                nearDist = distance;
                position = nearPos;
                near = i;
            }

        }
        return near;
    }
    
    bool TryHoverSegment(out int index)
    {
        if(TryHover(keyPoints, Color.white, out index))
        {
            mouseCorsor = MouseCursor.MoveArrow;
            return true;
        }
        return false;
    }

    bool TryHover(List<Vector3> points, Color color, out int index)
    {
        if(Tools.current == Tool.Move)
        {
            index = NearestLine(out nearestPosition);
            if(index >= 0 && IsSegmentHovering(nearestPosition))
            {
                Handles.color = color;
                DrawCircle(nearestPosition, clickRadius);
                return true;
            }
            
        }
        index = -1;
        return false;
    }

    bool IsSegmentHovering(Vector3 position)
    {
        return Vector3.Distance(mousePosition, position) < HandleUtility.GetHandleSize(position) * clickRadius;

    }

    bool TryDragSegment(List<Vector3> points, int index)
    {
        if (e.type == EventType.MouseDown && IsSegmentHovering(nearestPosition))
        {
            clickPosition = mousePosition;
            return true;
        }
        return false;
    }
}
