using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Edge {

    public List<Face> Faces { set; get; }
    public Vector3 Point { set; get; }
    public Vector3 Axis { set; get; }
    public float Angle { set; get; }

    public bool IsBinded = false;
    public bool IsRotated = false;
    public bool IsResized = false;

    public Edge(Vector3 p0, Vector3 p1, float angle)
    {
        if (p0 == p1)
            throw new System.Exception("These two points are same position!!");

        Point = (p0 + p1) * 0.5F;
        Axis = (p1 - p0).normalized;

        Angle = angle;

        Faces = new List<Face>();

    }



    public void AddFace(Face face)
    {
        if (Faces.Contains(face))
            throw new System.Exception("There is two same edge!!");
        if(Faces.Count > 1)
            throw new System.Exception("There is already two faces!!");
        Faces.Add(face);
        face.Edges.Add(this);

        if(Faces.Count == 2)
        {
            var prep = CGAlgorithm.Perp(Axis, Vector2.right);
            var signX = -Mathf.Sign(Faces.Select(f => f.Row).Sum());
            var signY = Mathf.Sign(Faces.Select(f => f.Col).Sum());
            Axis = prep < 0.01F ? signX * Vector3.right : signY * Vector3.up;
        }
    }


}
