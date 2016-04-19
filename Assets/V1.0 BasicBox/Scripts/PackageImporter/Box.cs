using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Geometry;

/// <summary>
/// It is a basic package class.
/// </summary>
public class Box {
    
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

    public int[] LookUpCoordinate(Face2D shapeFace)
    {
        var face = Faces.Find(p => p.ShapeFace == shapeFace);
        if (face == null)
            throw new System.Exception("The panel is not exact!");

        return new int[] { face.Row, face.Col };
    }

    public Face LookUp(int row, int col)
    {

        return Faces.Find(p => (p.Row == row && p.Col == col));

    }

    public void OnResize(float length, float width, float depth, float thickness = 0.01f)
    {

        DestCartMatrix = CalcCartMatrix(length, width, depth);
        foreach (var face in Faces)
        {
            face.UpdateDimension(DestCartMatrix[face.Row, face.Col]);
        }

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
        InitShape(data.Panels);
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

    void InitShape(List<PanelData> panels)
    {

        Faces = new List<Face>();

        Shape = new Shape2D();

        List<List<Vector2>> bleedInChildren = new List<List<Vector2>>();

        foreach (var panel in panels)
        {
            var vertices = panel.Vertices.Select(p => new Vector2(p.x, p.y)).ToList();
            var face = new Face();
            face.InitShape(vertices);
            Add(face);

            bleedInChildren.Add(face.Bleedline);

        }
        
        /*foreach (Transform child in transform)
        {
            var panel = child.GetComponent<Panel>();
            if (!panel)
            {
                panel = child.gameObject.AddComponent<Panel>();
            }

            panel.InitShape();
            Add(panel);

            bleedInChildren.Add(panel.Bleedline);

        }*/

        Outline = Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();
        Bleedline = PolygonAlgorithm.Merge(bleedInChildren);
    }

    void AddPanels()
    {

    }
    
    public CartesianMatrix<Rect> CalcCartMatrix(float length, float width, float depth)
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
    
    public void Add(Face face)
    {

        face.mBox = this;
        face.ShapeFace = Shape.AddPoints(face.Outline.Select(p => new Point2D(p)).ToArray());

        for (int row = -2, i = 0; row <= 2; row++, i++)
        {
            for (int col = -3, j = 0; col <= 3; col++, j++)
            {
                //var rect = srcMatrix[i, j];
                var rect = SrcCartMatrix[row, col];
                //var vertices = rect.Vector2Array();
                if (CGAlgorithm.CN_PnPoly(face.Center, rect.Vector2Array()) == 1)
                {
                    face.InitDimension(rect);
                    //Panels[row, col] = panel;
                    face.Row = row;
                    face.Col = col;
                    Faces.Add(face);
                    return;
                }
            }
        }

        throw new System.Exception("NO SUCH RECTANGLE ");
    }

}

