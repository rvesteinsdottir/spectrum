using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorIndicator : MonoBehaviour
{
  private int puzzleRows;
  private int puzzleCols;
  private int puzzleSize;
  private Color startColor;
  private Color endColor;
  private float tileSize;


  void Start()
  {
    GenerateVariables();
    GenerateIndicators();
  }

  private void GenerateVariables()
  {
    GameObject puzzleBoard = GameObject.Find("PuzzleBoard");
    Transform puzzleTransform = puzzleBoard.transform;
    PuzzleBoardManager existingBoard = puzzleBoard.GetComponent<PuzzleBoardManager>();

    puzzleRows = existingBoard.rows;
    puzzleCols = existingBoard.cols;
    puzzleSize = puzzleRows * puzzleCols;
    startColor = existingBoard.startColor;
    endColor = existingBoard.endColor;
    tileSize = existingBoard.tileSize;
  }

  private void GenerateIndicators()
  {
    GameObject referenceIndicator = (GameObject)Instantiate(Resources.Load("ColorIndicator"));

    GameObject startIndicator = (GameObject)Instantiate(referenceIndicator, transform);
    GameObject endIndicator = (GameObject)Instantiate(referenceIndicator, transform);

    float gridWidth = puzzleCols * tileSize;
    float gridHeight = puzzleRows * tileSize;

    // Changes pivot point for tiles is in the center
    
    startIndicator.GetComponent<Renderer>().material.color = startColor;
    endIndicator.GetComponent<Renderer>().material.color = endColor;
    transform.localScale = new Vector3(3, 0.2f, 1);
    


    transform.position = new Vector2(-gridWidth/2 + tileSize/2, (gridHeight/2 - tileSize/2)-2);
    startIndicator.transform.position = new Vector2(-tileSize * (puzzleCols/4),tileSize/4);
    endIndicator.transform.position = new Vector2(tileSize * (puzzleCols/4),-tileSize * (puzzleRows + 0.5f));

    // transform.position


    Destroy(referenceIndicator);
  }

}
