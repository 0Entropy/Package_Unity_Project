using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BoxContext : MonoBehaviour {

    public TextAsset data;

    public Mesh3DRenderer box2D;
    public GameObject box3D;

    Box mBox;
    
	// Use this for initialization
	void Start () {
        

        mBox = new Box();
         var package = JsonFx.Json.JsonReader.Deserialize<PackageData>(data.text);

        mBox.OnInit(package);

        box2D.Clear();
        box2D.OuterPolygon.Add(mBox.Bleedline);
        box2D.UpdateMesh();

        foreach(var face in mBox.Faces)
        {
            var obj = new GameObject();
            obj.transform.SetParent(box3D.transform);
            face.mTransform = obj.transform;
            var mesh3D = obj.AddComponent<Mesh3DRenderer>();
            mesh3D.FaceType = Facing.DOUBLE_FACE;
            mesh3D.IsTinkness = true;
            mesh3D.tinkness = 0.01F;

            mesh3D.Clear();
            mesh3D.OuterPolygon.Add(face.Outline);
            mesh3D.UpdateMesh();
        }

        /*List<List<Vector2>> bleedInChildren = new List<List<Vector2>>();
        
        foreach (var panel in package.Panels)
        {
            var vertices = panel.Vertices.Select(p => new Vector2(p.x, p.y)).ToList();
            
            var obj = new GameObject();
            obj.transform.SetParent(box3D.transform);
            var mesh3D = obj.AddComponent<Mesh3DRenderer>();
            mesh3D.FaceType = Facing.DOUBLE_FACE;
            mesh3D.IsTinkness = true;
            mesh3D.tinkness = 0.01F;

            mesh3D.Clear();
            mesh3D.OuterPolygon.Add(vertices);
            mesh3D.UpdateMesh();

            var bleed = CGAlgorithm.ScalePoly(vertices, 0.03f);
            bleedInChildren.Add(bleed);

        }
        var bleedLine = PolygonAlgorithm.Merge(bleedInChildren);
        box2D.OuterPolygon.Add(bleedLine);
       */

        var first = mBox.LookUp(0, 0);

        foreach(var crease in package.Creases)
        {
            if((crease.Neighbors[0][0] == 0 && crease.Neighbors[0][1] == 0) || 
                (crease.Neighbors[1][0] == 0 && crease.Neighbors[1][1] == 0))
            {
                Debug.Log(crease.FoldAngle);
                var coor = crease.Neighbors.Find(list => !(list[0] == 0 && list[1] == 0));
                var face = mBox.LookUp(coor[0], coor[1]);
                var trans = face.mTransform;
                trans.RotateAround((Vector3)crease.Vertices[0],
                    ((Vector3)crease.Vertices[1] - (Vector3)crease.Vertices[0]).normalized, crease.FoldAngle);
            }
        }

    }


	
	// Update is called once per frame
	void Update () {
	
	}
}
