using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Geometry;

public class Crease : MonoBehaviour
{

    public List<Edge2D> Edges { set; get; }
    public List<List<Vector3>> Segments { get { return Edges.Select(e => e.Points.Select(p => p.Position).ToList()).ToList(); } }
    public List<float> Angles { set; get; }
    //public List<Crease> Creases { set; get; }
    public List<CreaseData> Data
    {
        get
        {
            List<CreaseData> data = new List<CreaseData>();

            for(int i =0; i<Edges.Count; i++)
            {
                var e = Edges[i];
                CreaseData creaseData = new CreaseData();
                creaseData.Vertices = e.Points.Select(p => (SerializableVector3)p.Position).ToList();
                creaseData.FoldAngle = Angles[i];
                creaseData.Neighbors = new List<List<int>>();
                foreach (var f in e.Faces)
                {
                    var coor = new List<int>( GetComponent<Package>().LookUpCoordinate(f));
                    creaseData.Neighbors.Add(coor);
                    //Debug.Log(string.Format("Coordinate {0}, {1}", coor[0], coor[1]));
                }
                data.Add(creaseData);
            }

            return data;
        }
    }

    public void OnInit()
    {
        /*Segments = GetComponent<Package>().Shape.Edges.FindAll(e => e.Faces.Count == 2).Distinct().
            Select(e => e.Points.Select(p => p.Position).ToList()).
            ToList();*/

        Edges = GetComponent<Package>().Shape.Edges.FindAll(e => e.Faces.Count == 2).Distinct().ToList();
        //Segments = Edges.Select(e => e.Points.Select(p => p.Position).ToList()).ToList();

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
        //Segments = Edges.Select(e => e.Points.Select(p => p.Position).ToList()).ToList();
    }
}
