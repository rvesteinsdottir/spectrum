using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

// Code adapted from Voronoi diagram tutorial in Unity3D(C#) by Upgames published April 21, 2019 (https://www.youtube.com/watch?v=EDv69onIETk)

public class VoronoiDiagram : MonoBehaviour
{
  public Vector2Int imageDimensions;
  public int regionAmount;
  public Color startColor = new Color(255f/255f, 0/255f, 0/255f);
  public Color endColor = new Color(0/255f, 0/255f, 255f/255f);
  public Color[] pixelColors;
  public Color[] regions;


  private void Start()
  {
    
    GetComponent<SpriteRenderer>().sprite = Sprite.Create(GetDiagram(), new Rect(0,0, imageDimensions.x, imageDimensions.y), Vector2.one * 0.5f);
    transform.position = new Vector3(0, -2, 0);

    var collider = GetComponent<PolygonCollider2D>();
  }

 
  
  Texture2D GetDiagram()
  {
    Vector2Int[] centroids = new Vector2Int[regionAmount];
    regions = new Color[regionAmount];
    Color tileWidth = ((endColor - startColor)/imageDimensions.x);
    GameObject newSprite = GameObject.Find("VoronoiDiagram");
    List<Vector2>[] colliders = new List<Vector2>[regionAmount];
    
    for (int i = 0; i < regionAmount; i++)
    {
      centroids[i] = new Vector2Int(Random.Range(0, imageDimensions.x), Random.Range(0, imageDimensions.y));

      colliders[i] = new List<Vector2>(){ centroids[i] };

      regions[i] = startColor + (tileWidth * centroids[i].x);
    }
      
    pixelColors = new Color[imageDimensions.x * imageDimensions.y];

    for (int x = 0; x < imageDimensions.x; x++)
    {
      for (int y = 0; y < imageDimensions.y; y++)
      {
        int index = x * imageDimensions.x + y;
        pixelColors[index] = regions[GetClosestCentroidIndex(new Vector2Int(x, y), centroids)];
      }
    }

    IList<Vector2Int> vertices = new List<Vector2Int>();
    vertices = findVerts(pixelColors);
    assignColliders(vertices, colliders);

    return GetImageFromColorArray(pixelColors);
  }

  private void assignColliders(IList<Vector2Int> vertices, List<Vector2>[] colliders)
  {
    for (int i = 0; i < vertices.Count; i++)
    {
      float smallestDist = float.MaxValue;
      int colliderIndex = 0;

      for (int j = 0; j < colliders.Length; j++)
      {
        Vector2 currentCollider = colliders[j][0];

        if (Vector2.Distance(currentCollider, vertices[i]) < smallestDist)
        {
          smallestDist = Vector2.Distance(currentCollider, vertices[i]);
          colliderIndex = j;
        }
      }

      colliders[colliderIndex].Add(vertices[i]);
    }

    GameObject currentSprite = GameObject.Find("VoronoiDiagram");

    for (int x = 0; x < colliders.Length; x++)
    {

      Vector2 center = colliders[x][0];
      colliders[x].RemoveAt(0);
      List<Vector2> collidersList = colliders[x].ToList();
      
      collidersList.Sort((v, w) => compare(v, w, center));

      Vector2[] pointsArray = new Vector2[collidersList.Count];
      for (int z = 0; z < collidersList.Count; z++)
      {
        float positionY = (collidersList[z].x-imageDimensions.x/2)/100f;
        float positionX = (collidersList[z].y-imageDimensions.y/2)/100f;
        pointsArray[z] = new Vector2(positionX, positionY);
      }

      PolygonCollider2D newCollider = currentSprite.AddComponent<PolygonCollider2D>();
      newCollider.points = pointsArray;
      newCollider.SetPath(0, pointsArray);
      newCollider.isTrigger = true;
      // newCollider.tag = (x).ToString();;
      //newCollider.pathCount = 0;
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


  int GetClosestCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroidVectors)
  {
    float smallestDist = float.MaxValue;
    int index = 0;

    for (int i = 0; i < centroidVectors.Length; i++)
    {
        if (Vector2.Distance(pixelPos, centroidVectors[i]) < smallestDist)
        {
          smallestDist = Vector2.Distance(pixelPos, centroidVectors[i]);
          index = i;
        }
    }
    return index;
  }

  Texture2D GetImageFromColorArray(Color[] pixelColors)
  {
    Texture2D tex = new Texture2D(imageDimensions.x, imageDimensions.y);
    tex.filterMode = FilterMode.Point;

    tex.SetPixels(pixelColors);
    tex.Apply();

    return tex;
  }

  void onTriggerEnter(Collider col)
  {
    Debug.Log($"Object triggered{col}");
  }

  IList<Vector2Int> findVerts(Color[] pixelColors)
  {
    //Vector2Int[] voronoivertex;
    IList<int> allVerts = new List<int>();
    IList<Vector2Int> vertices= new List<Vector2Int>();

    // Turns any pixel with more than 3 different neighbor pixels white (there are some bugs but this is good enough for now)
    for (int row = 0; row < imageDimensions.x; row++)
    {
      for (int col = 0; col < imageDimensions.y; col++)
      {
        int j = row * imageDimensions.x + col;
        int countDifs = 0;

        if (j+imageDimensions.x >= pixelColors.Length)
        {
          countDifs += 1;
        }
        else if (pixelColors[j] != pixelColors[j+imageDimensions.x])
        {
          countDifs += 1;
        } 

        if(j+imageDimensions.x+1 >= pixelColors.Length)
        {
          countDifs += 1;
        }
        else if (pixelColors[j] != pixelColors[j+imageDimensions.x+1])
        {
          countDifs += 1;
        } 
                
        if (j - imageDimensions.x < 0)
        {
          countDifs += 1;
        }
        else if (pixelColors[j] != pixelColors[j - imageDimensions.x]) 
        {
          countDifs += 1;
        }

        if (j - imageDimensions.x +1 < 0)
        {
          countDifs += 1;
        }
        else if (pixelColors[j] != pixelColors[j - imageDimensions.x +1])
        {
          countDifs += 1;
        }

        if (j + 1 >= pixelColors.Length)
        {
          countDifs += 1;
        }
        else if (pixelColors[j] != pixelColors[j + 1])
        {
          countDifs += 1;
        }
        if (j - 1 + imageDimensions.x >= pixelColors.Length)
        {
          countDifs += 1;
        }
        else if (pixelColors[j] != pixelColors[j - 1 + imageDimensions.x])
        {
          countDifs += 1;
        }

        if (j - 1 < 0)
        {
          countDifs += 1;
        } 
        else if (pixelColors[j] != pixelColors[j - 1])
        {
          countDifs += 1;
        }

        if (j - 1 - imageDimensions.x < 0)
        {
          countDifs += 1;
        } 
        else if (pixelColors[j] != pixelColors[j - 1 - imageDimensions.x])
        {
          countDifs += 1;
        }

        if (countDifs > 3)
        {
          allVerts.Add(j);
          vertices.Add(new Vector2Int(row, col));
        }
      }
    }

    // for (int k = 0; k < allVerts.Count; k++)
    // {
    //   pixelColors[allVerts[k]] = Color.white;
    // }

    return vertices;
  }
}
