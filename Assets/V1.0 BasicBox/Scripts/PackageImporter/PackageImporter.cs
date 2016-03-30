using UnityEngine;
using System.Collections.Generic;
using Geometry;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PackageImporter : MonoBehaviour {

    public float Length = 10f;
    public float Width = 10f;
    public float Depth = 10f;
    public float Thickness = 1f;

    public List<Vector3> Vertices { set; get; }
    public List<int> Indices { set; get; }

    public List<PanelData> Panels = new List<PanelData>();

    public List<CreaseData> Creases = new List<CreaseData>();

    public Shape2D Shape { set; get; }

    /*// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}*/
}
