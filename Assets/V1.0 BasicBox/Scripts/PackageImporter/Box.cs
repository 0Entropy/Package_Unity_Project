using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using System;

/// <summary>
/// It is a basic package class.
/// </summary>
public class Box
{
    
    public Shape2D Shape { set; get; }

    public List<Vector2> Outline { set; get; }

    public List<Vector2> Bleedline { set; get; }

    public float Length { set; get; }
    public float Width { set; get; }
    public float Depth { set; get; }
    public float Thickness { set; get; }

    public List<Crease> Creases { set; get; }

    public List<Face> Faces { set; get; }

    public CartesianMatrix<Rect> SrcCartMatrix { set; get; }
    public CartesianMatrix<Rect> DestCartMatrix { set; get; }

    /*public int[] LookUpCoordinate(Face2D shapeFace)
    {
        var face = Faces.Find(p => p.ShapeFace == shapeFace);
        if (face == null)
            throw new System.Exception("The panel is not exact!");

        return new int[] { face.Row, face.Col };
    }*/

    /*public Face LookUp(int row, int col)
    {

        return Faces.Find(p => (p.Row == row && p.Col == col));

    }*/

    public Face this[int row, int col]
    {
        get { return Faces.Find(p => (p.Row == row && p.Col == col)); }
    }

    public void OnResize(float length, float width, float depth, float thickness = 0.01f)
    {

        DestCartMatrix = CalcCartMatrix(length, width, depth);

        //List<List<Vector2>> bleedInChildren = new List<List<Vector2>>();
        foreach (var face in Faces)
        {
            face.UpdateDimension(DestCartMatrix[face.Row, face.Col]);
            //bleedInChildren.Add(face.Bleedline);
        }

        Outline = Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();
        //Bleedline = PolygonAlgorithm.Merge(bleedInChildren);

        Bleedline = PolygonAlgorithm.Merge(Faces.Select(f => f.Bleedline).ToList());

    }

    /*public void OnInit(float length, float width, float depth, float thickness = 0.01f)
    {

        InitDimension(length, width, depth, thickness);

        InitShape();

    }*/

     
    #region Generable Box
    public void OnInit(PackageData data)
    {
        InitDimension(data.Length, data.Width, data.Depth, data.Thickness);
        //InitShape(data.Panels);
        //InitOutline(data.Panels.Select(p => p.Vertices.Select(v => (Vector3)v).ToList()).ToList());
        InitFaces(data.Panels);

        InitEdges(data.Creases);
    }
    
    #endregion

    #region Face

    public void InitFaces(List<PanelData> datas)
    {
        Shape = new Shape2D();
        Faces = new List<Face>();
        //List<List<Vector2>> bleedInChildren = new List<List<Vector2>>();

        foreach (var data in datas)
        {
            var face = new Face();
            face.OnInit(data);
            //Add(face);
            face.mBox = this;
            face.ShapeFace = Shape.AddPoints(face.SrcOutline.Select(p => new Point2D(p)).ToArray());
            
            Faces.Add(face);

        }


        Outline = Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();
        Bleedline = PolygonAlgorithm.Merge(Faces.Select(f => f.Bleedline).ToList());
    }

    #endregion

    #region Edge

    public bool IsNotBinded { get { return Edges.Any(e => !e.IsBinded); } }

    public List<Edge> Edges;
    private void InitEdges(List<CreaseData> creases)
    {
        Edges = new List<Edge>();
        foreach (var crease in creases)
        {
            var edge = new Edge(crease.Vertices[0], crease.Vertices[1], crease.FoldAngle);
            foreach (var neighbor in crease.Neighbors)
            {
                edge.AddFace(this[neighbor[0], neighbor[1]]);
            }
            Edges.Add(edge);
        }
    }

    private void UpdateEdges()
    {

    }

    #endregion

    void InitDimension(float length, float width, float depth, float thickness)
    {
        Length = length;
        Width = width;
        Depth = depth;
        Thickness = thickness;

        SrcCartMatrix = CalcCartMatrix(length, width, depth);
        DestCartMatrix = CalcCartMatrix(length, width, depth);
    }

    public static CartesianMatrix<Rect> CalcCartMatrix(float length, float width, float depth)
    {
        CartesianMatrix<Rect> result = new CartesianMatrix<Rect>(5, 7);

        var d_x = (length + width) * 0.5f;
        var d_y_0 = (depth + width) * 0.5f;
        var d_y_1 = (depth + length) * 0.5f;

        for (int row = -2; row <= 2; row++)
        {
            for (int col = -3; col <= 3; col++)
            {
                var c_x = col * d_x;
                var c_y = row * (col % 2 == 0 ? d_y_0 : d_y_1);// + 0.0059F;

                var w = col % 2 == 0 ? length : width;
                var h = row % 2 == 0 ? depth : (col % 2 == 0 ? width : length);

                var min_x = c_x - w * 0.5f;
                var max_x = c_x + w * 0.5f;
                var min_y = c_y - h * 0.5f;
                var max_y = c_y + h * 0.5f;

                result[row, col] = new Rect(min_x, min_y, w, h);
            }
        }

        return result;
    }
    
}

