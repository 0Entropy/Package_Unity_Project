using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BoxContext : MonoBehaviour {

    public TextAsset data;
    public Material mater;

    public Mesh3DRenderer box2D;
    public GameObject box3D;

    Box mBox;
    
	// Use this for initialization
	void Start () {
        

        mBox = new Box();
        var package = JsonFx.Json.JsonReader.Deserialize<PackageData>(data.text);
        //var package = JsonUtility.FromJson<PackageData>(data.text);

        mBox.OnInit(package);

        Draw2D();

        Draw3D();
        
    }

    public void OnPlusOne()
    {
        var l = Random.Range(1.0F, 3.0F);
        var w = Random.Range(1.0F, 3.0F);
        var d = Random.Range(1.0F, 3.0F);
        OnResize(l, w, d);
        Debug.Log(string.Format("[{0}, {1}, {2}]", l, w, d));
    }

    public void OnResize(float length, float width, float depth, float thickness = 0.01f)
    {
        mBox.OnResize(length, width, depth, thickness);
        foreach (var face in mBox.Faces)
        {

            face.mObject.transform.position = Vector3.zero;
            face.mObject.transform.rotation = Quaternion.identity;
        }

        foreach (var edge in mBox.Edges)
            edge.IsRotated = false;
        Draw2D();
        Draw3D();
    }

    void Draw2D()
    {
        box2D.Clear();
        box2D.OuterPolygon.Add(mBox.Bleedline);
        box2D.UpdateMesh();
        box2D.GetComponent<MeshRenderer>().material = mater;
    }

    void Draw3D()
    {
        foreach (var face in mBox.Faces)
        {
            if (!face.mObject)
            {
                face.mObject = new GameObject(string.Format("[{0},{1}]", face.Row, face.Col));
                
            }

            var face3D = face.mObject.GetComponent<Mesh3DRenderer>();

            if(face3D == null)
            {
                face3D = face.mObject.AddComponent<Mesh3DRenderer>();
                face3D.FaceType = Facing.DOUBLE_FACE;
                face3D.IsTinkness = true;
                face3D.tinkness = 0.016F;
                face3D.GetComponent<MeshRenderer>().material = mater;
            }

            face3D.Clear();
            face3D.OuterPolygon.Add(face.DestOutline);
            face3D.UpdateMesh();
        }
        
        if(mBox.IsNotBinded)
        {

            var root = mBox.Faces.Find(f => f.IsRoot);
            root.mObject.transform.SetParent(box3D.transform);
            BindByEdge(root);
        }

        while (mBox.Faces.FindAll(f => f.IsNext).Count > 0)
        {
            RotateAroundEdge();
        }
    }

    void BindByEdge(Face face)
    {
        foreach(var f in face.Neighbors)
        {
            var edge = f.FindEdgeByFace(face);
            if(!edge.IsBinded)
            {
                f.mObject.transform.SetParent(face.mObject.transform);
                edge.IsBinded = true;
                BindByEdge(f);
            }
        }
    }

    void RotateAroundEdge()
    {
        foreach (var f in mBox.Faces.FindAll(f => f.IsNext).Distinct())
        {
            //Debug.Log(face.mTransform.name);
            var edge = f.Edges.Find(e => !e.IsRotated);

            var point = f.Translate(edge.Point);
            
            f.mObject.transform.RotateAround(point, edge.Axis, edge.Angle);
            edge.IsRotated = true;
            //RotateByEdge(f);
        }
    }

    /*void RotateByEdge(Face face)
    {
        foreach(var f in face.Neighbors)
        {
            var edge = f.FindEdgeByFace(face);
            if (!edge.IsRotated)
            {
                f.mTransform.RotateAround(edge.Point, edge.Axis, edge.Angle);
                edge.IsRotated = true;
                RotateByEdge(f);
            }
        }
    }*/
	
	// Update is called once per frame
	void Update () {
	
	}
}
