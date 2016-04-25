using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;

public class Face
{

    public Box mBox { set; get; }

    public GameObject Object3D { set; get; }

    public GameObject Object2D { set; get; }

    #region Edge

    public List<Edge> Edges { set; get; }// = new List<Edge>();

    public Edge FindEdgeByFace(Face face) { return Edges.Find(e => e.Faces.Contains(face)); }

    public List<Face> Neighbors
    {
        get { return Edges.Select(e => e.Faces.Find(f => f != this)).ToList(); }
    }

    public bool IsRoot { get { return Row == 0 && Col == 0; } }

    public bool IsEnd { get { return Edges.Count == 1; } }

    public bool IsNext { get { return !IsRoot && ((IsEnd && Edges.Any(e => !e.IsRotated)) || (!IsEnd && Edges.FindAll(e => !e.IsRotated).Count() == 1)); } }

    #endregion

    public Face()
    {
        Edges = new List<Edge>();
    }

    public int Row { set; get; }
    public int Col { set; get; }

    public Rect SrcRect { set; get; }

    public Rect DestRect { set; get; }

    public Rect VarRect { set; get; }

    public Face2D ShapeFace { set; get; }

    public List<Vector2> SrcOutline { set; get; }

    public List<Vector2> Bleedline { set; get; }

    public List<Vector2> Alphas { set; get; }

    public Vector2 OffsetMin, OffsetMax;

    public List<Vector2> DestOutline { set; get; }// = new List<Vector2>();

    //private Vector2 SMALL = 0.00002F * Vector2.up;

    public void OnInit(PanelData data)
    {
        Col = data.Col;
        Row = data.Row;

        //InitOutline(data.Vertices.Select(p => (Vector2)p).ToList());

        SrcOutline = new List<Vector2>(data.Vertices.Select(p => (Vector2)p ).ToList());

        SrcRect = Rect.MinMaxRect(data.Right, data.Bottom, data.Left, data.Top);

        VarRect = Rect.MinMaxRect(data.VarRight, data.VarBottom, data.VarLeft, data.VarTop);

        Alphas = data.Alphas.Select(a => (Vector2)a).ToList();

        UpdateDimension(SrcRect);

        
    }

    public void InitOutline(List<Vector2> vertices)
    {

        SrcOutline = new List<Vector2>(vertices);

        //DestOutline = new List<Vector2>(SrcOutline);

        //Bleedline = CGAlgorithm.ScalePoly(DestOutline, 0.05f);
    }

    public void UpdateDimension(Rect dest)
    {
        DestRect = new Rect(dest);
        OffsetMin = DestRect.min - SrcRect.min;
        OffsetMax = DestRect.max - SrcRect.max;
        OnResize();
    }

    /*public void OnResize()
    {
        OnResize(OffsetMin, OffsetMax);
    }*/

    void OnResize()
    {
        //OffsetMin = offsetMin;
        //OffsetMax = offsetMax;

        if (DestOutline == null)
            DestOutline = new List<Vector2>();
        else
            DestOutline.Clear();

        for (int i = 0; i < SrcOutline.Count; i++)
        {
            var aX = Alphas[i].x;
            var aY = Alphas[i].y;
            var dX = OffsetMin.x * (1.0f - aX) + OffsetMax.x * aX;
            var dY = OffsetMin.y * (1.0f - aY) + OffsetMax.y * aY;
            var x = SrcOutline[i].x + dX;
            var y = SrcOutline[i].y + dY;
            DestOutline.Add(new Vector2(x, y));
        }

        Bleedline = CGAlgorithm.ScalePoly(DestOutline, 0.05f);
    }

    public Vector3 Translate(Vector3 p)
    {
        var aX = VarRect.width == 0 ? 1 : Mathf.Clamp((p.x - VarRect.xMin) / VarRect.width, 0, 1);
        var aY = VarRect.height == 0 ? 1 : Mathf.Clamp((p.y - VarRect.yMin) / VarRect.height, 0, 1);
        
        var offsetMin = DestRect.min - SrcRect.min;
        var offsetMax = DestRect.max - SrcRect.max;

        var dX = offsetMin.x * (1.0f - aX) + offsetMax.x * aX;
        var dY = offsetMin.y * (1.0f - aY) + offsetMax.y * aY;
        var x = p.x + dX;
        var y = p.y + dY;

        return new Vector3(x, y, 0);

    }

    public Vector3 Translate(Vector3 p0, Vector3 p1)
    {
        return (Translate(p0) + Translate(p1)) * 0.5F;
    }

}
