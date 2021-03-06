﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;

[CustomEditor(typeof(Panel))]
public class PanelEditor : Editor
{

    List<Vector2> AlignPoints = new List<Vector2>();

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
            keyPoints = new List<Vector3>(mPanel.VarRect.Vector3Array());
            
            AlignPoints.AddRange(mPanel.Outline);
            AlignPoints.AddRange(mPanel.SrcRect.Vector2List());
        }

        Handles.matrix = mPanel.transform.localToWorldMatrix;
        
        DrawOffsetLabel();

        DrawBorderLabel();
        
        _HandlesHelper.DrawLine(mPanel.SrcRect.Vector2List(), new Color(1, 0, 0), 4);

        _HandlesHelper.DrawLine(mPanel.Bleedline, new Color(0, 0, 1));

        foreach (var childPanel in mPanel.mPackage.Panels)
        {
            //var childPanel = child.GetComponent<Panel>();
            if (childPanel != mPanel)
            {
                _HandlesHelper.DrawLine(childPanel.destVertices, new Color(0, 1, 0), 4);
            }
            else
            {
                _HandlesHelper.DrawLine(childPanel.destVertices, new Color(0, 1, 0));
            }
        }


        for (int i = 0; i < keyPoints.Count; i++)
        {
            Handles.color = nearestIndex == i ? Color.green : Color.white;
            DrawSegment(i);
        }
        
        foreach (var p in keyPoints)
        {
            Handles.color = Color.white;
            Handles.CubeCap(0, p, Quaternion.identity, HandleUtility.GetHandleSize(p) * 0.06f);
        }

        //Quit if panning or no camera exists
        if (Tools.current == Tool.View || (e.isMouse && e.button > 0) || Camera.current == null || e.type == EventType.ScrollWheel)
            return;
        
        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            return;
        }
        
        EditorGUIUtility.AddCursorRect(new Rect(0, 0, Camera.current.pixelWidth, Camera.current.pixelHeight), mouseCursor);
        mouseCursor = MouseCursor.Arrow;

        worldToLocal = mPanel.transform.worldToLocalMatrix;
        inverseRotation = Quaternion.Inverse(mPanel.transform.rotation) * Camera.current.transform.rotation;

        screenMousePosition = new Vector3(e.mousePosition.x, Camera.current.pixelHeight - e.mousePosition.y);
        var plane = new Plane(-mPanel.transform.forward, mPanel.transform.position);
        var ray = Camera.current.ScreenPointToRay(screenMousePosition);
        float hit;
        if (plane.Raycast(ray, out hit))
            mousePosition = worldToLocal.MultiplyPoint(ray.GetPoint(hit));
        else
            return;
        
        nearestIndex = NearestLine(out nearestPosition);
        
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

    public Panel mPanel
    {
        get
        {
            return (Panel)target;
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
    //List<int> selectedIndices = new List<int>();
    Vector3 nearestPosition;

    MouseCursor mouseCursor;

    //bool autoSnap = true;

    Event e
    {
        get { return Event.current; }
    }

    void DrawBorderLabel()
    {
        var cX = mPanel.SrcRect.center.x;
        var cY = mPanel.SrcRect.center.y;
        var iX = mPanel.SrcRect.xMin;
        var aX = mPanel.SrcRect.xMax;
        var iY = mPanel.SrcRect.yMin;
        var aY = mPanel.SrcRect.yMax;

        var offset = HandleUtility.GetHandleSize(mPanel.SrcRect.center) * 0.2F;

        Handles.Label(new Vector3(iX, cY, 0) + Vector3.right * offset, "R: \n" + mPanel.Right);
        Handles.Label(new Vector3(aX, cY, 0) + Vector3.right * offset, "L: \n" + mPanel.Left);
        Handles.Label(new Vector3(cX, iY, 0) + Vector3.up * offset, "B: \n" + mPanel.Bottom);
        Handles.Label(new Vector3(cX, aY, 0) + Vector3.up * offset, "T: \n" + mPanel.Top);
    }

    void DrawOffsetLabel()
    {
        var offset = HandleUtility.GetHandleSize(mPanel.SrcRect.center) * 0.5F;
        Handles.Label(mPanel.SrcRect.center + Vector2.right * offset, "min : " + mPanel.OffsetMin.ToString() + "\nmax : " + mPanel.OffsetMax.ToString());
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

            //var j = (i + 1) % keyPoints.Count;

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
        if (Tools.current == Tool.Move)
        {
            index = NearestLine(out nearestPosition);
            if (index >= 0 && IsSegmentHovering(index))
            {
                Handles.color = Color.white;
                //DrawCircle(nearestPosition, clickRadius);
                _HandlesHelper.DrawCircle(nearestPosition);
                return true;
            }

        }
        index = -1;
        return false;
    }

    /*bool TryHover(List<Vector3> points, Color color, out int index)
    {
        if (Tools.current == Tool.Move)
        {
            index = NearestLine(out nearestPosition);
            if (index >= 0 && IsSegmentHovering(index))
            {
                Handles.color = color;
                //DrawCircle(nearestPosition, clickRadius);
                _HandlesHelper.DrawCircle(nearestPosition);
                return true;
            }

        }
        index = -1;
        return false;
    }*/

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
        /*for (int i = 0; i < keyPoints.Count; i++)
        {
            ImPanel.BorderPoints[i] = keyPoints[i];
        }*/
        //ImPanel.CalcAlphaFactor();
        mPanel.UpdateBorder(keyPoints);
    }

    void MoveSegment(int index, Vector3 from, Vector3 to)
    {

        var next = (index + 1) % keyPoints.Count;
        var delta = to - from;
        if (index % 2 != 0)
        {
            foreach (var p in AlignPoints)
            {
                if (Math.Abs(p.y - to.y) < HandleUtility.GetHandleSize(p) * 0.2F)
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
            foreach (var p in AlignPoints)
            {
                if (Math.Abs(p.x - to.x) < HandleUtility.GetHandleSize(p) * 0.2F)
                {
                    keyPoints[index] = new Vector3(p.x, keyPoints[index].y, 0);
                    keyPoints[next] = new Vector3(p.x, keyPoints[next].y, 0);
                    return;
                }
            }

            delta.y = 0;
        }

        keyPoints[index] = mPanel.VarRect.Vector3Array()[index] + delta;
        keyPoints[next] = mPanel.VarRect.Vector3Array()[next] + delta;

    }



}
