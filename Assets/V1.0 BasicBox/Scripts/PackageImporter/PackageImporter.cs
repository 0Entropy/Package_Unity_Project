using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using UnityEngine.UI;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PackageImporter : MonoBehaviour
{
    
    public float Thickness = 1f;

    public Vector3 Dimension;

    public List<Vector3> Vertices { set; get; }

    public List<int> Indices { set; get; }

    public List<PanelData> Panels = new List<PanelData>();
    
    public List<CreaseData> Creases = new List<CreaseData>();

    private Shape2D _shape;
    public Shape2D Shape
    {
        get
        {
            if (_shape == null)
            {

                _shape = new Shape2D();

                int i = 0;
                foreach (Transform child in transform)
                {
                    var mesh = child.GetComponent<MeshFilter>().sharedMesh;
                    _shape.AddMesh(mesh);

                    var panel = child.GetComponent<PanelImporter>();
                    if(panel == null)
                    {
                        panel = child.gameObject.AddComponent<PanelImporter>();
                    }
                    panel.ImPackage = this;

                    i++;
                }

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

            List<List<Vector2>> result = new List<List<Vector2>>();
            foreach (var child in GetComponentsInChildren<PanelImporter>())
            {
                result.Add(child.Bleedline);
            }

            return PolygonAlgorithm.Merge(result);

        }
    }

}
