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

    public float Length { set; get; }
    public float Width { set; get; }
    public float Depth { set; get; }
    public float Thickness { set; get; }

    public List<Face> Faces { set; get; }

    public CartesianMatrix<Rect> SrcCartMatrix { set; get; }
    public CartesianMatrix<Rect> DestCartMatrix { set; get; }

    public Rect Boundry
    {
        get
        {
            float min_x = 0;
            float min_y = 0;
            float max_x = 0;
            float max_y = 0;
            foreach (var face in Faces)
            {
                min_x = Mathf.Min(min_x, face.Bleedline.Min(p => p.x));
                max_x = Mathf.Max(max_x, face.Bleedline.Max(p => p.x));
                min_y = Mathf.Min(min_y, face.Bleedline.Min(p => p.y));
                max_y = Mathf.Max(max_y, face.Bleedline.Max(p => p.y));
            }

            return Rect.MinMaxRect(min_x, min_y, max_x, max_y);
        }
    }

    public Face this[int row, int col]
    {
        get { return Faces.Find(p => (p.Row == row && p.Col == col)); }
    }

    public void OnResize(float length, float width, float depth, float thickness = 0.01f)
    {
        DestCartMatrix = CalcCartMatrix(length, width, depth);

        foreach (var face in Faces)
        {
            face.Object3D.transform.position = Vector3.zero;
            face.Object3D.transform.rotation = Quaternion.identity;

            face.UpdateDimension(DestCartMatrix[face.Row, face.Col]);
        }

        foreach (var edge in Edges)
            edge.IsRotated = false;

    }

    #region Generable Box
    public void OnInit(PackageData data)
    {

        InitDimension(data.Length, data.Width, data.Depth, data.Thickness);

        InitFaces(data.Panels);

        InitEdges(data.Creases);

    }

    #endregion

    #region Face

    public void InitFaces(List<PanelData> datas)
    {

        Faces = new List<Face>();

        foreach (var data in datas)
        {
            var face = new Face();
            face.OnInit(data);

            face.mBox = this;

            Faces.Add(face);

        }

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

    public void OnBindFaces(GameObject obj)
    {
        if (IsNotBinded)
        {

            var root = Faces.Find(f => f.IsRoot);
            root.Object3D.transform.SetParent(obj.transform);
            BindByEdge(root);
        }
    }

    void BindByEdge(Face face)
    {
        foreach (var f in face.Neighbors)
        {
            var edge = f.FindEdgeByFace(face);
            if (!edge.IsBinded)
            {
                f.Object3D.transform.SetParent(face.Object3D.transform);
                edge.IsBinded = true;
                BindByEdge(f);
            }
        }
    }

    public void OnRotateFaces()
    {
        while (Faces.FindAll(f => f.IsNext).Count > 0)
        {
            RotateAroundEdge();
        }
    }

    void RotateAroundEdge()
    {
        foreach (var f in Faces.FindAll(f => f.IsNext).Distinct())
        {
            //Debug.Log(face.mTransform.name);
            var edge = f.Edges.Find(e => !e.IsRotated);

            var point = f.Translate(edge.Point);

            f.Object3D.transform.RotateAround(point, edge.Axis, edge.Angle);
            edge.IsRotated = true;
            //RotateByEdge(f);
        }
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
                var c_y = row * (col % 2 == 0 ? d_y_0 : d_y_1);

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

