namespace Geometry
{

    using UnityEngine;
    using System.Collections.Generic;
    using System;
    using System.Linq;

    public class Shape2D
    {
        private readonly _Lazy<List<Edge2D>> _lazyAllEdges;
        private readonly _Lazy<List<Point2D>> _lazyAllPoints;
        private readonly _Lazy<List<Edge2D>> _lazyExitedEdges;
        
        public Shape2D()
        {
            Faces = new List<Face2D>();

            _lazyAllEdges = new _Lazy<List<Edge2D>>(GetAllEdges);
            _lazyAllPoints = new _Lazy<List<Point2D>>(GetAllPoints);
            _lazyExitedEdges = new _Lazy<List<Edge2D>>(GetAllExistedEdges);
        }

        public List<Face2D> Faces { get; set; }
        public List<Edge2D> AllEdges { get { return _lazyAllEdges.Value; } }
        public List<Point2D> AllPoints { get { return _lazyAllPoints.Value; } }
        public List<Edge2D> AllExistedEdges { get { return _lazyExitedEdges.Value; } }
        
        public Face2D AddFace(Face2D face)
        {
            if (Faces.Any(f => f.IsMatchFor(face)))
            {
                throw new InvalidOperationException("There is already such a face in the shape!");
            }
            Faces.Add(face);
            face.Shape = this;
            _lazyExitedEdges.isValeChanged = true;
            _lazyAllEdges.isValeChanged = true;
            _lazyAllPoints.isValeChanged = true;
            return face;
        }
        
        public void RemoveFace(Face2D face)
        {
            Faces.Remove(face);

            _lazyExitedEdges.isValeChanged = true;
            _lazyAllEdges.isValeChanged = true;
            _lazyAllPoints.isValeChanged = true;
            //face.BasicEdges.ForEach(e => e.Faces.Remove(face));
            /*_lazyAllEdges.isValeChanged = true;
             _lazyAllPoints.isValeChanged = true;*/
        }

        public void Clear()
        {
            Faces.Clear();

            _lazyExitedEdges.isValeChanged = true;
            _lazyAllEdges.isValeChanged = true;
            _lazyAllPoints.isValeChanged = true;
        }

        private List<Edge2D> GetAllEdges()
        {
            return Faces.SelectMany(f => f.BasicEdges).Distinct().ToList();
        }

        private List<Point2D> GetAllPoints()
        {
            return Faces.SelectMany(f => f.ActualPoints).Distinct().ToList();
        }

        private List<Edge2D> GetAllExistedEdges()
        {
            List<Edge2D> result = Faces.SelectMany(f => f.AllParents).SelectMany(f => f.BasicEdges).Distinct().ToList();
            result.AddRange(AllEdges);
            return result;
        }

        public override string ToString()
        {
            return string.Format("Faces : {0}, Edges : {1}, Points : {2}", Faces.Count, GetAllEdges().Count, GetAllPoints().Count);
        }

        public Face2D AddPoints(params Point2D[] points)
        {
            //newEdges = new List<Edge2D>();
            int size = points.Length;
            if (size < 3)
                throw new System.Exception("The points count cannot less than 3.");
            

            List<Edge2D> edges = new List<Edge2D>();

            for (int i = 0, j = 1; i < size; i++, j = (i + 1) % size)
            {
                edges.Add(new Edge2D(points[i], points[j]));
            }

            bool[] isEdgeExits = new bool[size];

            List<Edge2D> allEdges = AllExistedEdges;

            foreach (var ss in allEdges)// allEdges)
            {
                for (int i = 0; i < size; i++)
                {
                    if (edges[i].IsMatchFor(ss))
                    {
                        Edge2D match = edges[i];
                        foreach(var p in match.Points)
                        {
                            p.Edges.Remove(match);
                        }

                        edges[i] = ss;
                        isEdgeExits[i] = true;

                    }
                }
            }

            for (int i = 0; i < size; i++)
            {
                if (!isEdgeExits[i])
                {
                    allEdges.Add(edges[i]);
                }
            }

            

            /*Face2D result = new Face2D(edges.ToArray());*/
            /*AddFace(new Face2D(edges.ToArray()));*/

            

            /*Debug.Log("Local variable allEdges count :" + allEdges.Count);
            Debug.Log("world variable GetAllEdges count :" + GetAllEdges().Count);*/

            return AddFace(new Face2D(edges.ToArray()));

        }

