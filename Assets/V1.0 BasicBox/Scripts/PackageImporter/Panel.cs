using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Panel : MonoBehaviour
{

    public PanelData Data
    {
        get
        {
            PanelData data = new PanelData();

            data.Right = Right;
            data.Left = Left;
            data.Top = Top;
            data.Bottom = Bottom;

            data.Vertices = Outline.Select(p => (SerializableVector3)((Vector3)p)).ToList();
            data.Alphas = Alphas.Select(a => (SerializableVector2)a).ToList();

            return data;
        }
    }
    public Package mPackage { set; get; }
    
    public int Row { set; get; }
    public int Col { set; get; }


    public float Right { get { return  BorderRect.xMin - DimRect.xMin; } }
    public float Left { get {return DimRect.xMax - BorderRect.xMax; } }
    public float Top { get { return DimRect.yMax - BorderRect.yMax; } }
    public float Bottom { get { return BorderRect.yMin - DimRect.yMin; } }

    /*public List<Vector3> Vertices { set; get; }

    public List<CreaseData> Creases { set; get; }*/

    public Rect DimRect { set; get; }
    //public Vector2[] DimArray { set; get; }
    public List<Vector2> DimPoints { set; get; }
    public List<Vector3> BorderPoints { set; get; }

    //private Shape2D Shape { set; get; }
    public Face2D Face { set; get; }

    public List<Vector2> Outline { set; get; }

    public List<Vector2> Bleedline { set; get; }

    public Rect BorderRect
    {
        set; get;
    }

    public Vector2 Center
    {
        get
        {
            var min_x = Outline.Min(p => p.x);
            var max_x = Outline.Max(p => p.x);
            var min_y = Outline.Min(p => p.y);
            var max_y = Outline.Max(p => p.y);

            var center_x = (max_x + min_x) * 0.5f;
            var center_y = (max_y + min_y) * 0.5f;

            return new Vector2(center_x, center_y);
        }
    }

    public List<Vector2> Alphas { set; get; }

    public Vector2 OffsetMin, OffsetMax;

    public List<Vector2> destVertices = new List<Vector2>();
    
    public void OnResize()
    {
        OnResize(OffsetMin, OffsetMax);
    }

    public void OnResize(Vector2 offsetMin, Vector2 offsetMax)
    {
        OffsetMin = offsetMin;
        OffsetMax = offsetMax;

        destVertices.Clear();

        for (int i = 0; i < Outline.Count; i++)
        {
            var aX = Alphas[i].x;
            var aY = Alphas[i].y;
            var dX = offsetMin.x * (1.0f - aX) + offsetMax.x * aX;
            var dY = offsetMin.y * (1.0f - aY) + offsetMax.y * aY;
            var x = Outline[i].x + dX;
            var y = Outline[i].y + dY;
            destVertices.Add(new Vector2(x, y));
        }
    }

    public void InitShape()
    {
        var Shape = new Shape2D();
        Shape.AddMesh(GetComponent<MeshFilter>().sharedMesh);

        Outline = Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();
        Bleedline = CGAlgorithm.ScalePoly(Outline, 0.03f);
    }

    public void InitDimension(Rect rect)
    {
        DimRect = new Rect(rect);
        DimPoints = new List<Vector2>(DimRect.Vector2Array());

        BorderRect = new Rect(rect);
        BorderPoints = new List<Vector3>(BorderRect.Vector3Array());
        
        Alphas = CalcAlphaFactor(DimRect);
        
    }

    /// <summary>
    /// 不同的Panel，其Alpha计算方法是不同的。
    /// 下次迭代时，可以做子类区分。
    /// </summary>
    /// <param name="BorderRect"></param>
    /// <returns></returns>
    List<Vector2> CalcAlphaFactor(Rect BorderRect)
    {
        List<Vector2> result = new List<Vector2>();
        //BorderRect = BorderPoints.BorderRect();
        foreach (var p in Outline)
        {
            var alphaX = BorderRect.width == 0 ? 1 : Mathf.Clamp((p.x - BorderRect.xMin) / BorderRect.width, 0, 1);
            var alphaY = BorderRect.height == 0 ? 1 : Mathf.Clamp((p.y - BorderRect.yMin) / BorderRect.height, 0, 1);
            
            result.Add(new Vector2(alphaX, alphaY));
        }
        return result;
    }

    public void UpdateBorder(List<Vector3> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            BorderPoints[i] = points[i];
        }
        BorderRect = BorderPoints.BorderRect();
        Alphas = CalcAlphaFactor(BorderRect);
        OnResize();
    }

}
