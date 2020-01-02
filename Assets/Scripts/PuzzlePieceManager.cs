using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceManager : MonoBehaviour
{
  private int puzzleRows;
  private int puzzleCols;
  private Color startColor;
  private Color endColor;
  private IList<Color> colorList;
  private float tileSize = 1f;

  // Implementation for having the color snap back if incorrect location
  private Transform[] colorLocation;
  private Vector2[] initialPosition;
  private Vector2 touchOffset;
  public bool[] locked;
  private bool draggingItem = false;

  //This may need to be something other than game object
  private GameObject draggedObject;

  // Start is called before the first frame update
  void Start()
  {
    GenerateVariables();
    GenerateColorArray();
    GenerateTiles();
  }

  private void Update()
  {
    // excluded " && !locked" here
    if (HasInput)
    {
      DragOrPickUp();
    }
    else
    {
      if (draggingItem)
        DropItem();
    }
  }

  Vector2 CurrentTouchPosition
  {
    get
    {
      Vector2 inputPos;
      inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      return inputPos;
    }
  }

  private void DragOrPickUp()
  {
    var inputPosition = CurrentTouchPosition;

    if (draggingItem)
    {
      draggedObject.transform.position = inputPosition + touchOffset;
    }
    else
    {
      RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.5f);

      if (touches.Length > 0)
      {
        var hit = touches[0];
        if (hit.transform != null)
        {
          draggingItem = true;
          draggedObject = hit.transform.gameObject;
          touchOffset = (Vector2)hit.transform.position - inputPosition;
          draggedObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
      }
    }
  }

  private bool HasInput
  {
    get
    {
      return Input.GetMouseButton(0);
    }
  }

  void DropItem()
  {
    draggingItem = false;
    draggedObject.transform.localScale = new Vector3(1f, 1f, 1f);
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

    colorList = new List<Color>();

    for (int i = 0; i < tileCount; i++)
    {
      colorList.Add(startColor + (tileWidth * i));
    }
  }

  private void GenerateTiles()
  {
    GameObject referenceTile = (GameObject)Instantiate(Resources.Load("SquareTile"));
    initialPosition = new Vector2[puzzleCols * puzzleRows];

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
        initialPosition[i] = tile.transform.position;

        var tileRenderer = tile.GetComponent<Renderer>();

        int end = colorList.Count;
        int start = 0;
        int randomIndex = Random.Range(start, end);
        Color tileColor = colorList[randomIndex];
        colorList.RemoveAt(randomIndex);
        tileRenderer.material.color = tileColor;
      }

    Destroy(referenceTile);

    float gridWidth = ((puzzleCols * puzzleRows)/2) * tileSize;
    float gridHeight = tileSize * 2;

    //Changes pivot point for tiles is in the center
    transform.position = new Vector2(-gridWidth/2 + tileSize/2, (gridHeight/2 - tileSize/2)-2);

  }
}