        public void OnDivide(Vector3 position)
        {
            foreach (var f in Faces)
            {
                if (f.IsOver(position))
                {
                    f.OnDivide();
                    //f.OnUnite();
                    break;
                }
            }
        }

        public void OnUnite(Vector3 position)
        {
            foreach (var f in Faces)
            {
                if (f.IsOver(position))
                {
                    f.OnUnite();
                    //f.OnUnite();
                    break;
                }
            }
        }

        public Point2D TopRightPoint
        {
            get
            {
                var result = AllPoints.First();

                foreach(var p in AllPoints)
                {
                    if((p.Position.x > result.Position.x) || (p.Position.x == result.Position.x && p.Position.y > result.Position.y))
                    {
                        result = p;
                    }

                }

                //Debug.Log("Top Left Point index : " + AllPoints.IndexOf(result));
                
                return result;
            }
        }

        public List<Point2D> Outline
        {
            get
            {
                
                
                List<Point2D> result = new List<Point2D>();
                
                var current = TopRightPoint;
                Point2D next = null;
                Point2D Pre = null;

                int i = 0;
                while( i < 10)
                {
                   
                    
                    result.Add(current);

                    next = Next(current, Pre);
                    Pre = current;
                    current = next;
                    i++;
                }
                return result;
            }
        }

        public Point2D Next(Point2D current, Point2D pre = null)
        {
            float minAngle = float.MaxValue;
            Point2D result = current;

            foreach (var e in AllEdges.Where(edge => edge.Points.Contains(current)).Where(edge => !edge.IsMatchFor(current, pre)))
            {
                /*if (pre != null && e.Points.Contains(pre))
                    continue;*/

                var dir = e.OtherPoint(current).Position - current.Position;
                
                /*ar sign = Math.Sign(CGAlgorithm.Perp(dir, Vector2.right));
                var angle = Vector3.Angle( dir, Vector3.right);*/

                var tan2 = Mathf.Atan2(dir.x, dir.y);
                if (tan2 < 0) tan2 += 2 * Mathf.PI;
                //tan2 *= 180 / Mathf.PI;

                if (tan2 < minAngle)
                {
                    minAngle = tan2;
                    result = e.OtherPoint(current);
                }

            }
            return result;
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

            LogOutline();

        }

        public void LogPointEdges(Point2D point)
        {
            foreach (var e in AllEdges.Where(edge => edge.Points.Contains(point)))
            {

                var dir = e.OtherPoint(point).Position - point.Position;
                var angle = Vector3.Angle(Vector3.right, dir);
                var dotValue = Vector3.Dot(Vector3.right, dir);
                var Perp = CGAlgorithm.Perp(Vector3.right, dir);
                var tan2 = Mathf.Atan2( dir.y, dir.x);
                if (tan2 < 0) tan2 += 2 * Mathf.PI;
                tan2 *= 180 / Mathf.PI;
                Debug.Log(string.Format("{0} to {1} 'angle is {2}, Perp is {3}, Dot is {4}, Tan2 is {5}",
                    point, e.OtherPoint(point), angle, Perp, dotValue, tan2));
            }
        }

        public void LogOutline()
        {

            var edges = AllEdges.Where(edge => edge.Faces.Count == 1).ToList();
            
            List<Point2D> result = new List<Point2D>();

            /*var current = edges[0].Points[0];
            var next = edges[0].Points[1];*/

            var rightMostEdge = edges.Find(e => e.Points[0] == TopRightPoint);
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
