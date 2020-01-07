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
    Debug.Log(collider.bounds);
  }
  
  Texture2D GetDiagram()
  {
    Vector2Int[] centroids = new Vector2Int[regionAmount];
    Color[] regions = new Color[regionAmount];
    Color tileWidth = ((endColor - startColor)/imageDimensions.x);
    GameObject newSprite = GameObject.Find("New Sprite");

    for (int i = 0; i < regionAmount; i++)
    {
      centroids[i] = new Vector2Int(Random.Range(0, imageDimensions.x), Random.Range(0, imageDimensions.y));

      //add new polygon collider to the centroid of each voronoi graph
      float posX = (centroids[i].x-imageDimensions.x/2)/100f;
      float posY = (centroids[i].y-imageDimensions.y/2)/100f;
      newSprite.AddComponent<PolygonCollider2D>().offset = new Vector2(posX, posY);

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

  //Vector2Int[] voronoivertex;
    IList<int> allVerts = new List<int>();
    
    // Turns any pixel with more than 3 different neighbor pixels white
    for (int j = 0; j < pixelColors.Length; j++)
    {
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
      else if(pixelColors[j] != pixelColors[j - imageDimensions.x]) 
      {
        countDifs += 1;
      }

      if (j - imageDimensions.x +1 < 0)
      {
        countDifs += 1;

      }
      else if(pixelColors[j] != pixelColors[j - imageDimensions.x +1])
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
        allVerts.Add(j);
      
    }

    for (int k = 0; k < allVerts.Count; k++)
    {
      pixelColors[allVerts[k]] = Color.white;
      if (allVerts[k] % imageDimensions.x == 0)
      {
        Debug.Log(allVerts[k]);
      }
    }

    return GetImageFromColorArray(pixelColors);
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
}
