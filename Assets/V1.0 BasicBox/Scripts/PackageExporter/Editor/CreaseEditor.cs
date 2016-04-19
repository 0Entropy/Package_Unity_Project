using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(Crease))]
public class CreaseEditor : Editor
{

    //public List<Vector3> keyPoints;
    public List<List<Vector3>> Segments;
    public List<float> Angles;
    Matrix4x4 worldToLocal;
    //Quaternion inverseRotation;

    Vector3 screenMousePosition;

    //MouseCursor mouseCursor;

    /*Event e
    {
        get { return Event.current; }
    }*/

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        if (target == null)
            return;

        EditorGUILayout.LabelField(mCrease.GetType().ToString());
        EditorGUILayout.LabelField(mPackage.GetType().ToString());

        //EditorGUILayout.BeginVertical();
        selectedIndices.Sort();
        foreach(var index in selectedIndices)
        {
            //EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField(index.ToString(), EditorStyles.boldLabel, GUILayout.MaxWidth(80));

            Angles[index] = EditorGUILayout.FloatField( index.ToString(), Angles[index], GUILayout.MinWidth(100));
            //EditorGUILayout.EndHorizontal();

        }
        //.EndVertical();
    }

    void OnSceneGUI()
    {
        if (target == null)
            return;

        if (Segments == null)
        {
            //Segments = mPackage.Shape.Edges.FindAll(e => e.Faces.Count == 2).Distinct().Select(e => e.Points.Select(p => p.Position).ToList()).ToList();
            //Debug.Log("Segments Count : " + Segments.Count);
            Segments = new List<List<Vector3>>(mCrease.Segments);
            Angles = new List<float>(mCrease.Angles);
        }

        Handles.matrix = mCrease.transform.localToWorldMatrix;

        foreach (var line in Segments)
        {
            //var points = line.Points.Select(p => (Vector2)p.Position);
            Handles.color = Color.red;
            foreach (var p in line)
                Handles.DotCap(0, p, Quaternion.identity, HandleUtility.GetHandleSize(p) * 0.03f);

            _HandlesHelper.DrawLine(line.Select(p => (Vector2)p), new Color(1, 0, 0));
        }

        for (int i = 0; i < Segments.Count; i++)
        {
            /*foreach (var p in Segments[selectedSegmentIndex])
            {

                Handles.color = Color.green;
                Handles.DotCap(0, p, Quaternion.identity, HandleUtility.GetHandleSize(p) * 0.03f);
                //Handles.color = Color.green;
                //_HandlesHelper.DrawCircle(Segments[selectedSegmentIndex][i], 0.08f);

            }*/
            if (selectedIndices.Contains(i))
            {

                foreach (var p in Segments[i])
                {

                    Handles.color = Color.green;
                    Handles.DotCap(0, p, Quaternion.identity, HandleUtility.GetHandleSize(p) * 0.03f);
                    //Handles.color = Color.green;
                    //_HandlesHelper.DrawCircle(Segments[selectedSegmentIndex][i], 0.08f);

                }
                
            }
        }

        //Quit if panning or no camera exists
        if (Tools.current == Tool.View ||
            (Event.current.isMouse && Event.current.button > 0) ||
            Camera.current == null ||
            Event.current.type == EventType.ScrollWheel)
            return;

        //Quit if laying out
        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            return;
        }

        //Cursor rectangle
        //EditorGUIUtility.AddCursorRect(new Rect(0, 0, Camera.current.pixelWidth, Camera.current.pixelHeight), mouseCursor);
        //mouseCursor = MouseCursor.Arrow;

        worldToLocal = mCrease.transform.worldToLocalMatrix;
        //inverseRotation = Quaternion.Inverse(mCrease.transform.rotation) * Camera.current.transform.rotation;

        screenMousePosition =
            new Vector3(
                Event.current.mousePosition.x,
            Camera.current.pixelHeight - Event.current.mousePosition.y);

        var plane = new Plane(-mCrease.transform.forward, mCrease.transform.position);
        var ray = Camera.current.ScreenPointToRay(screenMousePosition);
        float hit;
        if (plane.Raycast(ray, out hit))
            mousePosition = worldToLocal.MultiplyPoint(ray.GetPoint(hit));
        else
            return;

        //Update nearest line and nearest position
        //nearestIndex = NearestLine(out nearestPosition);


        var newState = UpdateState();
        if (state != newState)
            SetState(newState);

        HandleUtility.Repaint();

    }

    #region State

    int dragIndex;

    enum State { Hover, Drag, BoxSelect, TapSelect }//, DragSelected, RotateSelected, ScaleSelected, Extrude }

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
        Event e = Event.current;
        var controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (state)
        {
            case State.Hover:

                if (TryDeleteSelected())
                {
                    Debug.Log("Try Delete Selected.");
                    return State.Hover;
                }

                /*if (TryHoverSegment(out dragIndex) && TryDragSegment(dragIndex))
                    return State.Drag;*/

                if (TryHoverSegment(out dragIndex) && TryTapSelect(dragIndex))
                    return State.TapSelect;

                if (TryBoxSelect())
                    return State.BoxSelect;

                break;
            case State.TapSelect:
                if (TryTapSelectEnd())
                    return State.Hover;
                break;
            case State.Drag:
                //mouseCursor = MouseCursor.MoveArrow;
                MoveSegment(dragIndex, clickPosition, mousePosition);
                if (TryStopDrag())
                    return State.Hover;
                break;

            case State.BoxSelect:
                if (TryBoxSelectEnd())
                    return State.Hover;
                break;
        }
        return state;
    }
    #endregion

    #region Tap Select
    bool TryTapSelect(int index)
    {
        Event e = Event.current;
        if(e.type == EventType.mouseDown && IsSegmentHovering(index))
        {
            if (!control)
            {
                selectedIndices.Clear();
            }
            selectedIndices.Add(index);
            return true;
        }
        return false;
    }

    bool TryTapSelectEnd()
    {
        Event e = Event.current;
        if (e.type == EventType.mouseUp)
        {
            //selectedIndices.Add(index);
            Repaint();
            return true;
        }
        return false;
    }
    #endregion

    #region BoxSelect

    //int selectedSegmentIndex = -1;
    
    List<int> selectedIndices = new List<int>();

    bool control
    {
        get
        {
            return Application.platform == RuntimePlatform.OSXEditor ? Event.current.command : Event.current.control;
        }
    }

    bool TryBoxSelect()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            clickPosition = mousePosition;
            return true;
        }
        return false;
    }

    bool TryBoxSelectEnd()
    {
        var min = new Vector3(Mathf.Min(clickPosition.x, mousePosition.x), Mathf.Min(clickPosition.y, mousePosition.y));
        var max = new Vector3(Mathf.Max(clickPosition.x, mousePosition.x), Mathf.Max(clickPosition.y, mousePosition.y));
        Handles.color = Color.white;
        Handles.DrawLine(new Vector3(min.x, min.y), new Vector3(max.x, min.y));
        Handles.DrawLine(new Vector3(min.x, max.y), new Vector3(max.x, max.y));
        Handles.DrawLine(new Vector3(min.x, min.y), new Vector3(min.x, max.y));
        Handles.DrawLine(new Vector3(max.x, min.y), new Vector3(max.x, max.y));

        if (Event.current.type == EventType.MouseUp)
        {
            var rect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);

            if (!control)
            {
                selectedIndices.Clear();
            }
            for (int i = 0; i < Segments.Count; i++)
            {
                if (rect.Contains(Segments[i][0]) && rect.Contains(Segments[i][1]))
                {
                    selectedIndices.Add(i);
                }

            }
            Repaint();
            return true;
        }
        return false;
    }

    bool TryDeleteSelected()
    {
        Event e = Event.current;

        if (KeyPressed(KeyCode.Delete))
        {
            //Debug.Log(" Try Delete Select Event : " + e);

            if (selectedIndices.Count > 0)
            {

                selectedIndices.Sort();

                UpdateTarget();



                selectedIndices.Clear();
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Hover

    const float clickRadius = 0.08f;

    Vector3 nearestPosition;

    Vector3 clickPosition;
    Vector3 mousePosition;

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

    int NearestLine(out Vector3 position)
    {
        var near = -1;
        var nearDist = float.MaxValue;
        position = Vector3.zero;

        for (int i = 0; i < Segments.Count; i++)
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

    bool IsSegmentHovering(int index)//(Vector3 position)
    {

        Vector2 nearestPos;
        return MouseToSegmentDistance(index, out nearestPos) < HandleUtility.GetHandleSize(nearestPos) * clickRadius;

    }

    float MouseToSegmentDistance(int index, out Vector2 nearPos)
    {
        //var next = (index + 1) % keyPoints.Count;

        return CGAlgorithm.PointToSegementDistance(mousePosition, Segments[index][0], Segments[index][1], out nearPos);

    }

    #endregion

    #region Drag


    //int nearestIndex;

    bool TryDragSegment(int index)
    {
        if (Event.current.type == EventType.MouseDown && IsSegmentHovering(index))
        {
            clickPosition = mousePosition;

            return true;
        }
        return false;
    }
    
    bool TryStopDrag()
    {
        if (Event.current.type == EventType.MouseUp)
        {
            dragIndex = -1;

            //UpdateBorder();
            Debug.Log("Stop!!");
            return true;
        }
        return false;
    }

    #endregion

    #region Move
    void MoveSegment(int index, Vector3 from, Vector3 to)
    {
        Debug.Log("move!!");
        /*var next = (index + 1) % keyPoints.Count;
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
        keyPoints[next] = mPanel.BorderPoints[next] + delta;*/

    }
    #endregion

    #region Update Data
    void UpdateTarget()
    {
        //mCrease.Segments = new List<List<Vector3>>(Segments);
        mCrease.RemoveByIndices(selectedIndices);
        Segments = new List<List<Vector3>>(mCrease.Segments);


    }
    #endregion

    bool KeyPressed(KeyCode key)
    {
        var e = Event.current;
        return e.type == EventType.KeyDown &&
            e.modifiers == (EventModifiers.Control | EventModifiers.FunctionKey) &&
            e.keyCode == key;
    }

    public Crease mCrease
    {
        get
        {
            return (Crease)target;
        }
    }

    public Package mPackage
    {
        get
        {
            return mCrease.GetComponent<Package>();
        }
    }


}
