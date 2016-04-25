using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class BoxContext : MonoBehaviour
{

    public TextAsset data;
    public Material mater;
    
    public GameObject box2D;
    public GameObject box3D;
    
    Box mBox;
    
    void Start()
    {


        mBox = new Box();
        var package = JsonFx.Json.JsonReader.Deserialize<PackageData>(data.text);
        //var package = JsonUtility.FromJson<PackageData>(data.text);

        mBox.OnInit(package);

        inputL.text = mBox.Length.ToString();
        inputW.text = mBox.Width.ToString();
        inputD.text = mBox.Depth.ToString();

        mBox.Draw3D(box3D, mater);
        
        mBox.Draw2D(box2D, mater);

        box2D.transform.Rotate(Vector3.up, 180);
    }


    public void OnResize(float length, float width, float depth, float thickness = 0.01f)
    {

        Length = length;
        Width = width;
        Depth = depth;

        Debug.Log(string.Format("[{0}, {1}, {2}]", length, width, depth));
        mBox.OnResize(length, width, depth, thickness);

        /* Draw3D();

         Draw2D();*/
        mBox.Draw3D(box3D, mater);

        mBox.Draw2D(box2D, mater);

    }
    
    #region 2D  

    void Draw2D()
    {
        foreach (var face in mBox.Faces)
        {
            if (!face.Object2D)
            {
                face.Object2D = new GameObject(string.Format("[{0},{1}]", face.Row, face.Col));
                face.Object2D.transform.SetParent(box2D.transform);
            }

            var face2D = face.Object2D.GetComponent<Mesh3DRenderer>();

            if (face2D == null)
            {
                face2D = face.Object2D.AddComponent<Mesh3DRenderer>();
                face2D.FaceType = Facing.FACE_BACK;
                face2D.IsTinkness = false;
                face2D.tinkness = 0.0F;
                face2D.GetComponent<MeshRenderer>().material = mater;
            }

            face2D.Clear();
            face2D.OuterPolygon.Add(face.Bleedline);
            face2D.UVRect = mBox.Boundry;
            face2D.UpdateMesh();
        }

    }
    #endregion
    void Draw3D()
    {
        foreach (var face in mBox.Faces)
        {
            if (!face.Object3D)
            {
                face.Object3D = new GameObject(string.Format("[{0},{1}]", face.Row, face.Col));

            }

            var face3D = face.Object3D.GetComponent<Mesh3DRenderer>();

            if (face3D == null)
            {
                face3D = face.Object3D.AddComponent<Mesh3DRenderer>();
                face3D.FaceType = Facing.DOUBLE_FACE;
                face3D.IsTinkness = true;
                face3D.tinkness = 0.016F;
                face3D.GetComponent<MeshRenderer>().material = mater;
            }

            face3D.Clear();
            face3D.OuterPolygon.Add(face.DestOutline);
            face3D.UVRect = mBox.Boundry;
            face3D.UpdateMesh();
        }
        
        mBox.OnBindFaces(box3D);
        
        mBox.OnRotateFaces();
    }

    /*void BindByEdge(Face face)
    {
        foreach (var f in face.Neighbors)
        {
            var edge = f.FindEdgeByFace(face);
            if (!edge.IsBinded)
            {
                f.Object3D.transform.SetParent(face.Object3D.transform);
                edge.IsBinded = true;
                BindByEdge(f);
            }
        }
    }*/

    void RotateAroundEdge()
    {
        foreach (var f in mBox.Faces.FindAll(f => f.IsNext).Distinct())
        {
            //Debug.Log(face.mTransform.name);
            var edge = f.Edges.Find(e => !e.IsRotated);

            var point = f.Translate(edge.Point);

            f.Object3D.transform.RotateAround(point, edge.Axis, edge.Angle);
            edge.IsRotated = true;
            //RotateByEdge(f);
        }
    }

    #region Unit Test

    float Length, Width, Depth;
    // Use this for initialization
    public InputField inputL, inputW, inputD;

    public void OnRefresh()
    {
        OnResize(Length, Width, Depth);
    }

    public void OnRandom()
    {
        Length = ((int)(Random.Range(1.0F, 3.0F) * 100)) * 0.01F;
        Width = ((int)(Random.Range(1.0F, 3.0F) * 100)) * 0.01F;
        Depth = ((int)(Random.Range(1.0F, 3.0F) * 100)) * 0.01F;

        OnResize(Length, Width, Depth);
    }

    public void PulsLength()
    {
        Length += 0.05F;
        OnResize(Length, Width, Depth);
    }

    public void PlusWidth()
    {
        Width += 0.05F;
        OnResize(Length, Width, Depth);
    }

    public void PlusDepth()
    {
        Depth += 0.05F;
        OnResize(Length, Width, Depth);
    }

    public void OnResize()
    {
        Length = float.Parse(inputL.text);
        Width = float.Parse(inputW.text);
        Depth = float.Parse(inputD.text);
        OnResize(Length, Width, Depth);

    }
    #endregion
}
