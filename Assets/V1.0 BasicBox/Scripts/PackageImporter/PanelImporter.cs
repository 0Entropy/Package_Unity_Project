using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PanelImporter : MonoBehaviour
{



    /// <summary>
    /// Grid_Y Value
    /// </summary>
    public int Row { set; get; }
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
