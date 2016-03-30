using UnityEngine;
using System.Collections;

namespace Geometry
{

    public static class NewBehaviourScript
    {

        public static void AddMesh(this Shape2D shape, Mesh mesh)
        {
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {

                var p0 = new Point2D(mesh.vertices[mesh.triangles[i]]);//, mesh.name + "-" + mesh.triangles[i]);
                var p1 = new Point2D(mesh.vertices[mesh.triangles[i + 1]]);//, mesh.name + "-" + mesh.triangles[i+1]);
                var p2 = new Point2D(mesh.vertices[mesh.triangles[i + 2]]);//, mesh.name + "-" + mesh.triangles[i+2]);
                
                var face = shape.AddPoints(p0, p1, p2);
            }
        }
    }
}
