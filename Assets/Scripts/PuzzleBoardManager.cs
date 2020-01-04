using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBoardManager : MonoBehaviour
{
  //[SerializeField]
  public int rows = 4;
  //[SerializeField]
  public int cols = 4;
  //[SerializeField]
  public float tileSize = 0.9f;
  public Color startColor = new Color(255f/255f, 0/255f, 0/255f);
  public Color endColor = new Color(0/255f, 0/255f, 255f/255f);
  public Color[,] colorArray;
  public Dictionary<Vector3, Color> tilePositions;

  void Start()
  {
    GenerateColorArray();
    GenerateGrid();
  }

  private void GenerateGrid()
  {
    GameObject referenceTile = (GameObject)Instantiate(Resources.Load("HexTile"));

    float gridWidth = cols * tileSize;
    float gridHeight = rows * tileSize;
    tilePositions = new Dictionary<Vector3, Color>();

    for (int row = 0; row < rows; row++)
    {
      for (int col = 0; col < cols; col++)
      {
        GameObject tile = (GameObject)Instantiate(referenceTile, transform);

        float posX = col * tileSize;
        if (row % 2 == 0) {
          posX -= 0.3f;
        }
        float posY = row * -tileSize;

        // Changes pivot point for tiles is in the center
        tile.transform.position = new Vector2(-gridWidth/2 + tileSize/2 + posX, (gridHeight/2 - tileSize/2)-2 + posY);
        tile.transform.Rotate(0.0f, 0.0f, 50.0f, Space.World);

        Color tileColor = colorArray[row, col];
        tile.GetComponent<Renderer>().material.color = new Color (194/255f, 194/255f, 194/255f);
        tilePositions.Add(tile.transform.position, tileColor);
      }
    }

    Destroy(referenceTile);
  }

  public void GenerateColorArray()
  {
    Color tileWidth = ((endColor - startColor)/(cols * rows));

    colorArray = new Color[rows, cols];
    int x = 0;

    for (int row = 0; row < rows; row++)
    {
      for (int col = 0; col < cols; col++)
      {
        colorArray[row,col] = (startColor + (tileWidth * x));
        x += 1;
      }
    }
  }
}