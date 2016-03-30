namespace Geometry
{

    using UnityEngine;
    using System.Collections.Generic;
    using System;
    using System.Linq;

    public class Shape2D
    {
        //private readonly _Lazy<List<Edge2D>> _lazyAllEdges;
        //private readonly _Lazy<List<Point2D>> _lazyAllPoints;
        //private readonly _Lazy<List<Edge2D>> _lazyExitedEdges;
        private readonly _Lazy<List<Point2D>> _lazyOutlinePoints;

        public Shape2D()
        {
            Faces = new List<Face2D>();
            Edges = new List<Edge2D>();
            Points = new List<Point2D>();

            //_lazyAllEdges = new _Lazy<List<Edge2D>>(GetAllEdges);
            //_lazyAllPoints = new _Lazy<List<Point2D>>(GetAllPoints);
            //_lazyExitedEdges = new _Lazy<List<Edge2D>>(GetAllExistedEdges);
            _lazyOutlinePoints = new _Lazy<List<Point2D>>(GetOutlinePoints);
        }

        public List<Face2D> Faces { get; set; }
        //public List<Edge2D> AllEdges { get { return _lazyAllEdges.Value; } }
        //public List<Point2D> AllPoints { get { return _lazyAllPoints.Value; } }
        //public List<Edge2D> AllExistedEdges { get { return _lazyExitedEdges.Value; } }
        public List<Point2D> OutlinePoints { get { return _lazyOutlinePoints.Value; } }

        public List<Edge2D> Edges { set; get; }

        public List<Point2D> Points { set; get; }

        public Face2D AddFace(Face2D face)
        {
            if (Faces.Any(f => f.IsMatchFor(face)))
            {
                throw new InvalidOperationException("There is already such a face in the shape!");
            }
            Faces.Add(face);
            face.Shape = this;
            //_lazyExitedEdges.isValeChanged = true;
            //_lazyAllEdges.isValeChanged = true;
            //_lazyAllPoints.isValeChanged = true;
            _lazyOutlinePoints.isValeChanged = true;
            //Debug.Log("Outline Count : " + OutlinePoints.Count);
            return face;
        }

        public void RemoveFace(Face2D face)
        {
            Faces.Remove(face);

            //_lazyExitedEdges.isValeChanged = true;
            //_lazyAllEdges.isValeChanged = true;
            //_lazyAllPoints.isValeChanged = true;
            _lazyOutlinePoints.isValeChanged = true;
            //face.BasicEdges.ForEach(e => e.Faces.Remove(face));
            /*_lazyAllEdges.isValeChanged = true;
             _lazyAllPoints.isValeChanged = true;*/
        }

        public void Clear()
        {
            Faces.Clear();

            _lazyOutlinePoints.isValeChanged = true;
            //_lazyExitedEdges.isValeChanged = true;
            //_lazyAllEdges.isValeChanged = true;
            //_lazyAllPoints.isValeChanged = true;
        }

        /*private List<Edge2D> GetAllEdges()
        {
            //return Faces.SelectMany(f => f.Edges).Distinct().ToList();
            return Edges;
        }*/

        /*private List<Point2D> GetAllPoints()
        {
            //return Faces.SelectMany(f => f.BasicPoints).Distinct().ToList();
            //return Faces.SelectMany(f => f.BasicPoints).Distinct().ToList();
            //return Edges.SelectMany(e => e.Points).Distinct().ToList();
            return Points;
        }*/

        /*private List<Edge2D> GetAllExistedEdges()
        {
            List<Edge2D> result = Faces.SelectMany(f => f.AllParents).SelectMany(f => f.Edges).Distinct().ToList();
            result.AddRange(AllEdges);
            return result;
        }*/

        private List<Point2D> GetOutlinePoints()
        {
            var edges = Edges.FindAll(edge => edge.Faces.Count == 1).Distinct().ToList();

            //Debug.Log("All Edges 's face is only one : " + edges.Count);

            List<Point2D> result = new List<Point2D>();

            var rightMostEdge = edges.Find(e => e.Points[0] == RightMostPoint);

            var current = rightMostEdge.Points[0];
            var next = rightMostEdge.Points[1];

            result.Add(current);

            while (next != current)
            {

                result.Add(next);

                var edge = edges.Find(e => e.Points[0] == next);
                next = edge.Points[1];

            }

            return result;
        }

        public override string ToString()
        {
            return string.Format("{0} Faces, {1} Edges, {2} Points.", Faces.Count, Edges.Count, Points.Count);
        }

        public Face2D AddPoints(params Point2D[] points)
        {
            //newEdges = new List<Edge2D>();
            int size = points.Length;
            if (size < 3)
                throw new System.Exception("The points count cannot less than 3.");


            //List<Edge2D> allEdges = new List<Edge2D>(AllEdges);

            List<Edge2D> edges = new List<Edge2D>(size);
            

            //Debug.Log("All Edges Count : " + Edges.Count);


            for (int i = 0, j = 1; i < size; i++, j = (i + 1) % size)
            {
                var ss = Edges.Find(e => e.IsMatchFor(points[i], points[j]));
                if (ss != null)
                {
                    //Debug.Log(ss);
                    edges.Add(ss);

                }
                else
                {
                    
                    var edge = new Edge2D(points[i], points[j]);
                    Edges.Add(edge);
                    edges.Add(edge);
                    if (!Points.Contains(points[i])) Points.Add(points[i]);
                    if (!Points.Contains(points[j])) Points.Add(points[j]);
                }
            }
            
            return AddFace(new Face2D(edges.ToArray()));

        }

        public Point2D RightMostPoint
        {
            get
            {
                var result = Points.First();

                foreach (var p in Points)
                {
                    if ((p.Position.x > result.Position.x) || (p.Position.x == result.Position.x && p.Position.y > result.Position.y))
                    {
                        result = p;
                    }

                }

                //Debug.Log("Top Left Point index : " + AllPoints.IndexOf(result));

                return result;
            }
        }





        public void UnitTest()
        {
            /*foreach(var p in AllPoints)
            {
                foreach (var e in p.Edges)
                {
                    Debug.Log(string.Format("{0} to {1} is {2}", p, e.OtherPoint(p), e));
                }
            }*/

            /*foreach(var pt in Outline)
            {
                Debug.Log(pt);
            }

            LogPointEdges(AllPoints[13]);

            LogPointEdges(AllPoints[12]);*/

            //LogOutline();

            foreach (var p in OutlinePoints)
                Debug.Log(p);

        }

        public void LogPointEdges(Point2D point)
        {
            foreach (var e in Edges.Where(edge => edge.Points.Contains(point)))
            {

                var dir = e.OtherPoint(point).Position - point.Position;
                var angle = Vector3.Angle(Vector3.right, dir);
                var dotValue = Vector3.Dot(Vector3.right, dir);
                var Perp = CGAlgorithm.Perp(Vector3.right, dir);
                var tan2 = Mathf.Atan2(dir.y, dir.x);
                if (tan2 < 0) tan2 += 2 * Mathf.PI;
                tan2 *= 180 / Mathf.PI;
                Debug.Log(string.Format("{0} to {1} 'angle is {2}, Perp is {3}, Dot is {4}, Tan2 is {5}",
                    point, e.OtherPoint(point), angle, Perp, dotValue, tan2));
            }
        }

        public void LogOutline()
        {

            var edges = Edges.Where(edge => edge.Faces.Count == 1).ToList();

            List<Point2D> result = new List<Point2D>();

            /*var current = edges[0].Points[0];
            var next = edges[0].Points[1];*/

            var rightMostEdge = edges.Find(e => e.Points[0] == RightMostPoint);
            Debug.Log(rightMostEdge);

            var current = rightMostEdge.Points[0];
            var next = rightMostEdge.Points[1];

            result.Add(current);

            while (next != current)
            {

                result.Add(next);

                var edge = edges.Find(e => e.Points[0] == next);
                next = edge.Points[1];

            }

            foreach (var p in result)
                Debug.Log(p);

        }
    }
}
