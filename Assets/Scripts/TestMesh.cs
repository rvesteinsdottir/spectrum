using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using csDelaunay;
using System.IO;

//[RequireComponent(typeof(MeshFilter))]
public class TestMesh : MonoBehaviour
{
    public int polygonNumber;
    public Color startColor;
    public Color endColor;
    public Vector2 imageDim;
    public Color[] colorArray;
 
    private List<Vector2f> points;
    private List<Vector2> adjustedPoints;
    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;
    private float highX;
    private float highY;
    private float lowX;
    private float lowY;
    private GameObject meshParent;
    public PolygonCollider2D[] allColliders;
    private float lowestSeenX;
    private float lowestSeenY;
    private float highestSeenX;
    private float highestSeenY;


    void Start() {
        if(PlayerPrefs.GetInt("Level") == 1)
            polygonNumber = 6;
        else if (PlayerPrefs.GetInt("Level") == 2)
            polygonNumber = 8;
        else
            polygonNumber = 10;
 
        // Create random points
        imageDim = new Vector2(Screen.width - 80, Screen.width - 80);
        Debug.Log(imageDim);
        startColor = HexToColor(PlayerPrefs.GetString("ColorOne", ColorToHex(Color.red)));
        endColor = HexToColor(PlayerPrefs.GetString("ColorTwo", ColorToHex(Color.blue)));
        points = CreateRandomPoint();
        highX = ((imageDim.x/2)/100f);
        highY = ((imageDim.y/2)/100f);
        lowX = -highX;
        lowY = -highY;
        lowestSeenX = ((imageDim.x/2)/100f);
        lowestSeenY = ((imageDim.y/2)/100f);
        highestSeenX = 0;
        highestSeenY = 0;

        
        meshParent = GameObject.Find("MeshParent");
        colorArray = new Color[polygonNumber];
    
        Rectf bounds = new Rectf(0,0,imageDim.x,imageDim.y);
        Voronoi voronoi = new Voronoi(points,bounds);
 
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;
 
        DisplayDiagram();
        DisplayEdges();
    }


    // Method from csDelunay library creator PouletFrit on May 26, 2014 via Unity Forum (https://forum.unity.com/threads/delaunay-voronoi-diagram-library-for-unity.248962/)
    private List<Vector2f> CreateRandomPoint() {
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < polygonNumber; i++) {
            points.Add(new Vector2f(Random.Range(0,imageDim.x), Random.Range(0,imageDim.y)));
        }
 
