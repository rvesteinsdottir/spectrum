using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceManager : MonoBehaviour
{
  private int puzzleRows;
  private int puzzleCols;
  private int puzzleSize;
  private Color startColor;
  private Color endColor;
  private IList<Color> colorList;
  private float tileSize = 1f;
  private Hashtable initialPosition;
  private Dictionary<Vector3, Color> desiredTilePosition = new Dictionary<Vector3, Color>();
  private Vector2 touchOffset;
  private bool draggingItem = false;
  private GameObject draggedObject;
  public IList<Color> correctMatches = new List<Color>();

  void Start()
  {
    GenerateVariables();
    GenerateColorArray();
    GenerateTiles();
  }

  private void Update()
  {   
    var onetime = false;
    {
      // Store Dictionary of desired tile position
      if (!onetime)
      {
        GameObject puzzleBoard = GameObject.Find("PuzzleBoard");
        Transform puzzleTransform = puzzleBoard.transform;
        PuzzleBoardManager existingBoard = puzzleBoard.GetComponent<PuzzleBoardManager>();
        desiredTilePosition = existingBoard.tilePositions;
        onetime = true;
      }
    }

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

  // Method adapted from Unity School article, November 4, 2015 (http://unity.grogansoft.com/drag-and-drop/)
  private void DragOrPickUp()
  {
    var inputPosition = CurrentTouchPosition;

    if (draggingItem)
    {
      draggedObject.transform.position = inputPosition + touchOffset;
    }
    else
    {
      RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.2f);

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
    GameObject puzzleBoard = GameObject.Find("PuzzleBoard");
    Transform puzzleTransform = puzzleBoard.transform;
    Color draggedObjectColor = draggedObject.GetComponent<Renderer>().material.color;
    bool objectMatch = false;

    foreach (Transform child in puzzleTransform)
    {
      float xDiff = Mathf.Abs(child.position.x - draggedObject.transform.position.x);
      float yDiff = Mathf.Abs(child.position.y - draggedObject.transform.position.y);

      if ((desiredTilePosition[child.position] == draggedObjectColor) && xDiff <= 0.5f && yDiff <= 0.5f && !correctMatches.Contains(draggedObjectColor))
      {
        draggedObject.transform.position = child.position;
        correctMatches.Add(draggedObjectColor);
        objectMatch = true;
      }

      Debug.Log(correctMatches.Count);
    }

    draggingItem = false;
    objectMatch = false;
    draggedObject.transform.localScale = new Vector3(1f, 1f, 1f);
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
  }

  private void GenerateColorArray()
  {
    Color tileWidth = ((endColor - startColor)/puzzleSize);
    colorList = new List<Color>();

    for (int i = 0; i < puzzleSize; i++)
    {
      colorList.Add(startColor + (tileWidth * i));
    }
  }

  private void GenerateTiles()
  {
    GameObject referenceTile = (GameObject)Instantiate(Resources.Load("SquareTile"));
    initialPosition = new Hashtable();

    for (int i = 0; i < (puzzleSize); i++)
    {
      GameObject tile = (GameObject)Instantiate(referenceTile, transform);

      float posX = i * tileSize;
      float posY = 5;
      if (i >= (puzzleSize)/2)
      {
        posX = (i - (puzzleSize)/2) * tileSize;
        posY -= 1;
      }

      tile.transform.position = new Vector3(posX, posY, 1f);

      // Assign random color to tile
      int randomIndex = Random.Range(0, colorList.Count);
      Color tileColor = colorList[randomIndex];
      colorList.RemoveAt(randomIndex);
      tile.GetComponent<Renderer>().material.color = tileColor;

      initialPosition.Add(tileColor, tile.transform.position);
    }

    Destroy(referenceTile);

    float gridWidth = ((puzzleSize)/2) * tileSize;
    float gridHeight = tileSize * 2;

    //Changes pivot point for tiles is in the center
    transform.position = new Vector2(-gridWidth/2 + tileSize/2, (gridHeight/2 - tileSize/2)-2);

  }
}
