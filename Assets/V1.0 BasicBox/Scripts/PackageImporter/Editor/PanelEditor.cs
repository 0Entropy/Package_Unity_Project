﻿using UnityEngine;
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

        if (keyPoints == null)
        {
            
            keyPoints = new List<Vector3>(ImPanel.keyPoints);
            Debug.Log("Border : " + ImPanel.BorderRect.center.ToString());
            ImPanel.TestRowAndCol();
        }

        Handles.matrix = ImPanel.transform.localToWorldMatrix;

        for (int i = 0; i < keyPoints.Count; i++)
        {
            Handles.color = nearestIndex == i ? Color.green : Color.white;
            DrawSegment(i);
        }

        //Handles.DrawSolidRectangleWithOutline(ImPanel.BorderArray, new Color(0.2f, 0.2f, 0.2f, 0.2f), new Color(0.0f, 0.4f, 0.6f));

        //Handles.color = new Color(0, 0.5f, 0.8f);

        foreach (var p in keyPoints)
        {
            Handles.CubeCap(0, p, Quaternion.identity, HandleUtility.GetHandleSize(p) * 0.08f);
        }

        //Quit if panning or no camera exists
        if (Tools.current == Tool.View || (e.isMouse && e.button > 0) || Camera.current == null || e.type == EventType.ScrollWheel)
            return;

        //Quit if laying out
        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            return;
        }

        //Cursor rectangle
        EditorGUIUtility.AddCursorRect(new Rect(0, 0, Camera.current.pixelWidth, Camera.current.pixelHeight), mouseCursor);
        mouseCursor = MouseCursor.Arrow;

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

        //Update nearest line and nearest position
        nearestIndex = NearestLine(out nearestPosition);

        //var index = -1;
        //TryHoverSegment(out index);

        //Update the state and repaint
        var newState = UpdateState();
        if (state != newState)
            SetState(newState);
        HandleUtility.Repaint();

        /*e.Use();*/
    }

    enum State { Hover, Drag }//, BoxSelect, DragSelected, RotateSelected, ScaleSelected, Extrude }

    State state;

    //Initialize state
    void SetState(State newState)
    {
        state = newState;
        switch (state)
        {
            case State.Hover:
                break;
        }
        //Debug.Log("state : " + state.ToString());
        
    }

    State UpdateState()
    {
        switch (state)
        {
            case State.Hover:

                if (TryHoverSegment(out dragIndex) && TryDragSegment(dragIndex))
                    return State.Drag;
                break;

            case State.Drag:
                mouseCursor = MouseCursor.MoveArrow;
                MoveSegment(dragIndex, clickPosition, mousePosition);
                if (TryStopDrag())
                    return State.Hover;
                break;
                
        }
        return state;
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

    const float clickRadius = 0.08f;

    List<Vector3> keyPoints;

    Matrix4x4 worldToLocal;
    Quaternion inverseRotation;

    Vector3 screenMousePosition;

    Vector3 clickPosition;
    Vector3 mousePosition;

    int nearestIndex;
    int dragIndex;
    List<int> selectedIndices = new List<int>();
    Vector3 nearestPosition;

    MouseCursor mouseCursor;

    bool autoSnap = true;

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
            var distance = MouseToSegmentDistance(i, out nearPos);

            if (distance < nearDist)
            {
                nearDist = distance;
                position = nearPos;
                near = i;
            }

        }
        return near;
    }

    float MouseToSegmentDistance(int index, out Vector2 nearPos)
    {
        var next = (index + 1) % keyPoints.Count;

        return CGAlgorithm.PointToSegementDistance(mousePosition, keyPoints[index], keyPoints[next], out nearPos);

    }

    bool TryHoverSegment(out int index)
    {
        if (TryHover(keyPoints, Color.white, out index))
        {
            mouseCursor = MouseCursor.MoveArrow;
            return true;
        }
        return false;
    }

    bool TryHover(List<Vector3> points, Color color, out int index)
    {
        if (Tools.current == Tool.Move)
        {
            index = NearestLine(out nearestPosition);
            if (index >= 0 && IsSegmentHovering(index))
            {
                Handles.color = color;
                DrawCircle(nearestPosition, clickRadius);
                return true;
            }

        }
        index = -1;
        return false;
    }

    bool IsSegmentHovering(int index)//(Vector3 position)
    {

        Vector2 nearestPos;
        return MouseToSegmentDistance(index, out nearestPos) < HandleUtility.GetHandleSize(nearestPos) * clickRadius;

    }

    bool TryDragSegment(int index)
    {
        if (TryDrag(keyPoints, index))
        {
            //clickPosition = mousePosition;
            return true;
        }
        return false;
    }

    bool TryDrag(List<Vector3> points, int index)
    {
        if (e.type == EventType.MouseDown && IsSegmentHovering(index))
        {
            clickPosition = mousePosition;
            
            return true;
        }
        return false;
    }

    bool TryStopDrag()
    {
        if (e.type == EventType.MouseUp)
        {
            dragIndex = -1;

            UpdateBorder();
            return true;
        }
        return false;
    }

    private void UpdateBorder()
    {
        for (int i = 0; i < keyPoints.Count; i++)
        {
            ImPanel.keyPoints[i] = keyPoints[i];
        }
    }

    void MoveSegment(int index, Vector3 from, Vector3 to)
    {

        var next = (index + 1) % keyPoints.Count;
        var delta = to - from;
        if (index % 2 != 0)
        {
            foreach(var p in ImPanel.Outline)
            {
                if(Math.Abs(p.y - to.y) < clickRadius)
                {
                    //delta.y = p.y - mousePosition.y;
                    keyPoints[index] = new Vector3(keyPoints[index].x, p.y, 0);
                    keyPoints[next] = new Vector3(keyPoints[next].x, p.y, 0);
                    return;
                }
            }
            delta.x = 0;
        }
        else
        {
            foreach (var p in ImPanel.Outline)
            {
                if (Math.Abs(p.x - to.x) < clickRadius)
                {
                    keyPoints[index] = new Vector3(p.x, keyPoints[index].y, 0);
                    keyPoints[next] = new Vector3(p.x, keyPoints[next].y, 0);
                    return;
                }
            }

            delta.y = 0;
        }

        keyPoints[index] = ImPanel.keyPoints[index] + delta;
        keyPoints[next] = ImPanel.keyPoints[next] + delta;

    }


    
}
