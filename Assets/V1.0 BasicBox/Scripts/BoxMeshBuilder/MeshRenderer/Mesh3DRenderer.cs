using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;

[Flags]
public enum Facing
{
    DOUBLE_FACE = 3,
    FACE_BACK = 1,
    FACE_FORWARD = 2
}


public class Mesh3DRenderer : AbstractMeshRenderer
{

    public List<List<Vector2>> OuterPolygon { set; get; }
    public List<List<Vector2>> InnerHole { set; get; }

    public Facing FaceType = Facing.FACE_BACK;
    public bool IsTinkness = false;


    public float tinkness = 0.06f;

    public Rect UVRect { set; get; }

    public override void Clear()
    {
        base.Clear();

        if (OuterPolygon == null)
            OuterPolygon = new List<List<Vector2>>();
        else
            OuterPolygon.Clear();

        if (InnerHole == null)
            InnerHole = new List<List<Vector2>>();
        else
            InnerHole.Clear();
    }

    protected override void HandleOnUpdate()
    {

        for (int i = 0; i < OuterPolygon.Count; i++)
        {
            //List<Vector2> union = PolygonAlgorithm.Combine(OuterPolygon[i], new List<List<Vector2>>() { InnerHole[i] });
            var union = OuterPolygon[i];
            // var inner = CGAlgorithm.ScalePoly(union, -0.1F);
            var trian = ForceEarCut.ComputeTriangles(union);
            if ((FaceType & Facing.FACE_BACK) == Facing.FACE_BACK)
                CalcFaceMeshData(union, trian, Facing.FACE_FORWARD, tinkness, ref vertices, ref normals, ref uv, ref triangles);
            if ((FaceType & Facing.FACE_BACK) == Facing.FACE_BACK)
                CalcFaceMeshData(union, trian, Facing.FACE_BACK, tinkness, ref vertices, ref normals, ref uv, ref triangles);
        }

        if (IsTinkness)
        {
            foreach (var outer in OuterPolygon)
            {
                //outer.Reverse();
                CalcSideMeshData(outer, tinkness, ref vertices, ref normals, ref uv, ref triangles);
            }

            /*foreach (var inner in InnerHole)
            {
                CalcSideMeshData(inner, tinkness, ref vertices, ref normals, ref uv, ref triangles);
            }*/
        }

    }

    public void CalcSideMeshData(List<Vector2> points, float offset, ref List<Vector3> vertices, ref List<Vector3> normals, ref List<Vector2> uv, ref List<int> triangles)
    {

        int count = points.Count;
        int offsetIndex = vertices.Count;
        var offsetBack = Vector3.back * offset;
        var offsetForward = Vector3.forward * 0.0000001F;
        float shapeDepth = offset;// * 2.0F;//Mathf.Abs(offset) * 2.0F;

        for (int i = 0; i < count; i++)
        {
            //  + offsetPos		1'b-----------------------0'a
            //  |				        |		        					|
            //  i+1 <--- i		    |				        			|
            //  |				        |			        				|
            //  - offsetPos		    2'c-----------------------3'd
            //

            Vector3 p0 = (Vector3)points[i];
            Vector3 p1 = (Vector3)points[(i + 1) % count];

            vertices.Add(p0 + offsetForward);
            vertices.Add(p1 + offsetForward);
            vertices.Add(p1 + offsetBack);
            vertices.Add(p0 + offsetBack);

            Vector3 dir = p1 - p0;
            Vector3 tangent = Vector3.back;
            Vector3 normal = Vector3.zero;

            Vector3.OrthoNormalize(ref dir, ref tangent, ref normal);

            for (int j = 0; j < 4; j++)
            {
                normals.Add(normal);
            }

            float dis = Vector3.Distance(p0, p1);
            float f = dis / shapeDepth;

            uv.Add(new Vector2(0, 0));
            uv.Add(new Vector2(0, f));
            uv.Add(new Vector2(1, f));
            uv.Add(new Vector2(1, 0));
        }

        //Build triangles array
        for (int a = 0, n = count * 4; a < n; a += 4)
        {
            var b = (a + 1) % n;
            var c = (a + 2) % n;
            var d = (a + 3) % n;
            triangles.Add(offsetIndex + a);
            triangles.Add(offsetIndex + c);
            triangles.Add(offsetIndex + b);
            triangles.Add(offsetIndex + a);
            triangles.Add(offsetIndex + d);
            triangles.Add(offsetIndex + c);
        }
    }

    public void CalcFaceMeshData(List<Vector2> points, List<int> triangles, Facing facing, float thickness, ref List<Vector3> verts, ref List<Vector3> normals, ref List<Vector2> uvs, ref List<int> tris)
    {


        int count = points.Count;
        int offsetIndex = verts.Count;
        float thick = facing == Facing.FACE_FORWARD ? 0.0000001F : -thickness;
        Vector3 normal = facing == Facing.FACE_FORWARD ? Vector3.forward : Vector3.back;

        /*var min_x = points.Min(p => p.x);
        var max_x = points.Max(p => p.x);
        var min_y = points.Min(p => p.y);
        var max_y = points.Max(p => p.y);

        var w = max_x - min_x;
        var h = max_y - min_y;*/

        for (int i = 0; i < count; i++)
        {
            Vector2 pt = points[i];
            verts.Add((Vector3)pt + Vector3.forward * thick);
            normals.Add(normal);
            if(UVRect != null)
            uvs.Add(new Vector2(( UVRect.xMax - pt.x) /UVRect.width, (pt.y - UVRect.yMin)/UVRect.width));
            //uvs.Add(new Vector2(pt.x / w, pt.y / h));
        }

        for (int i = 0; i < triangles.Count; i += 3)
        {
            if (facing == Facing.FACE_FORWARD)
            {
                tris.Add(offsetIndex + triangles[i]);
                tris.Add(offsetIndex + triangles[i + 2]);
                tris.Add(offsetIndex + triangles[i + 1]);
            }
            else if (facing == Facing.FACE_BACK)
            {
                tris.Add(offsetIndex + triangles[i]);
                tris.Add(offsetIndex + triangles[i + 1]);
                tris.Add(offsetIndex + triangles[i + 2]);
            }
        }
    }
}
