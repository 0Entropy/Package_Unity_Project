using UnityEngine;
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

            keyPoints = new List<Vector3>(mPanel.BorderPoints);
            //Debug.Log("Border Rect Center : " + ImPanel.BorderRect.center.ToString());
            //var dimension = ImPanel.DimensionArray;
            AlignPoints.AddRange(mPanel.Outline);
            AlignPoints.AddRange(mPanel.DimPoints);
        }

        Handles.matrix = mPanel.transform.localToWorldMatrix;

        //Handles.Label(ImPanel.DimRect.center, ImPanel.Col + ", " + ImPanel.Row);

        //Handles.Label(ImPanel.DimRect.center + Vector2.up, ImPanel.Left + ", " + ImPanel.Right + ", " + ImPanel.Top + ", " + ImPanel.Bottom);

        DrawOffsetLabel();

        DrawBorderLabel();



        DrawLine(mPanel.DimPoints, new Color(1, 0, 0), 4);

        foreach (Transform child in mPanel.mPackage.transform)
        {
            var childPanel = child.GetComponent<Panel>();
            if (childPanel != mPanel)
            {
                DrawLine(childPanel.destVertices, new Color(0, 1, 0), 4);
            }
            else
            {
                DrawLine(childPanel.destVertices, new Color(0, 1, 0));
            }
        }


        for (int i = 0; i < keyPoints.Count; i++)
        {
            Handles.color = nearestIndex == i ? Color.green : Color.white;
            DrawSegment(i);
        }

        //Handles.DrawSolidRectangleWithOutline(ImPanel.BorderArray, new Color(0.2f, 0.2f, 0.2f, 0.2f), new Color(0.0f, 0.4f, 0.6f));

        //Handles.color = new Color(0, 0.5f, 0.8f);

        foreach (var p in keyPoints)
        {
            Handles.color = Color.white;
            Handles.CubeCap(0, p, Quaternion.identity, HandleUtility.GetHandleSize(p) * 0.06f);
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
        var cX = mPanel.DimRect.center.x;
        var cY = mPanel.DimRect.center.y;
        var iX = mPanel.DimRect.xMin;
        var aX = mPanel.DimRect.xMax;
        var iY = mPanel.DimRect.yMin;
        var aY = mPanel.DimRect.yMax;

        var offset = HandleUtility.GetHandleSize(mPanel.DimRect.center) * 0.2F;

        Handles.Label(new Vector3(iX, cY, 0) + Vector3.right * offset, "R: \n" + mPanel.Right);
        Handles.Label(new Vector3(aX, cY, 0) + Vector3.right * offset, "L: \n" + mPanel.Left);
        Handles.Label(new Vector3(cX, iY, 0) + Vector3.up * offset, "B: \n" + mPanel.Bottom);
        Handles.Label(new Vector3(cX, aY, 0) + Vector3.up * offset, "T: \n" + mPanel.Top);
    }

    void DrawOffsetLabel()
    {
        var offset = HandleUtility.GetHandleSize(mPanel.DimRect.center) * 0.5F;
        Handles.Label(mPanel.DimRect.center + Vector2.right * offset, "min : " + mPanel.OffsetMin.ToString() + "\nmax : " + mPanel.OffsetMax.ToString());
    }

    void DrawLine(IEnumerable<Vector2> points, Color32 color, float DottedSpaceSize = 0)
    {

        if (points == null || points.Count() == 0)

            return;

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

        keyPoints[index] = mPanel.BorderPoints[index] + delta;
        keyPoints[next] = mPanel.BorderPoints[next] + delta;

    }



}
