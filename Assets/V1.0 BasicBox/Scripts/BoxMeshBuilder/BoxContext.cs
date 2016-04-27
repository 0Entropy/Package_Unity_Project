using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class BoxContext : MonoBehaviour
{

    public Material innerMat, outerMat, sideMat;

    public Camera contentCamera;

    public Material mater;
    
    public GameObject box2D;
    public GameObject box3D;
    
    public Box mBox { set; get; }
    
    public void OnInit(string data)
    {
        

        mBox = new Box();

        OnClear();

        var package = JsonFx.Json.JsonReader.Deserialize<PackageData>(data);

        mBox.OnInit(package);

        //Draw3D();
        Draw3DWithMaterial();
        Draw2D();
        
        box2D.transform.Rotate(Vector3.up, 180);

        UpdateCamera();
    }

    public void OnResize(float length, float width, float depth, float thickness = 0.01f)
    {

        Length = length;
        Width = width;
        Depth = depth;
        
        mBox.OnResize(length, width, depth, thickness);

        //Draw3D();
        Draw3DWithMaterial();
        Draw2D();

        //Debug.Log(mBox.Boundry + ", " + mBox.Boundry.center);

        UpdateCamera();

    }

    void UpdateCamera()
    {
        
        if (!contentCamera.orthographic)
            contentCamera.orthographic = true;
        var size = Mathf.Max(mBox.Boundry.height, mBox.Boundry.width) * 0.5F;
        contentCamera.orthographicSize =size;
        var x = -mBox.Boundry.center.x + (size - mBox.Boundry.width * 0.5F);
        var y = mBox.Boundry.center.y + (size - mBox.Boundry.height * 0.5F);
        var z = contentCamera.transform.position.z;
        contentCamera.transform.position = new Vector3(x, y, z);
    }


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
                face2D.FaceType = Facing.FACE_FORWARD;
                face2D.thickness = 0.0F;
                face2D.GetComponent<MeshRenderer>().material = mater;
            }

            face2D.Clear();
            face2D.OuterPolygon.Add(face.Bleedline);
            face2D.UVRect = mBox.Boundry;
            face2D.UpdateMesh();
        }

    }

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
                face3D.thickness = 0.016F;
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

    void Draw3DWithMaterial()
    {
        foreach (var face in mBox.Faces)
        {
            if (!face.Object3D)
            {
                face.Object3D = new GameObject(string.Format("[{0},{1}]", face.Row, face.Col));

            }

            var face3D = face.Object3D.GetComponent<Box3DRenderer>();

            if (face3D == null)
            {
                face3D = face.Object3D.AddComponent<Box3DRenderer>();
                face3D.OnInit();
                face3D.outerMat = outerMat;
                face3D.innerMat = innerMat;
                face3D.sideMat = sideMat;
            }

            face3D.Clear();
            face3D.AddOuter(face.DestOutline);
            face3D.SetBoundry(mBox.Boundry);
            face3D.UpdateMesh();
        }

        mBox.OnBindFaces(box3D);

        mBox.OnRotateFaces();
    }

    void OnClear()
    {
        if (box2D.transform.childCount == 0 && box3D.transform.childCount == 0)
            return;

        GameObject temp3D = new GameObject(box2D.name);
        GameObject temp2D = new GameObject(box3D.name);
        temp2D.transform.CopyFrom(box2D);
        temp3D.transform.CopyFrom(box3D);
        GameObject.DestroyImmediate(box2D);
        GameObject.DestroyImmediate(box3D);

        box2D = temp2D;
        box3D = temp3D;
    }

    #region Unit Test

    float Length, Width, Depth;

    public InputField inputL, inputW, inputD;

    public TextAsset data;

    void Start()
    {
        OnInit(data.text);
        
        inputL.text = mBox.Length.ToString();
        inputW.text = mBox.Width.ToString();
        inputD.text = mBox.Depth.ToString();
    }

    public void OnInit()
    {
        OnInit(data.text);
    }

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
