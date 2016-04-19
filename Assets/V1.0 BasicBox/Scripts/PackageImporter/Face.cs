using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;

public class Face {
    
    public Box mBox { set; get; }

    public Transform mTransform { set; get; }

    public int Row { set; get; }
    public int Col { set; get; }

    public float Right { get { return ResizableRect.xMin - SrcDimRect.xMin; } }
    public float Left { get { return SrcDimRect.xMax - ResizableRect.xMax; } }
    public float Top { get { return SrcDimRect.yMax - ResizableRect.yMax; } }
    public float Bottom { get { return ResizableRect.yMin - SrcDimRect.yMin; } }

    public Rect SrcDimRect { set; get; }

    public Rect DestDimRect { set; get; }

    public Rect ResizableRect { set; get; }

    public Face2D ShapeFace { set; get; }

    public List<Vector2> Outline { set; get; }

    public List<Vector2> Bleedline { set; get; }
    
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

    public void InitShape( List<Vector2> vertices)
    {
        var Shape = new Shape2D();
        /*Shape.AddMesh(GetComponent<MeshFilter>().sharedMesh);*/
        Shape.AddPoints(vertices.Select(p => new Point2D(p)).ToArray());

        Outline = Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();

        destVertices = new List<Vector2>(Outline);

        Bleedline = CGAlgorithm.ScalePoly(Outline, 0.03f);
    }

    public void InitDimension(Rect rect)
    {
        SrcDimRect = new Rect(rect);
        //SrcDimPoints = new List<Vector2>(SrcDimRect.Vector2Array());

        ResizableRect = new Rect(rect);
        //BorderPoints = new List<Vector3>(BorderRect.Vector3Array());

        Alphas = CalcAlphaFactor(SrcDimRect);

    }

    public void UpdateDimension(Rect rect)
    {
        DestDimRect = new Rect(rect);
        var offsetMin = DestDimRect.min - SrcDimRect.min;
        var offsetMax = DestDimRect.max - SrcDimRect.max;
        OnResize(offsetMin, offsetMax);
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

        ResizableRect = points.BorderRect();
        Alphas = CalcAlphaFactor(ResizableRect);
        OnResize();
    }
}
