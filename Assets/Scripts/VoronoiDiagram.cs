using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code adapted from Voronoi diagram tutorial in Unity3D(C#) by upgames published April 21, 2019 (https://www.youtube.com/watch?v=EDv69onIETk)

public class VoronoiDiagram : MonoBehaviour
{
  public Vector2Int imageDimensions;
  public int regionAmount;
  public Color startColor = new Color(255f/255f, 0/255f, 0/255f);
  public Color endColor = new Color(0/255f, 0/255f, 255f/255f);

  private void Start()
  {
    GetComponent<SpriteRenderer>().sprite = Sprite.Create(GetDiagram(), new Rect(0,0, imageDimensions.x, imageDimensions.y), Vector2.one * 0.5f);
  }
  
  Texture2D GetDiagram()
  {
    Vector2Int[] centroids = new Vector2Int[regionAmount];
    Color[] regions = new Color[regionAmount];
    Color tileWidth = ((endColor - startColor)/imageDimensions.x);

    for (int i = 0; i < regionAmount; i++)
    {
      centroids[i] = new Vector2Int(Random.Range(0, imageDimensions.x), Random.Range(0, imageDimensions.y));
      regions[i] = startColor + (tileWidth * centroids[i].x);

    }
      
    Color[] pixelColors = new Color[imageDimensions.x * imageDimensions.y];

    for (int x = 0; x < imageDimensions.x; x++)
    {
      for (int y = 0; y < imageDimensions.y; y++)
      {
        int index = x * imageDimensions.x + y;
        pixelColors[index] = regions[GetClosestCentroidIndex(new Vector2Int(x, y), centroids)];
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
        if ((Vector2.Distance(pixelPos, centroidVectors[i]) < smallestDist))
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

  // public void GenerateColorArray()
  // {
  //   Color tileWidth = ((endColor - startColor)/(cols * rows));

  //   colorArray = new Color[rows, cols];
  //   int x = 0;

  //   for (int row = 0; row < rows; row++)
  //   {
  //     for (int col = 0; col < cols; col++)
  //     {
  //       colorArray[row,col] = (startColor + (tileWidth * x));
  //       x += 1;
  //     }
  //   }
  // }

}
