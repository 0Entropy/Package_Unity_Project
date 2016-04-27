using UnityEngine;
using System.Collections.Generic;

public class Box3DRenderer : MonoBehaviour
{

    public Material innerMat { set { innerMesh.GetComponent<MeshRenderer>().material = value; } }
    public Material outerMat { set { outerMesh.GetComponent<MeshRenderer>().material = value; } }
    public Material sideMat { set { sideMesh.GetComponent<MeshRenderer>().material = value; } }

    Mesh3DRenderer innerMesh { set; get; }
    Mesh3DRenderer outerMesh { set; get; }
    Mesh3DRenderer sideMesh { set; get; }

    public void OnInit()
    {
        var innerObj = new GameObject("inner");
        innerObj.transform.SetParent(transform);
        innerMesh = innerObj.AddComponent<Mesh3DRenderer>();
        innerMesh.FaceType = Facing.FACE_BACK;
        innerMesh.thickness = 0.02F;

        var outerObj = new GameObject("outer");
        outerObj.transform.SetParent(transform);
        outerMesh = outerObj.AddComponent<Mesh3DRenderer>();
        outerMesh.FaceType = Facing.FACE_FORWARD;
        outerMesh.thickness = 0;

        var sideObj = new GameObject("side");
        sideObj.transform.SetParent(transform);
        sideMesh = sideObj.AddComponent<Mesh3DRenderer>();
        sideMesh.FaceType = Facing.NULL;
        sideMesh.thickness = 0.02F;

    }

    public void Clear()
    {
        innerMesh.Clear();
        outerMesh.Clear();
        sideMesh.Clear();
    }

    public void AddOuter(List<Vector2> poly)
    {
        innerMesh.OuterPolygon.Add(poly);
        outerMesh.OuterPolygon.Add(poly);
        sideMesh.OuterPolygon.Add(poly);
    }

    public void SetBoundry(Rect rect)
    {
        innerMesh.UVRect = rect;
        outerMesh.UVRect = rect;
        sideMesh.UVRect = rect;
    }

    public void UpdateMesh()
    {
        innerMesh.UpdateMesh();
        outerMesh.UpdateMesh();
        sideMesh.UpdateMesh();
    }
}
