using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceManager : MonoBehaviour
{
  private int puzzleRows;
  private int puzzleCols;
  private Color startColor;
  private Color endColor;
  private Color[] colorArray;
  private float tileSize = 1f;

  // Start is called before the first frame update
  void Start()
  {
    GenerateVariables();
    GenerateColorArray();
    GenerateTiles();
  }

  private void GenerateVariables()
  {
    GameObject puzzleBoard = GameObject.Find("PuzzleBoard");

    PuzzleBoardManager existingBoard = puzzleBoard.GetComponent<PuzzleBoardManager>();

    puzzleRows = existingBoard.rows;
    puzzleCols = existingBoard.cols;
    startColor = existingBoard.startColor;
    endColor = existingBoard.endColor;
  }

  private void GenerateColorArray()
  {
    int tileCount = (puzzleCols * puzzleRows);
    Color tileWidth = ((endColor - startColor)/tileCount);

    colorArray = new Color[tileCount];

    for (int i = 0; i < tileCount; i++)
    {
      colorArray[i] = (startColor + (tileWidth * i));
    }
  }

  private void GenerateTiles()
  {
    GameObject referenceTile = (GameObject)Instantiate(Resources.Load("SquareTile"));

      for (int i = 0; i < (puzzleCols * puzzleRows); i++)
      {
        GameObject tile = (GameObject)Instantiate(referenceTile, transform);

        float posX;
        float posY;
        if (i < (puzzleCols * puzzleRows)/2)
        {
          posX = i * tileSize;
          posY = 5;
        } else
        {
          posX = (i - (puzzleCols * puzzleRows)/2) * tileSize;
          posY = 4;
        }

        tile.transform.position = new Vector2(posX, posY);

        var tileRenderer = tile.GetComponent<Renderer>();

        int end = colorArray.Length;
        int start = 0;
        int randomIndex = Random.Range(start, end);
        Color tileColor = colorArray[randomIndex];
        //colorArray.Controls.RemoveAt(randomIndex);
        //colorArray = colorArray.Except(new Color[]{tileColor}).ToArray();

        tileRenderer.material.color = tileColor;
      }

    Destroy(referenceTile);

    float gridWidth = ((puzzleCols * puzzleRows)/2) * tileSize;
    float gridHeight = tileSize * 2;

    transform.position = new Vector2(-gridWidth/2 + tileSize/2, (gridHeight/2 - tileSize/2)-2);


  }
}
