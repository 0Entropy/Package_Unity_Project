using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Geometry;

public class Crease : MonoBehaviour
{

    public List<List<Vector3>> Segments { set; get; }
    public List<float> Angles { set; get; }
    //public List<Crease> Creases { set; get; }

    public List<Edge2D> Edges { set; get; }

    public void OnInit()
    {
        /*Segments = GetComponent<Package>().Shape.Edges.FindAll(e => e.Faces.Count == 2).Distinct().
            Select(e => e.Points.Select(p => p.Position).ToList()).
            ToList();*/

        Edges = GetComponent<Package>().Shape.Edges.FindAll(e => e.Faces.Count == 2).Distinct().ToList();
        Segments = Edges.Select(e => e.Points.Select(p => p.Position).ToList()).ToList();

        Angles = new List<float>(Edges.Count);
        for(int i = 0; i<Edges.Count; i++)
        {
            Angles.Add(90);
        }
    }

    public void RemoveByIndices(List<int> indices)
    {
        for (int i = indices.Count - 1; i >= 0; i--)
        {
            var index = indices[i];
            Edges.RemoveAt(index);
            Angles.RemoveAt(index);
        }
        Debug.Log("Edges.Count : " + Edges.Count);
        Segments = Edges.Select(e => e.Points.Select(p => p.Position).ToList()).ToList();
    }
}
