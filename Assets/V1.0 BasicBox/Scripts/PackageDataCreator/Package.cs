using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using UnityEngine.UI;
using System.Collections;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Package : MonoBehaviour
{

    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
         WebGLInput.captureAllKeyboardInput = false;
#endif
    }
    public PackageData Data
    {
        get
        {
            PackageData data = new PackageData();
            data.Length = Length;
            data.Width = Width;
            data.Depth = Depth;
            data.Thickness = Thickness;
            data.Panels = Panels.Select(p => p.Data).ToList();
            
            data.Creases = GetComponent<Crease>().Data;

            return data;
        }
    }

    public Shape2D Shape { set; get; }

    public List<Vector2> Outline { set; get; }

    public List<Vector2> Bleedline { set; get; }

    public float Length { set; get; }
    public float Width { set; get; }
    public float Depth { set; get; }
    public float Thickness { set; get; }
    
    public List<Crease> Creases { set; get; }
    
    public List<Panel> Panels { set; get; }

    public CartesianMatrix<Rect> SrcCartMatrix { set; get; }
    public CartesianMatrix<Rect> DestCartMatrix { set; get; }
    
    public int[] LookUpCoordinate(Face2D face)
    {
        var panel = Panels.Find(p => p.Face == face);
        if (panel == null)
            throw new System.Exception("The panel is not exact!");
       
         return new int[] { panel.Row, panel.Col };
    }

    public Panel LookUp(int row, int col)
    {

        return Panels.Find(p => (p.Row == row && p.Col == col));
        
    }

    public void OnResize(float length, float width, float depth, float thickness = 0.01f)
    {
        //destMatrix = CalcRectMatrix(length, width, depth);
        DestCartMatrix = Box.CalcCartMatrix(length, width, depth);
        foreach(var panel in Panels)
        {
            panel.UpdateDimension(DestCartMatrix[panel.Row, panel.Col]);
        }
        
    }

    public void OnInit(float length, float width, float depth, float thickness = 0.01f)
    {

        InitDimension(length, width, depth, thickness);

        InitShape();

    }

    void InitDimension(float length, float width, float depth, float thickness)
    {
        Length = length;
        Width = width;
        Depth = depth;
        Thickness = thickness;
        
        SrcCartMatrix = Box.CalcCartMatrix(length, width, depth);
        DestCartMatrix = Box.CalcCartMatrix(length, width, depth);
    }

    void InitShape()
    {

        Panels = new List<Panel>();
        
        Shape = new Shape2D();

        List<List<Vector2>> bleedInChildren = new List<List<Vector2>>();

        foreach (Transform child in transform)
        {
            var panel = child.GetComponent<Panel>();
            if (!panel)
            {
                panel = child.gameObject.AddComponent<Panel>();
            }
            
            panel.InitShape();
            Add(panel);

            bleedInChildren.Add(panel.Bleedline);

        }

        Outline = Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();
        Bleedline = PolygonAlgorithm.Merge(bleedInChildren);
    }

    /*public CartesianMatrix<Rect> CalcCartMatrix(float length, float width, float depth)
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
    }*/



    public void Add(Panel panel)
    {

        panel.mPackage = this;
        panel.Face = Shape.AddPoints(panel.Outline.Select(p => new Point2D(p)).ToArray());

        for (int row = -2, i = 0; row <= 2; row++, i++)
        {
            for (int col = -3, j = 0; col <= 3; col++, j++)
            {
                //var rect = srcMatrix[i, j];
                var rect = SrcCartMatrix[row, col];
                //var vertices = rect.Vector2Array();
                if (CGAlgorithm.CN_PnPoly(panel.Center, rect.Vector2Array()) == 1)
                {
                    panel.InitDimension(rect);
                    //Panels[row, col] = panel;
                    panel.Row = row;
                    panel.Col = col;
                    Panels.Add(panel);
                    return;
                }
            }
        }

        throw new System.Exception("NO SUCH RECTANGLE ");
    }

}

/// <summary>
/// Cartesian coordinate system
/// </summary>
/// <typeparam name="T"></typeparam>
public class CartesianMatrix<T>
{
    /*private int Row;
    private int Col;*/
    private int offsetRow;// = 2;
    private int offsetCol;//= 3;

    public T[,] Array { set; get; }

    /*public List<T> Content { set; get; }*/

    public CartesianMatrix(int rowNum, int colNum)
    {
        offsetRow = rowNum / 2;
        offsetCol = colNum / 2;
//         Row = rowNum;
//         Col = colNum;
        Array = new T[rowNum, colNum];
    }

    public T this[int row, int col]
    {
        set { Array[row + offsetRow, col + offsetCol] = value; }
        get { return Array[row + offsetRow, col + offsetCol]; }
    }
}
