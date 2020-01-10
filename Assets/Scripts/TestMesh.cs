using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using csDelaunay;

[RequireComponent(typeof(MeshFilter))]
public class TestMesh : MonoBehaviour
{
    public int polygonNumber = 40;
 
    // This is where we will store the resulting data
    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;
    private List<Vector2f> points;

    void Start() {
        // Create your sites (lets call that the center of your polygons)
        points = CreateRandomPoint();
    
        Rectf bounds = new Rectf(0,0,512,512);
       
        Voronoi voronoi = new Voronoi(points,bounds,5);
 
        
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;
        //triangles = voronoi.Triangles;
 
        DisplayVoronoiDiagram();
    }

     private List<Vector2f> CreateRandomPoint() {
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < polygonNumber; i++) {
            points.Add(new Vector2f(Random.Range(0,512), Random.Range(0,512)));
        }
 
        return points;
    }

    private void DisplayVoronoiDiagram() {
        foreach(KeyValuePair<Vector2f, Site> entry in sites)
        {
            Site newSite = entry.Value;
            List<Edge> siteEdges = newSite.Edges;
            Vector2[] vertices2D = new Vector2[siteEdges.Count];
            for (int i = 0; i < siteEdges.Count; i++)
            {
                if (siteEdges[i].ClippedEnds == null) continue;

                vertices2D[i] = new Vector2(siteEdges[i].ClippedEnds[LR.LEFT].x, siteEdges[i].ClippedEnds[LR.LEFT].y);
                Debug.Log(vertices2D[i]);


                // Use the triangulator to get indices for creating triangles
                Triangulator tr = new Triangulator(vertices2D);
                int[] indices = tr.Triangulate();

                // Create the Vector3 vertices
                Vector3[] vertices = new Vector3[vertices2D.Length];
                for (int j=0; j<vertices.Length; j++) {
                    vertices[j] = new Vector3(vertices2D[j].x, vertices2D[j].y, 0);
                }

                // Create the mesh
                Mesh msh = new Mesh();
                msh = GetComponent<MeshFilter> ().mesh;  
                msh.vertices = vertices;
                msh.triangles = indices;
                msh.RecalculateNormals();
                msh.RecalculateBounds();

                // Set up game object with mesh;
                gameObject.AddComponent(typeof(MeshRenderer));
                // MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
                // filter.mesh = msh;
            }
        }
        // List<Edge> meshEdges = sites[points[0]].Edges;

        // Debug.Log(meshEdges[0].ClippedEnds[LR.LEFT]);

 
    }

    // void Start () {
    //     // Create Vector2 vertices
    //     Vector2[] vertices2D = new Vector2[] {
    //         new Vector2(0,0),
    //         new Vector2(0,50),
    //         new Vector2(50,50),
    //         new Vector2(50,100),
    //         new Vector2(0,100),
    //         new Vector2(0,150),
    //         new Vector2(150,150),
    //         new Vector2(150,100),
    //         new Vector2(100,100),
    //         new Vector2(100,50),
    //         new Vector2(150,50),
    //         new Vector2(150,0),
    //     };
 
    //     // Use the triangulator to get indices for creating triangles
    //     Triangulator tr = new Triangulator(vertices2D);
    //     int[] indices = tr.Triangulate();
 
    //     // Create the Vector3 vertices
    //     Vector3[] vertices = new Vector3[vertices2D.Length];
    //     for (int i=0; i<vertices.Length; i++) {
    //         vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
    //     }
 
    //     // Create the mesh
    //     Mesh msh = new Mesh();
    //     msh = GetComponent<MeshFilter> ().mesh;  
    //     msh.vertices = vertices;
    //     msh.triangles = indices;
    //     msh.RecalculateNormals();
    //     msh.RecalculateBounds();
 
    //     // Set up game object with mesh;
    //     gameObject.AddComponent(typeof(MeshRenderer));
    //     // MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
    //     // filter.mesh = msh;
    // }
}
