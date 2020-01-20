using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using csDelaunay;
using System.IO;

public class MeshVorDiagram : MonoBehaviour
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
    private GameObject voronoiDiagram;
    public PolygonCollider2D[] allColliders;


    void Start() {
        if (PlayerPrefs.GetInt("Level") == 1)
            polygonNumber = 6;
        else if (PlayerPrefs.GetInt("Level") == 2)
            polygonNumber = 8;
        else
            polygonNumber = 10;

        GenerateVariables();
        DisplayDiagram();
        DisplayEdges();
    }


    private void GenerateVariables()
    {
        // Generate display from platform dimensions
        imageDim = new Vector2(Screen.width - 75, Screen.width - 75);
        highX = ((imageDim.x/2)/100f);
        highY = ((imageDim.y/2)/100f);
        lowX = -highX;
        lowY = -highY;

        // Fetch color preferences
        startColor = HexToColor(PlayerPrefs.GetString("ColorOne", ColorToHex(Color.red)));
        endColor = HexToColor(PlayerPrefs.GetString("ColorTwo", ColorToHex(Color.blue)));
        colorArray = new Color[polygonNumber];

        // Create random set of points for diagram
        points = CreateRandomPoint();
        Rectf bounds = new Rectf(0,0,imageDim.x,imageDim.y);
        Voronoi voronoi = new Voronoi(points,bounds);
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;
    }

    // CreateRandomPoints method from csDelunay library creator PouletFrit on May 26, 2014 via Unity Forum (https://forum.unity.com/threads/delaunay-voronoi-diagram-library-for-unity.248962/)
    private List<Vector2f> CreateRandomPoint() {
        List<Vector2f> points = new List<Vector2f>();

        for (int pointCount = 0; pointCount < polygonNumber; pointCount++) {
            points.Add(new Vector2f(Random.Range(0,imageDim.x), Random.Range(0,imageDim.y)));
        }
 
        return points;
    }

    private void DisplayDiagram() {
        voronoiDiagram = GameObject.Find("VoronoiDiagram");
        Color tileWidth = ((endColor - startColor)/imageDim.x);
        allColliders = new PolygonCollider2D[polygonNumber];

        Vector2f[] siteKeys = sites.Keys.ToArray();
        GenerateAdjustedPoints();

        for (int siteKeyIndex = 0; siteKeyIndex < siteKeys.Length; siteKeyIndex++)
        {
            Vector2f entryKey = siteKeys[siteKeyIndex];
            Site entryValue = sites[entryKey];
            
            // Generate mesh vertices from Voronoi region edges
            List<Vector2> allVertices = GenerateVertices(entryValue);

            // Remove duplicates from vertices list
            List<Vector2> uniqueVerticesList = allVertices.Distinct().ToList();

            // Sort list in counter clockwise order for triangulation
            uniqueVerticesList.Sort((a, b) => compare(a, b, adjustedPoints[siteKeyIndex]));
            Vector2[] uniqueVerticesArray = uniqueVerticesList.ToArray();

            // Generate tile color based on points relation to y axis
            Color tileColor = (startColor + (tileWidth * entryValue.y));
            colorArray[siteKeyIndex] = tileColor;
            
            allColliders[siteKeyIndex] = GenerateGameObject(uniqueVerticesArray, tileColor);
        }
    }

    private void GenerateAdjustedPoints()
    {
        adjustedPoints = new List<Vector2>();
        Vector2f[] siteKeys = sites.Keys.ToArray();

        for (int siteKeyIndex = 0; siteKeyIndex < siteKeys.Length; siteKeyIndex++)
        {
            Vector2f entryKey = siteKeys[siteKeyIndex];
            Site entryValue = sites[entryKey];

            // Change location of point from pixel to vector
            float sitePosX = AdjustedPosition("x", entryValue.x);
            float sitePosY = AdjustedPosition("y", entryValue.y);

            adjustedPoints.Add(new Vector2(sitePosX, sitePosY));
        }
    }

    PolygonCollider2D GenerateGameObject(Vector2[] uniqueVerticesArray, Color tileColor)
    {
        GameObject polygonGO = new GameObject("MeshPolygon");
        polygonGO.transform.SetParent(voronoiDiagram.transform);
        polygonGO.transform.localPosition = new Vector3(0f, 0f, -2f);
        polygonGO.layer = LayerMask.NameToLayer("Board");

        AddMesh(polygonGO, uniqueVerticesArray);

        // Create texture for game object
        Texture2D texture = new Texture2D(128, 128, TextureFormat.ARGB32, false);
        for (int row = 0; row < texture.height; row++)
        {
            for (int col = 0; col < texture.width; col++)
                texture.SetPixel(row, col, tileColor);
        }
        texture.Apply();

        Material newMaterial = new Material(Shader.Find("Unlit/Texture"));
        newMaterial.mainTexture = texture;
        polygonGO.GetComponent<MeshRenderer>().material = newMaterial; 

        // Disable game object until tile is correctly placed
        polygonGO.GetComponent<MeshRenderer>().enabled = false;

        // Add polygon collider
        PolygonCollider2D newCollider = polygonGO.AddComponent<PolygonCollider2D>();
        newCollider.points = uniqueVerticesArray;
        newCollider.SetPath(0, uniqueVerticesArray);
        newCollider.isTrigger = true;

        return newCollider;
    }

    private void AddMesh(GameObject newPolygonGO, Vector2[] uniqueVerticesArray)
    {
        Vector3[] uniqueVertices3D = new Vector3[uniqueVerticesArray.Length];

        for (int index = 0; index < uniqueVerticesArray.Length; index++)
        {
            uniqueVertices3D[index] = new Vector3(uniqueVerticesArray[index].x, uniqueVerticesArray[index].y, 1);
        }

        newPolygonGO.AddComponent<MeshFilter>();
        newPolygonGO.AddComponent<MeshRenderer>();

        // Use triangulator to get indices for creating mesh triangles
        Triangulator tr = new Triangulator(uniqueVerticesArray);
        int[] indices = tr.Triangulate();

        // Add mesh to new game object
        Mesh mesh = newPolygonGO.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = uniqueVertices3D;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    List<Vector2> GenerateVertices(Site newSite)
    {
        Vector2 center = new Vector2(AdjustedPosition("x", newSite.x), AdjustedPosition("y", newSite.y));
        List<Edge> siteEdges = newSite.Edges;
        bool containsLowX = false;
        bool containsLowY = false;
        bool containsHighX = false;
        bool containsHighY = false;

        List<Vector2> vertices2D = GenerateVerticesList(siteEdges);

        // Check to see if any corners need to be added to polygon
        for (int vertexIndex = 0; vertexIndex < vertices2D.Count; vertexIndex++)
        {
            if (vertices2D[vertexIndex].x == lowX)
                containsLowX = true;
            if (vertices2D[vertexIndex].y == lowX)
                containsLowY = true;
            if (vertices2D[vertexIndex].x == highX)
                containsHighX = true;
            if (vertices2D[vertexIndex].y == highY)
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

    List <Vector2> GenerateVerticesList(List<Edge> siteEdges)
    {
        List<Vector2> vertices = new List<Vector2>();

        for (int edgeIndex = 0; edgeIndex < siteEdges.Count; edgeIndex++)
        {
            if (siteEdges[edgeIndex].ClippedEnds == null) continue;
            
            Vector2 leftVertex = new Vector2(siteEdges[edgeIndex].ClippedEnds[LR.LEFT].x, siteEdges[edgeIndex].ClippedEnds[LR.LEFT].y);
            Vector2 adjustedLeft = new Vector2(AdjustedPosition("x", leftVertex.x), AdjustedPosition("y", leftVertex.y));

            Vector2 rightVertex = new Vector2(siteEdges[edgeIndex].ClippedEnds[LR.RIGHT].x, siteEdges[edgeIndex].ClippedEnds[LR.RIGHT].y);
            Vector2 adjustedRight = new Vector2(AdjustedPosition("x", rightVertex.x), AdjustedPosition("y", rightVertex.y));
            
            vertices.Add(new Vector2(adjustedLeft.x, adjustedLeft.y));
            vertices.Add(new Vector2(adjustedRight.x, adjustedRight.y));
        }

        return vertices;
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

    // Method from Stack Overflow answer from ciamej on August 8, 2011 (https://stackoverflow.com/questions/6989100/sort-points-in-clockwise-order)
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
        float det = (a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y);
        if (det < 0)
            return 1;
        if (det > 0)
            return -1;

        float d1 = (a.x - center.x) * (a.x - center.x) + (a.y - center.y) * (a.y - center.y);
        float d2 = (b.x - center.x) * (b.x - center.x) + (b.y - center.y) * (b.y - center.y);
        return d1 > d2 ? 1 : -1;
    }

    private void DisplayEdges() {
        Texture2D diagramTexture = new Texture2D((int)imageDim.x, (int)imageDim.y, TextureFormat.ARGB32, false);
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(diagramTexture, new Rect(0, 0, imageDim.x, imageDim.y), (Vector2.one * 0.5f));
        transform.localPosition = new Vector3(0, -2.2f, -2f);
        int borderOffset = 5;
        Color borderColor = new Color (179/255f, 189/255f, 201/255f);

        for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
        {
            Edge edge = edges[edgeIndex];

            if (edge.ClippedEnds == null) continue;
            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], diagramTexture, borderColor);

            // Set remaining sections of texture to transparent
            for (int row = 0; row < imageDim.x; row++)
            {
                for (int column = 0; column < imageDim.y; column++)
                {
                    if (diagramTexture.GetPixel(row, column) != borderColor)
                    {
                        if (row <= borderOffset || column <= borderOffset || row >= (imageDim.x - borderOffset - 1) || column >= imageDim.y - borderOffset - 1)
                            diagramTexture.SetPixel(row, column, GetClosestCentroid(new Vector2(row, column)));
                        else
                            diagramTexture.SetPixel(row, column, new Color (1,1,1,0));
                    }
                }
            }
        }

        diagramTexture.filterMode = FilterMode.Point;
        diagramTexture.Apply();
        Material newMaterial = new Material(Shader.Find("Unlit/Texture"));
        newMaterial.mainTexture = diagramTexture;

        this.GetComponent<Renderer>().material.mainTexture = diagramTexture;
    }

    Color GetClosestCentroid(Vector2 pixelPos)
    {
        float smallestDist = float.MaxValue;
        int index = 0;
        Vector2 adjustedPos = new Vector2( AdjustedPosition("x", pixelPos.x), AdjustedPosition("y", pixelPos.y));

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

    float AdjustedPosition(string axis, float currentValue)
    {
        if (axis == "x")
            return ((currentValue - imageDim.x/2)/100f);
        else
            return ((currentValue - imageDim.y/2)/100f);
    }
 
    // DrawLine method (Bresenham line algorithm) from csDelunay library creator PouletFrit on May 26, 2014 via Unity Forum (https://forum.unity.com/threads/delaunay-voronoi-diagram-library-for-unity.248962/)
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

    // ColorToHex and HexToColor methods adapted from method published by Grish_tad on Unity Forum on December 29, 2017 (https://answers.unity.com/questions/1447929/how-to-save-color.html)
    string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
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
