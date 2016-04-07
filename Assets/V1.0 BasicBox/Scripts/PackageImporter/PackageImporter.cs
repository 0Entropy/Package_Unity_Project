using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using UnityEngine.UI;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PackageImporter : MonoBehaviour
{

    public float Thickness = 1f;

    //public Vector3 Dimension;

    public float Length { set; get; }
    public float Width { set; get; }
    public float Depth { set; get; }

    public List<Vector3> Vertices { set; get; }

    public List<int> Indices { set; get; }

    public List<PanelData> Panels = new List<PanelData>();

    public List<CreaseData> Creases = new List<CreaseData>();

    public void ResetShape()
    {
        Shape = new Shape2D();

        int i = 0;
        foreach (Transform child in transform)
        {
            var mesh = child.GetComponent<MeshFilter>().sharedMesh;
            Shape.AddMesh(mesh);

            var panel = child.GetComponent<PanelImporter>();
            if (panel == null)
            {
                panel = child.gameObject.AddComponent<PanelImporter>();
            }
            panel.ImPackage = this;

            i++;
        }

        //Outline = Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();
    }

    public Shape2D DimensionShape { set; get; }
    public List<Rectangle> DimenRects { set; get; }
    public void ResetDimension()
    {

        DimenRects = new List<Rectangle>();

        var d_x = (Length + Width) * 0.5f;
        var d_y_0 = (Depth + Width) * 0.5f;
        var d_y_1 = (Depth + Length) * 0.5f;

        for (int row = -2; row <= 2; row++)
        {
            for (int col = -3; col <= 3; col++)
            {
                var center_x = col * d_x;
                var center_y = row * (col % 2 == 0 ? d_y_0 : d_y_1);

                var width = col % 2 == 0 ? Length : Width;
                var height = row % 2 == 0 ? Depth : (col % 2 == 0 ? Width : Length);
                
                var min_x = center_x - width * 0.5f;
                var max_x = center_x + width * 0.5f;
                var min_y = center_y - height * 0.5f;
                var max_y = center_y + height * 0.5f;

                var vertices = new Vector2[]{
                    new Vector2(min_x, min_y),
                    new Vector2(min_x, max_y),
                    new Vector2(max_x, max_y),
                    new Vector2(max_x, min_y)
            };            //DimensionShape.AddPoints(p0, p1, p2, p3);

                DimenRects.Add(new Rectangle() { Col = col, Row = row, Vertices = vertices });// new List<Vector2>(vertices) });
            }
        }

    }

    //private Shape2D _shape;
    public Shape2D Shape
    {
        set; get;
    }

    public List<Vector2> Outline
    {
        get
        {
            return Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();
        }
    }

    public List<Vector2> DimensionOutline
    {
        get
        {
            return DimensionShape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();
        }
    }

    public List<Vector2> Bleedline
    {
        get
        {

            List<List<Vector2>> result = new List<List<Vector2>>();
            foreach (var child in GetComponentsInChildren<PanelImporter>())
            {
                result.Add(child.Bleedline);
            }

            return PolygonAlgorithm.Merge(result);

        }
    }

    public Vector2[] GetDimension(Vector2 position)
    {
        //dimension = new List<Vector2>();

        foreach(var dimen in DimenRects)
        {
            if(CGAlgorithm.CN_PnPoly(position, dimen.Vertices) == 1)
            {
                return dimen.Vertices;
            }
        }

        return null;
    }

}

public class Rectangle : Geometry.Shape2D
{
    public int Row { set; get; }
    public int Col { set; get; }

    public Vector2[] Vertices { set; get; }
}
