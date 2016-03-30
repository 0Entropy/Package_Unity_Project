using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Geometry;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PanelImporter : MonoBehaviour {



    /// <summary>
    /// Grid_Y Value
    /// </summary>
    public int Row { set; get; }
    /// <summary>
    /// Grid_x Value
    /// </summary>
    public int Col { set; get; }

    //public RectOffset Border { set; get; }
    public float Left { set; get; }
    public float Right { set; get; }
    public float Top { set; get; }
    public float Bottom { set; get; }

    public List<Vector3> Vertices { set; get; }

    public List<CreaseData> Creases { set; get; }

    private Shape2D _shape;
    public Shape2D Shape {
        get
        {
            if(_shape == null)
            {
                _shape = new Shape2D();
                _shape.AddMesh(GetComponent<MeshFilter>().sharedMesh);
            }
            return _shape;
        }
    }

    
    

    /*// Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}*/
}
