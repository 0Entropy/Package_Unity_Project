using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Geometry;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PackageImporter : MonoBehaviour
{

    public float Length = 10f;
    public float Width = 10f;
    public float Depth = 10f;
    public float Thickness = 1f;

    public List<Vector3> Vertices { set; get; }

    public List<int> Indices { set; get; }

    public List<PanelData> Panels = new List<PanelData>();

    public List<CreaseData> Creases = new List<CreaseData>();

    private Shape2D _shape;
    public Shape2D Shape
    {
        get
        {

            _shape = new Shape2D();
            //Debug.Log("Children Count : " + transform.childCount);

            int i = 0;
            foreach (Transform child in transform)
            {
                var mesh = child.GetComponent<MeshFilter>().sharedMesh;
                _shape.AddMesh(mesh);
                //Debug.Log(string.Format("index : {0}, mesh's point Count : {1}, points count : {2}", i, mesh.vertexCount, _shape.AllPoints.Count));
                i++;
            }

            //Debug.Log("Package i Add : " + i);



            return _shape;
        }
    }

    public List<Vector2> GetBleedLine(float bleedDimension)
    {
        //List<List<Vector2>> result = new List<List<Vector2>>();
        List<Vector2> result = new List<Vector2>();

        foreach (var child in transform.GetComponentsInChildren<PanelImporter>())
        {
            var bleed = CGAlgorithm.ScalePoly(child.Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList(), bleedDimension);
            if (result.Count == 0)
                result.AddRange(bleed);
            //CGAlgorithm.
            //result.Add(bleed);
        }

        //return PolygonAlgorithm.Merge(result);

        return null;
    }
    /*// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}*/
}
