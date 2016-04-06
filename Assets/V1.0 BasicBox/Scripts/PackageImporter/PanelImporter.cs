using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PanelImporter : MonoBehaviour
{
    public PackageImporter ImPackage { set; get; }

    public void TestRowAndCol()
    {
        var center = BorderRect.center;
        var l = ImPackage.Dimension.x;
        var w = ImPackage.Dimension.z;
        var d = ImPackage.Dimension.y;
        var sign_x = Mathf.Sign(center.x);
        var sign_y = Mathf.Sign(center.y);
        
        var col = Mathf.RoundToInt((center.x) / (l + w) * 2.0f);
        
        var row = Mathf.RoundToInt((center.y) / (d + (col % 2 == 0 ? w : l)) * 2.0f);

        Debug.Log(string.Format("Col : {0}, Row : {1};", col, row));
    }

    /// <summary>
    /// Grid_Y Value
    /// </summary>
    public int Row
    {
        get
        {
            var actual_center_x = BorderRect.center.x;

            return 0;
        }
    }
    /// <summary>
    /// Grid_x Value
    /// </summary>
    public int Col { set; get; }



    public float Width { get { return BorderRect.size.x; } }
    public float Height { get { return BorderRect.size.y; } }

    //public RectOffset Border { set; get; }
    public float Left { set; get; }
    public float Right { set; get; }
    public float Top { set; get; }
    public float Bottom { set; get; }

    public List<Vector3> Vertices { set; get; }

    public List<CreaseData> Creases { set; get; }

    public List<Vector3> keyPoints { set; get; }



    private Shape2D _shape;
    public Shape2D Shape
    {
        get
        {
            if (_shape == null)
            {
                _shape = new Shape2D();
                _shape.AddMesh(GetComponent<MeshFilter>().sharedMesh);
                keyPoints = new List<Vector3>(BorderArray);
            }
            return _shape;
        }
    }



    public List<Vector2> Outline
    {
        get
        {
            return Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();

        }
    }

    public List<Vector2> Bleedline
    {
        get
        {
            return CGAlgorithm.ScalePoly(Outline, 0.03f);
        }
    }

    public Rect BorderRect
    {
        get
        {
            var min_x = Outline.Min(p => p.x);
            var max_x = Outline.Max(p => p.x);
            var min_y = Outline.Min(p => p.y);
            var max_y = Outline.Max(p => p.y);

            return Rect.MinMaxRect(min_x, min_y, max_x, max_y);

        }
    }

    public Rect OriginalRect
    {
        get
        {
            return new Rect();
        }
    }

    public Vector3[] BorderArray
    {
        get
        {
            var min_x = Outline.Min(p => p.x);
            var max_x = Outline.Max(p => p.x);
            var min_y = Outline.Min(p => p.y);
            var max_y = Outline.Max(p => p.y);

            return new Vector3[]
            {
                new Vector2(min_x, min_y),
                new Vector2(min_x, max_y),
                new Vector2(max_x, max_y),
                new Vector2(max_x, min_y)
            };
        }
    }

}
