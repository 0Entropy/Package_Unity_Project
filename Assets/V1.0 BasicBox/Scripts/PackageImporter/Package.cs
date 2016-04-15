using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using UnityEngine.UI;
using System.Collections;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Package : MonoBehaviour
{
    public Shape2D Shape { set; get; }

    public List<Vector2> Outline { set; get; }

    public List<Vector2> Bleedline { set; get; }

    public float Length { set; get; }
    public float Width { set; get; }
    public float Depth { set; get; }
    public float Thickness { set; get; }

    //public Rect[,] srcMatrix { set; get; }

    public List<Crease> Creases { set; get; }

    //public Panel[,] Panels { set; get; }
    public CartesianMatrix<Panel> Panels { set; get; }

    public CartesianMatrix<Rect> SrcCarMatrix { set; get; }

    public Rect[,] destMatrix { set; get; }

    public void OnResize(float length, float width, float depth, float thickness = 0.01f)
    {
        destMatrix = CalcRectMatrix(length, width, depth);

        for (int row = -2, i = 0; row <= 2; row++, i++)
        {
            for (int col = -3, j = 0; col <= 3; col++, j++)
            {
                var panel = Panels[row, col];
                if (panel == null)
                    continue;

                var offsetMin = destMatrix[i, j].min - SrcCarMatrix[row, col].min;//srcMatrix[i, j].min;//
                var offsetMax = destMatrix[i, j].max - SrcCarMatrix[row, col].max; //srcMatrix[i, j].max;// 
                panel.OnResize(offsetMin, offsetMax);
            }
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

        //srcMatrix = CalcRectMatrix(length, width, depth);
        SrcCarMatrix = CalcCarMatrix(length, width, depth);

        /*var row = -1;
        for (int col = -3; col <= 3; col++)
        {
            var rect = SrcCarMatrix[row, col];
            SrcCarMatrix[row, col] = Rect.MinMaxRect(rect.xMin, rect.yMin - (rect.width - rect.height), rect.xMax, rect.yMax);
        }*/

        ///
        destMatrix = CalcRectMatrix(length, width, depth);
    }

    void InitShape()
    {
        Panels = new CartesianMatrix<Panel>(5, 7);

        Shape = new Shape2D();

        List<List<Vector2>> bleedInChildren = new List<List<Vector2>>();

        foreach (Transform child in transform)
        {
            /*var mesh = child.GetComponent<MeshFilter>().sharedMesh;
            Shape.AddMesh(mesh);*/

            var panel = child.GetComponent<Panel>();
            if (!panel)
            {
                panel = child.gameObject.AddComponent<Panel>();
            }

            //panel.OnInit(this);

            panel.InitShape();
            Add(panel);

            bleedInChildren.Add(panel.Bleedline);

        }

        Outline = Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();
        Bleedline = PolygonAlgorithm.Merge(bleedInChildren);
    }

    public Rect[,] CalcRectMatrix(float length, float width, float depth)
    {

        Rect[,] result = new Rect[5, 7];

        var d_x = (length + width) * 0.5f;
        var d_y_0 = (depth + width) * 0.5f;
        var d_y_1 = (depth + length) * 0.5f;

        for (int row = -2, i = 0; row <= 2; row++, i++)
        {
            for (int col = -3, j = 0; col <= 3; col++, j++)
            {
                var c_x = col * d_x;
                var c_y = row * (col % 2 == 0 ? d_y_0 : d_y_1);// + 0.0059F;

                var w = col % 2 == 0 ? length : width;
                var h = row % 2 == 0 ? depth : (col % 2 == 0 ? width : length);

                var min_x = c_x - w * 0.5f;
                var max_x = c_x + w * 0.5f;
                var min_y = c_y - h * 0.5f;
                var max_y = c_y + h * 0.5f;

                //result[i, j] = Rect.MinMaxRect(min_x, min_y, max_x, max_y);
                result[i, j] = new Rect(min_x, min_y, w, h);
            }
        }

        return result;
    }

    public CartesianMatrix<Rect> CalcCarMatrix(float length, float width, float depth)
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

                //result[i, j] = Rect.MinMaxRect(min_x, min_y, max_x, max_y);
                result[row, col] = new Rect(min_x, min_y, w, h);
            }
        }

        return result;
    }

    public void Add(Panel panel)
    {

        panel.mPackage = this;
        panel.Face = Shape.AddPoints(panel.Outline.Select(p => new Point2D(p)).ToArray());

        for (int row = -2, i = 0; row <= 2; row++, i++)
        {
            for (int col = -3, j = 0; col <= 3; col++, j++)
            {
                //var rect = srcMatrix[i, j];
                var rect = SrcCarMatrix[row, col];
                //var vertices = rect.Vector2Array();
                if (CGAlgorithm.CN_PnPoly(panel.Center, rect.Vector2Array()) == 1)
                {
                    panel.InitDimension(rect);
                    Panels[row, col] = panel;
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
    private int offsetRow;// = 2;
    private int offsetCol;//= 3;

    private T[,] mPanels;// = new T[5, 7];

    public CartesianMatrix(int rowNum, int colNum)
    {
        offsetRow = rowNum / 2;
        offsetCol = colNum / 2;
        mPanels = new T[rowNum, colNum];
    }

    public T this[int row, int col]
    {
        set { mPanels[row + offsetRow, col + offsetCol] = value; }
        get { return mPanels[row + offsetRow, col + offsetCol]; }
    }
}
