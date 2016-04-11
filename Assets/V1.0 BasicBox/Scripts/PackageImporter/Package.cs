using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using UnityEngine.UI;
using System.Collections;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Package : MonoBehaviour
{



    //public GameObject InitMesh { set; get; }

    //public List<PanelData> Panels = new List<PanelData>();

    //public List<CreaseData> Creases = new List<CreaseData>();
    
    //public List<Vector3> Vertices { set; get; }

    //public List<int> Indices { set; get; }

    public Shape2D Shape { set; get; }

    public List<Vector2> Outline { set; get; }

    public List<Vector2> Bleedline { set; get; }

    //public List<Dimension> Dimensions { set; get; }

    //public Vector3 InitDim { set; get; }

    public float Length { set; get; }
    public float Width { set; get; }
    public float Depth { set; get; }
    public float Thickness { set; get; }

    public Rect[,] RectMatrix { set; get; }

    public List<Crease> Creases { set; get; }

    public Panel[,] Panels { set; get; }

    #region Resize

    public Rect[,] ResizeMatrix { set; get; }

    public void OnResize(float length, float width, float depth, float thickness = 0.01f)
    {
        ResizeMatrix = CalcRectMatrix(length, width, depth);

        foreach(Transform child in transform)
        {
            var panel = child.GetComponent<Panel>();
            if (!panel)
            {
                //panel = child.gameObject.AddComponent<PanelImporter>();
                continue;
            }
            panel.OnResize();
        }
    }

    #endregion

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

        RectMatrix = CalcRectMatrix(length, width, depth);
        ResizeMatrix = CalcRectMatrix(length, width, depth);
    }

    void InitShape()
    {
        Shape = new Shape2D();

        List<List<Vector2>> bleedInChildren = new List<List<Vector2>>();

        foreach (Transform child in transform)
        {
            var mesh = child.GetComponent<MeshFilter>().sharedMesh;
            Shape.AddMesh(mesh);

            var panel = child.GetComponent<Panel>();
            if (!panel)
            {
                panel = child.gameObject.AddComponent<Panel>();
            }

            panel.OnInit(this);

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
                var c_y = row * (col % 2 == 0 ? d_y_0 : d_y_1)+ 0.0059F;

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

    public Rect GetDimVertices(Vector2 position, ref int r, ref int c)
    {

        for (int row = -2, i = 0; row <= 2; row++, i++)
        {
            for (int col = -3, j = 0; col <= 3; col++, j++)
            {
                var rect = RectMatrix[i, j];
                //var vertices = rect.Vector2Array();
                if (CGAlgorithm.CN_PnPoly(position, rect.Vector2Array()) == 1)
                {
                    r = row;
                    c = col;
                    return rect;
                }
            }
        }
        
        throw new System.Exception("NO SUCH RECTANGLE ");
    }

    public void GetRectTransformByRowAndCol(int row, int col, ref Vector2 offsetMin, ref Vector2 offsetMax)
    {
        if(ResizeMatrix == null || RectMatrix == null)
        {
            return;
        }

        var i = row + 2;
        var j = col + 3;

        offsetMin = ResizeMatrix[i, j].min - RectMatrix[i, j].min;
        offsetMax = ResizeMatrix[i,j].max - RectMatrix[i, j].max;
        ///return new RectOffset(max.x, min.x, max.y, min.y);
    }
}