        return points;
    }

    private void DisplayDiagram() {
        Color tileWidth = ((endColor - startColor)/imageDim.x);
        int index = 0;
        allColliders = new PolygonCollider2D[polygonNumber];
        adjustedPoints = new List<Vector2>();

        Vector2f[] siteKeys = sites.Keys.ToArray();

        for (int j = 0; j < siteKeys.Length; j++)
        {
            Vector2f entryKey = siteKeys[j];

            Site entryValue = sites[entryKey];

            float sitePosX = ((entryValue.x - imageDim.x/2)/100f);
            float sitePosY = ((entryValue.y - imageDim.y/2)/100f);
            adjustedPoints.Add(new Vector2(sitePosX, sitePosY));
        }

        for (int i = 0; i < siteKeys.Length; i++)
        {
            Vector2f entryKey = siteKeys[i];
            Site entryValue = sites[entryKey];
            
            // Generate mesh vertices from Voronoi region edges
            List<Vector2> allVertices = GenerateVertices(entryValue);

            // Remove duplicates from vertices list
            List<Vector2> uniqueVerticesList = allVertices.Distinct().ToList();

            // Sort list in counter clockwise order for triangulation
            uniqueVerticesList.Sort((v, w) => compare(v, w, adjustedPoints[i]));

            Color tileColor = (startColor + (tileWidth * entryValue.y));
            //Color tileColor = Color.white;
            colorArray[index] = tileColor;
            Vector2[] uniqueVerticesArray = uniqueVerticesList.ToArray();

            allColliders[index] = GenerateGameObject(uniqueVerticesArray, tileColor);
            index ++;
        }
    }

    PolygonCollider2D GenerateGameObject(Vector2[] uniqueVerticesArray, Color tileColor)
    {
        GameObject newGameObject = new GameObject("MeshPolygon");
        newGameObject.transform.SetParent(meshParent.transform);
        newGameObject.transform.localPosition = new Vector3(0f, 0f, -2f);
        newGameObject.layer = LayerMask.NameToLayer("Board");

        GenerateMesh(newGameObject, uniqueVerticesArray);

        // Add texture based on y position of center
        Texture2D texture = new Texture2D(128, 128, TextureFormat.ARGB32, false);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
                texture.SetPixel(x, y, Color.white);
        }
        texture.Apply();
        Material newMaterial = new Material(Shader.Find("Unlit/Texture"));
        newMaterial.mainTexture = texture;
        newGameObject.GetComponent<MeshRenderer>().material = newMaterial; 
        newGameObject.GetComponent<MeshRenderer>().enabled = false;

        // Add polygon collider
        PolygonCollider2D newCollider = newGameObject.AddComponent<PolygonCollider2D>();
        newCollider.points = uniqueVerticesArray;
        newCollider.SetPath(0, uniqueVerticesArray);
        newCollider.isTrigger = true;

        return newCollider;
    }

    private void GenerateMesh(GameObject newGameObject, Vector2[] uniqueVerticesArray)
    {
        Vector3[] uniqueVertices3D = new Vector3[uniqueVerticesArray.Length];

        for (int i = 0; i < uniqueVerticesArray.Length; i++)
        {
            uniqueVertices3D[i] = new Vector3(uniqueVerticesArray[i].x, uniqueVerticesArray[i].y, 1);
        }

        newGameObject.AddComponent<MeshFilter>();
        newGameObject.AddComponent<MeshRenderer>();

        // Use the triangulator to get indices for creating mesh triangles
        Triangulator tr = new Triangulator(uniqueVerticesArray);
        int[] indices = tr.Triangulate();

        // Add mesh to new game object
        Mesh mesh = newGameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = uniqueVertices3D;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    List<Vector2> GenerateVertices(Site newSite)
    {
        
        float sitePosX = ((newSite.x - imageDim.x/2)/100f);
        float sitePosY = ((newSite.y - imageDim.x/2)/100f);
        Vector2 center = new Vector2(sitePosX, sitePosY);
        bool containsLowX = false;
        bool containsLowY = false;
        bool containsHighX = false;
        bool containsHighY = false;

        List<Edge> siteEdges = newSite.Edges;
        List<Vector2> vertices2D = new List<Vector2>();

        // Reduce to an array of all vertices of each edge
        for (int i = 0; i < siteEdges.Count; i++)
        {
            if (siteEdges[i].ClippedEnds == null) continue;
            
            Vector2 leftVertex = new Vector2(siteEdges[i].ClippedEnds[LR.LEFT].x, siteEdges[i].ClippedEnds[LR.LEFT].y);
            Vector2 adjustedLeft = new Vector2(((leftVertex.x-imageDim.x/2)/100f), (leftVertex.y-imageDim.y/2)/100f);

            Vector2 rightVertex = new Vector2(siteEdges[i].ClippedEnds[LR.RIGHT].x, siteEdges[i].ClippedEnds[LR.RIGHT].y);
            Vector2 adjustedRight = new Vector2(((rightVertex.x-imageDim.x/2)/100f), (rightVertex.y-imageDim.y/2)/100f);
            
            vertices2D.Add(new Vector2(adjustedLeft.x, adjustedLeft.y));
            vertices2D.Add(new Vector2(adjustedRight.x, adjustedRight.y));

            if (adjustedLeft.x == lowX || adjustedRight.x == lowX)
                containsLowX = true;
            if (adjustedLeft.y == lowY || adjustedRight.y == lowY)
                containsLowY = true;
            if (adjustedLeft.x == highX || adjustedRight.x == highX)
                containsHighX = true;
            if (adjustedLeft.y == highY || adjustedRight.y == highY)
                containsHighY = true;
        }

        // Add corners to vertices list if needed
        if(containsLowX && containsLowY && isOuterCentroid("lowerLeft", center))
            vertices2D.Add(new Vector2(lowX, lowY));
        else if(containsHighX && containsHighY && isOuterCentroid("upperRight", center))
            vertices2D.Add(new Vector2(highX, highY));
        else if (containsLowX && containsHighY && isOuterCentroid("upperLeft", center)) 
            vertices2D.Add(new Vector2(lowX, highY));
        else if (containsHighX && containsLowY && isOuterCentroid("lowerRight", center)) 
            vertices2D.Add(new Vector2(highX, lowY));
        else if (containsHighY && containsLowY)
        {
            // cannot be ANY vertex, must be the lowest or highest
            //if (vertices2D[0].x > 0 && isOuterCentroid("upperRight", center) && isOuterCentroid("lowerRight", center))
            if (isOuterCentroid("upperRight", center) && isOuterCentroid("lowerRight", center))
            {
                vertices2D.Add(new Vector2(highX, highY));
                vertices2D.Add(new Vector2(highX, lowY));
            }
            else if (isOuterCentroid("lowerLeft", center) && isOuterCentroid("upperLeft", center))
            {
                vertices2D.Add(new Vector2(lowX, highY));
                vertices2D.Add(new Vector2(lowX, lowY));
            }
        }
        else if (containsHighX && containsLowX)
        {
            // cannot be ANY vertex, must be the lowest or highest
            if (isOuterCentroid("upperRight", center) && isOuterCentroid("upperLeft", center))
            {
                vertices2D.Add(new Vector2(lowX, highY));
                vertices2D.Add(new Vector2(highX, highY));
            }
            else if (isOuterCentroid("lowerRight", center) && isOuterCentroid("lowerLeft", center))
            {
                vertices2D.Add(new Vector2(lowX, lowY));
                vertices2D.Add(new Vector2(highX, lowY));
            }
        }

        return vertices2D;
    }

    bool isOuterCentroid(string place, Vector2 currentCentroid)
    {
        bool currentIsOutermost = true; 
        if (place == "lowerLeft")
        {
            for (int i = 0; i < adjustedPoints.Count; i++)
            {
                if ((adjustedPoints[i].x < currentCentroid.x) && (adjustedPoints[i].y < currentCentroid.y))
                    currentIsOutermost = false;
            }
        }
        else if (place == "upperRight")
        {
            for (int i = 0; i < adjustedPoints.Count; i++)
            {
                if((adjustedPoints[i].x > currentCentroid.x) && (adjustedPoints[i].y > currentCentroid.y))
                    currentIsOutermost = false;
            }
        }
        else if (place == "lowerRight")
        {

            for (int i = 0; i < adjustedPoints.Count; i++)
            {
                if((adjustedPoints[i].x > currentCentroid.x) && (adjustedPoints[i].y < currentCentroid.y))
                    currentIsOutermost = false;
            }
        }
        else if (place == "upperLeft")
        {

            for (int i = 0; i < adjustedPoints.Count; i++)
            {
                if((adjustedPoints[i].x < currentCentroid.x) && (adjustedPoints[i].y > currentCentroid.y))
                    currentIsOutermost = false;
            }
        }

        return currentIsOutermost;
    }


    // Method adapted from Stack OverFlow answer from ciamej on August 8, 2011 (https://stackoverflow.com/questions/6989100/sort-points-in-clockwise-order)
    int compare (Vector2 a, Vector2 b, Vector2 center)
    {
        if (a.x - center.x >= 0 && b.x - center.x < 0)
            return 1;
        if (a.x - center.x < 0 && b.x - center.x >= 0)
            return -1;
        if (a.x - center.x == 0 && b.x - center.x == 0) {
            if (a.y - center.y >= 0 || b.y - center.y >= 0)
                return a.y > b.y ? 1 : -1;
            return b.y > a.y ? 1 : -1;
        }

        // compute the cross product of vectors (center -> a) x (center -> b)
        float det = (a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y);
        if (det < 0)
            return 1;
        if (det > 0)
            return -1;

        // points a and b are on the same line from the center
        // check which point is closer to the center
        float d1 = (a.x - center.x) * (a.x - center.x) + (a.y - center.y) * (a.y - center.y);
        float d2 = (b.x - center.x) * (b.x - center.x) + (b.y - center.y) * (b.y - center.y);
        return d1 > d2 ? 1 : -1;
    }

    private void DisplayEdges() {
        
        Texture2D tx = new Texture2D((int)imageDim.x, (int)imageDim.y, TextureFormat.ARGB32, false);
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(tx, new Rect(0,0, imageDim.x, imageDim.y), (Vector2.one * 0.5f));
        transform.localPosition = new Vector3(0, -2.2f, -2f);
        int borderOffset = 5;
        Color borderColor = new Color (179/255f, 189/255f, 201/255f);

        for (int i = 0; i < edges.Count; i++)
        {
            Edge edge = edges[i];

            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;

            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, borderColor);

            // Set remaining sections of texture to white
            for (int row = 0; row < imageDim.x; row++)
            {
                for (int column = 0; column < imageDim.y; column++)
                {
                    if (tx.GetPixel(row, column) != borderColor)
                    {
                        if (row <= borderOffset || column <= borderOffset || row >= (imageDim.x - borderOffset - 1) || column >= imageDim.y - borderOffset - 1)
                        {
                            Color pixelColor = GetClosestCentroid(new Vector2(row, column));
                            tx.SetPixel(row, column, pixelColor);
                        }
                        else
                        {
                            tx.SetPixel(row, column, new Color (1,1,1,0));
                        }
                    }
                }
            }
        }

        tx.filterMode = FilterMode.Point;
        tx.Apply();
        Material newMaterial = new Material(Shader.Find("Unlit/Texture"));
        newMaterial.mainTexture = tx;

        this.GetComponent<Renderer>().material.mainTexture = tx;
    }

    Color GetClosestCentroid(Vector2 pixelPos)
    {
        float smallestDist = float.MaxValue;
        int index = 0;
        Vector2 adjustedPos = new Vector2(((pixelPos.x - imageDim.x/2)/100f), ((pixelPos.y - imageDim.y/2)/100f));

        for (int i = 0; i < adjustedPoints.Count; i++)
        {
            if (Vector2.Distance(adjustedPos, adjustedPoints[i]) < smallestDist)
            {
            smallestDist = Vector2.Distance(adjustedPos, adjustedPoints[i]);
            index = i;
            }
        }
        return colorArray[index];
    }
 
    // Bresenham line algorithm
    private void DrawLine(Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0) {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;
       
        int dx = Mathf.Abs(x1-x0);
        int dy = Mathf.Abs(y1-y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx-dy;
       
        while (true) {
            tx.SetPixel(x0+offset,y0+offset,c);
            tx.SetPixel(x0+offset+1,y0+offset, c);
            tx.SetPixel(x0+offset-1,y0+offset, c);
           
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2*err;
            if (e2 > -dy) {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx) {
                err += dx;
                y0 += sy;
            }
        }
    }


    // CREDIT
    string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        Debug.Log(hex);
        return hex;
    }
    
    Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        return new Color(r/255f, g/255f, b/255f , 1);
    }
}
