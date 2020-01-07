using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code adapted from Voronoi diagram tutorial in Unity3D(C#) by Upgames published April 21, 2019 (https://www.youtube.com/watch?v=EDv69onIETk)

public class VoronoiDiagram : MonoBehaviour
{
  public Vector2Int imageDimensions;
  public int regionAmount;
  public Color startColor = new Color(255f/255f, 0/255f, 0/255f);
  public Color endColor = new Color(0/255f, 0/255f, 255f/255f);

  private void Start()
  {
    
    GetComponent<SpriteRenderer>().sprite = Sprite.Create(GetDiagram(), new Rect(0,0, imageDimensions.x, imageDimensions.y), Vector2.one * 0.5f);

    var collider = GetComponent<PolygonCollider2D>();
  }
  
  Texture2D GetDiagram()
  {
    Vector2Int[] centroids = new Vector2Int[regionAmount];
    Color[] regions = new Color[regionAmount];
    Color tileWidth = ((endColor - startColor)/imageDimensions.x);
    GameObject newSprite = GameObject.Find("New Sprite");
    List<Vector2>[] colliders = new List<Vector2>[regionAmount];
    //Dictionary<Vector2Int, IList> colliders = new Dictionary<Vector2Int, IList>();
    
    for (int i = 0; i < regionAmount; i++)
    {
      centroids[i] = new Vector2Int(Random.Range(0, imageDimensions.x), Random.Range(0, imageDimensions.y));

      //add new polygon collider to the centroid of each voronoi graph
      //float posX = (centroids[i].x-imageDimensions.x/2)/100f;
      //float posY = (centroids[i].y-imageDimensions.y/2)/100f;
      //newSprite.AddComponent<PolygonCollider2D>().offset = new Vector2(posX, posY);
      colliders[i] = new List<Vector2>(){ centroids[i] };
      //IList<Vector2Int> associatedVerts = new List<Vector2Int>();

      //colliders.Add(centroids[i], associatedVerts);

      regions[i] = startColor + (tileWidth * centroids[i].x);
    }

    // add collider for testing
    // PolygonCollider2D newCollider = newSprite.AddComponent<PolygonCollider2D>();
    // newCollider.offset = new Vector2(0 , 0);
    // newCollider.points = new[]{new Vector2(0,0), new Vector2(1,1), new Vector2(3,6), new Vector2(3,4)};
    // newCollider.SetPath (0, new[]{ new Vector2(0,0), new Vector2(1,1), new Vector2(3,6), new Vector2(3,4) });
      
    Color[] pixelColors = new Color[imageDimensions.x * imageDimensions.y];

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
    //Debug.Log($"Total vertices {vertices.Count}");
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
      //Debug.Log($"vertex index{i}: {vertices[i]}, collider: {colliders[colliderIndex][0]}");
    }

    GameObject currentSprite = GameObject.Find("New Sprite");

    for (int x = 0; x < colliders.Length; x++)
    {
      PolygonCollider2D newCollider = currentSprite.AddComponent<PolygonCollider2D>();
      float posX = (colliders[x][0].x-imageDimensions.x/2)/100f;
      float posY = (colliders[x][0].y-imageDimensions.y/2)/100f;
      newCollider.offset = new Vector2(posX, posY);

      int length = colliders[x].Count - 1;
      Vector2[] pointsArray = new Vector2[length];
      for (int z = 1; z < length; z++)
      {
        float positionX = colliders[x][z].x;
        float positionY =  colliders[x][z].y;
        pointsArray[z-1] = new Vector2(positionX, positionY);
      }

      Debug.Log(pointsArray.GetType());
      // newCollider.points = new[]{ pointsArray };
      // newCollider.SetPath (0, new[]{ pointsArray });

    }
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

        if (countDifs > 4)
        {
          allVerts.Add(j);
          vertices.Add(new Vector2Int(row, col));
        }
      }
    }

    for (int k = 0; k < allVerts.Count; k++)
    {
      pixelColors[allVerts[k]] = Color.white;
    }

    return vertices;
  }
}
