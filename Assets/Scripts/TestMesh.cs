using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using csDelaunay;

[RequireComponent(typeof(MeshFilter))]
public class TestMesh : MonoBehaviour
{
    private int polygonNumber = 10;
    private Color startColor = Color.red;
    private Color endColor = Color.blue;
 
    // This is where we will store the resulting data
    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;
    private List<Vector2f> points;
    private Vector2 imageDim; 

    void Start() {
        // Create your sites (lets call that the center of your polygons)
        points = CreateRandomPoint();
        imageDim = new Vector2(512, 512);
    
        Rectf bounds = new Rectf(0,0,imageDim.x,imageDim.y);
        Voronoi voronoi = new Voronoi(points,bounds);
 
        
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
        Debug.Log(sites.Count);
        Color tileWidth = ((endColor - startColor)/polygonNumber);
        int index = 0;
       
        float highX = ((imageDim.x/2)/100f);
        float highY = ((imageDim.y/2)/100f);
        float lowX = -highX;
        float lowY = -highY;
        

        foreach(KeyValuePair<Vector2f, Site> entry in sites)
        {
            Site newSite = entry.Value;
            float sitePosX = ((newSite.x - imageDim.x/2)/100f);
            float sitePosY = ((newSite.y - imageDim.x/2)/100f);
            Vector2 center = new Vector2(sitePosX, sitePosY);
            bool containsLowX = false;
            bool containsLowY = false;
            bool containsHighX = false;
            bool containsHighY = false;

            List<Edge> siteEdges = newSite.Edges;
            List<Vector2> vertices2D = new List<Vector2>();
            //Vector3[] meshVertices = new Vector3[siteEdges.Count];
            Debug.Log($"site {center}");

            // reduce to an array of all vertices of each edge
            for (int i = 0; i < siteEdges.Count; i++)
            {
                if (siteEdges[i].ClippedEnds == null) continue;

                float posX = siteEdges[i].ClippedEnds[LR.LEFT].x; 
                float posY = siteEdges[i].ClippedEnds[LR.LEFT].y;

                Vector2 adjustedLeft = new Vector2(((posX-imageDim.x/2)/100f), (posY-imageDim.y/2)/100f);
                vertices2D.Add(new Vector2(adjustedLeft.x, adjustedLeft.y));

                float nextPosX = siteEdges[i].ClippedEnds[LR.RIGHT].x; 
                float nextPosY = siteEdges[i].ClippedEnds[LR.RIGHT].y;

                Vector2 adjustedRight = new Vector2(((nextPosX-imageDim.x/2)/100f), (nextPosY-imageDim.y/2)/100f);
                vertices2D.Add(new Vector2(adjustedRight.x, adjustedRight.y));

                if (adjustedLeft.x == lowX || adjustedRight.x == lowX)
                {
                    containsLowX = true;
                }

                if (adjustedLeft.y == lowY || adjustedRight.y == lowY)
                {
                    containsLowY = true;
                }
                if (adjustedLeft.x == highX || adjustedRight.x == highX)
                {
                    containsHighX = true;
                }
                if (adjustedLeft.y == highY || adjustedRight.y == highY)
                {
                    containsHighY = true;
                }
            }

            // Add corners to vertices list if needed
            if (containsLowX && containsLowY) 
            {
                vertices2D.Add(new Vector2(lowX, lowY));
            }
            else if (containsHighX && containsHighY) 
            {
                vertices2D.Add(new Vector2(highX, highY));
            }
            else if (containsLowX && containsHighY) 
            {
                vertices2D.Add(new Vector2(lowX, highY));
            }
            else if (containsHighX && containsLowY) 
            {
                vertices2D.Add(new Vector2(highX, lowY));
            }

            // Remove duplicates from vertices list
            List<Vector2> uniqueVertices = vertices2D.Distinct().ToList();
            Debug.Log(vertices2D.Count);
            Debug.Log(uniqueVertices.Count);

            // Sort list in counter clockwise order
            uniqueVertices.Sort((v, w) => compare(v, w, center));
            Vector2[] uniqueVerticesArray = uniqueVertices.ToArray();
            Vector3[] vertices3D = new Vector3[uniqueVerticesArray.Length];


            for (int q = 0; q < uniqueVerticesArray.Length; q++)
            {
                vertices3D[q] = new Vector3(uniqueVerticesArray[q].x, uniqueVerticesArray[q].y, 0);
                Debug.Log(vertices3D[q]);
            }

            // Use the triangulator to get indices for creating triangles
            Triangulator tr = new Triangulator(uniqueVerticesArray);
            int[] indices = tr.Triangulate();

            GameObject go = new GameObject("Empty");
            go.transform.position = new Vector3(0, -2, 2);

            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();

            Mesh mesh = go.GetComponent<MeshFilter>().mesh;

            mesh.Clear();
            mesh.vertices = vertices3D;
            mesh.triangles = indices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            //mesh.uv = uniqueVerticesArray;

            Texture2D texture = new Texture2D(128, 128);

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color color = (startColor + (tileWidth * index));
                    texture.SetPixel(x, y, color);
                }
            }
            
            texture.Apply();

            Material mat = new Material(Shader.Find("Unlit/Texture"));
            mat.mainTexture = texture;

            go.GetComponent<MeshRenderer>().material = mat; 


            PolygonCollider2D newCollider = go.AddComponent<PolygonCollider2D>();
            newCollider.points = uniqueVerticesArray;
            newCollider.SetPath(0, uniqueVerticesArray);
            newCollider.isTrigger = true;

            index += 1;
        }
    }

    // Method adapted from Stack OverFlow answer from ciamej on August 8, 2011 (https://stackoverflow.com/questions/6989100/sort-points-in-clockwise-order)
    int compare (Vector2 a, Vector2 b, Vector2 center)
    {
        if (a.x - center.x >= 0 && b.x - center.x < 0)
            //return true;
            return 1;
        if (a.x - center.x < 0 && b.x - center.x >= 0)
            //return false;
            return -1;
        if (a.x - center.x == 0 && b.x - center.x == 0) {
            if (a.y - center.y >= 0 || b.y - center.y >= 0)
                //return a.y > b.y;
                return a.y > b.y ? 1 : -1;
            //return b.y > a.y;
            return b.y > a.y ? 1 : -1;
        }

        // compute the cross product of vectors (center -> a) x (center -> b)
        float det = (a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y);
        if (det < 0)
            //return true;
            return 1;
        if (det > 0)
            //return false;
            return -1;

        // points a and b are on the same line from the center
        // check which point is closer to the center
        float d1 = (a.x - center.x) * (a.x - center.x) + (a.y - center.y) * (a.y - center.y);
        float d2 = (b.x - center.x) * (b.x - center.x) + (b.y - center.y) * (b.y - center.y);
        //return d1 > d2;
        return d1 > d2 ? 1 : -1;
    }

}
