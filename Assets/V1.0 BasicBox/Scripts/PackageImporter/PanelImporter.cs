using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PanelImporter : MonoBehaviour
{
    public PackageImporter ImPackage { set; get; }



    /// <summary>
    /// Grid_Y Value
    /// </summary>
    public int Row;// { set; get; }
    /// <summary>
    /// Grid_x Value
    /// </summary>
    public int Col;// { set; get; }

    //public float Width { get { return BorderRect.size.x; } }
    //public float Height { get { return BorderRect.size.y; } }

    //public RectOffset Border { set; get; }
    public float Right { get { return  BorderRect.xMin - DimRect.xMin; } }
    public float Left { get {return DimRect.xMax - BorderRect.xMax; } }
    public float Top { get { return DimRect.yMax - BorderRect.yMax; } }
    public float Bottom { get { return BorderRect.yMin - DimRect.yMin; } }

    public List<Vector3> Vertices { set; get; }

    public List<CreaseData> Creases { set; get; }

    public Rect DimRect { set; get; }
    //public Vector2[] DimArray { set; get; }
    public List<Vector2> DimPoints { set; get; }
    public List<Vector3> BorderPoints { set; get; }

    private Shape2D Shape { set; get; }

    public List<Vector2> Outline { set; get; }

    public List<Vector2> Bleedline { set; get; }

    public Rect BorderRect
    {
        get
        {
            return BorderPoints.BorderRect();
        }
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

    public Vector2 offsetMin, offsetMax;

    public void OnResize()
    {
        ///var destRect = ImPackage.GetRectByRowAndCol(Row, Col);
        
        ImPackage.GetRectTransformByRowAndCol(Row, Col, ref offsetMin, ref offsetMax);

        
    }
    
    public void OnInit(PackageImporter package)
    {
        ImPackage = package;
        InitShape();
        //DimArray = package.GetDimVertices(Center, ref Row, ref Col);
        DimRect = package.GetDimVertices(Center, ref Row, ref Col);
        DimPoints = new List<Vector2>(DimRect.Vector2Array());
        //keyPoints = new List<Vector3>(DimArray.Select(p => (Vector3)p));
        BorderPoints = new List<Vector3>(DimRect.Vector3Array());
    }

    void InitShape()
    {
        Shape = new Shape2D();
        Shape.AddMesh(GetComponent<MeshFilter>().sharedMesh);

        Outline = Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();
        Bleedline = CGAlgorithm.ScalePoly(Outline, 0.03f);
    }

}
