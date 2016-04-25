using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Panel : MonoBehaviour
{
    //public Face mFace { set; get; }

    public PanelData Data
    {
        get
        {
            PanelData data = new PanelData();

            data.Row = Row;
            data.Col = Col;

            data.Right = SrcRect.xMin;
            data.Left = SrcRect.xMax;
            data.Top = SrcRect.yMax;
            data.Bottom = SrcRect.yMin;

            data.VarRight = VarRect.xMin;
            data.VarLeft = VarRect.xMax;
            data.VarTop = VarRect.yMax;
            data.VarBottom = VarRect.yMin;

            data.Vertices = Outline.Select(p => (SerializableVector2)p).ToList();
            data.Alphas = Alphas.Select(a => (SerializableVector2)a).ToList();

            return data;
        }
    }

    public Package mPackage { set; get; }//

    public int Row { set; get; }//{ set { mFace.Row = value; }  get { return mFace.Row; } }
    public int Col { set; get; }//{ set { mFace.Col = value; } get { return mFace.Col; } }

    public float Right {  get { return VarRect.xMin - SrcRect.xMin; } }
    public float Left {get { return SrcRect.xMax - VarRect.xMax; } }
    public float Top {  get { return SrcRect.yMax - VarRect.yMax; } }
    public float Bottom {  get { return VarRect.yMin - SrcRect.yMin; } }


    public Rect SrcRect { set; get; }// { get { return mFace.SrcDimRect; } }

    public Rect DestRect { set; get; }// { get { return mFace.DestDimRect; } }

    public Rect VarRect { set; get; }//{ get { return mFace.ResizableRect; } }

    public Face2D Face { set; get; }

    public List<Vector2> Outline { set; get; }// { get { return mFace.Outline; } }

    public List<Vector2> Bleedline { set; get; }//{ get { return mFace.Bleedline; } }


    public Vector2 Center
    {
        get
        {
            var min_x = Outline.Min(p => p.x);
            var max_x = Outline.Max(p => p.x);
            var min_y = Outline.Min(p => p.y);
            var max_y = Outline.Max(p => p.y);

            var center_x = (max_x + min_x) * 0.5f;
            var center_y = (max_y + min_y) * 0.5f;

            return new Vector2(center_x, center_y);
        }
    }

    public List<Vector2> Alphas { set; get; }//{ get { return mFace.Alphas; } }

    public Vector2 OffsetMin, OffsetMax;

    public List<Vector2> destVertices { set; get; }// { get { return mFace.destVertices; } }


    public void OnResize()
    {
        OnResize(OffsetMin, OffsetMax);
    }

    public void OnResize(Vector2 offsetMin, Vector2 offsetMax)
    {
        OffsetMin = offsetMin;
        OffsetMax = offsetMax;

        destVertices.Clear();

        for (int i = 0; i < Outline.Count; i++)
        {
            var aX = Alphas[i].x;
            var aY = Alphas[i].y;
            var dX = offsetMin.x * (1.0f - aX) + offsetMax.x * aX;
            var dY = offsetMin.y * (1.0f - aY) + offsetMax.y * aY;
            var x = Outline[i].x + dX;
            var y = Outline[i].y + dY;
            destVertices.Add(new Vector2(x, y));
        }

        Bleedline = CGAlgorithm.ScalePoly(destVertices, 0.05f);
    }

    public void InitShape()
    {
        var Shape = new Shape2D();
        Shape.AddMesh(GetComponent<MeshFilter>().sharedMesh);

        /*mFace = new Face();
        mFace.InitOutline(Shape.OutlinePoints.Select(p => p.Position).ToList());*/

        Outline = Shape.OutlinePoints.Select(p => (Vector2)p.Position).ToList();

        destVertices = new List<Vector2>(Outline);

        Bleedline = CGAlgorithm.ScalePoly(Outline, 0.05f);/*
        Bleedline = CGAlgorithm.ScalePoly(Bleedline, 0.001f);
        Bleedline = CGAlgorithm.ScalePoly(Bleedline, 0.001f);*/
    }

    public void InitDimension(Rect rect)
    {
        //mFace.InitDimension(rect);

        SrcRect = new Rect(rect);

        VarRect = new Rect(rect);

        Alphas = CalcAlphaFactor(SrcRect);

    }

    public void UpdateDimension(Rect rect)
    {
        //mFace.UpdateDimension(rect);
        DestRect = new Rect(rect);
        var offsetMin = DestRect.min - SrcRect.min;
        var offsetMax = DestRect.max - SrcRect.max;
        OnResize(offsetMin, offsetMax);
    }

    /// <summary>
    /// 不同的Panel，其Alpha计算方法是不同的。
    /// 下次迭代时，可以做子类区分。
    /// </summary>
    /// <param name="BorderRect"></param>
    /// <returns></returns>

    List<Vector2> CalcAlphaFactor(Rect variable)
    {
        List<Vector2> result = new List<Vector2>();
        //BorderRect = BorderPoints.BorderRect();
        foreach (var p in Outline)
        {
            var alphaX = variable.width == 0 ? 1 : Mathf.Clamp((p.x - variable.xMin) / variable.width, 0, 1);
            var alphaY = variable.height == 0 ? 1 : Mathf.Clamp((p.y - variable.yMin) / variable.height, 0, 1);

            result.Add(new Vector2(alphaX, alphaY));
        }
        return result;
    }

    public void UpdateBorder(List<Vector3> points)
    {
        //mFace.UpdateBorder(points);
        VarRect = points.BorderRect();
        Alphas = CalcAlphaFactor(VarRect);
        OnResize();
    }

}
