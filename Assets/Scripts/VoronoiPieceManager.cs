using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiPieceManager : MonoBehaviour
{

  private int puzzleSize;
  private Color startColor;
  private Color endColor;
  private Color[] pixelColors;
  private List<Color> colorList;
  private Color[] colorArray;
  private Hashtable initialPosition;
  // private Dictionary<Vector3, Color> desiredTilePosition = new Dictionary<Vector3, Color>();
  private Vector2 touchOffset;
  private bool draggingItem = false;
  private GameObject draggedObject;
  public IList<Color> correctMatches = new List<Color>();
  private bool onetime = false;
  private PolygonCollider2D[] allColliders;

  void Start()
  {
    GenerateVariables();
  }

  private void Update()
  {   
    // Store Dictionary of desired tile position
    if (!onetime)
    {
      GameObject puzzleBoard = GameObject.Find("VoronoiDiagram");
      Transform puzzleTransform = puzzleBoard.transform;
      VoronoiDiagram existingBoard = puzzleBoard.GetComponent<VoronoiDiagram>();
      colorArray = existingBoard.regions;
      pixelColors = existingBoard.pixelColors;
      allColliders = existingBoard.allColliders;

      colorList = new List<Color>();
      for (int index = 0; index < colorArray.Length; index++)
      {
          colorList.Add(colorArray[index]);
      }


      GenerateTiles();

      // desiredTilePosition = existingBoard.tilePositions;
      onetime = true;
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
    GameObject puzzleBoard = GameObject.Find("VoronoiDiagram");
    Transform puzzleTransform = puzzleBoard.transform;
    Color draggedObjectColor = draggedObject.GetComponent<Renderer>().material.color;


    // see if dropping item on a collider
    var inputPosition = CurrentTouchPosition;
    Collider2D selectedCollider;
    int PiecesLayerMask = 1 << 8;
    RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.2f, PiecesLayerMask);

    if (touches.Length > 0)
    {
      var hit = touches[0];
      if (hit.collider != null)
      {
        selectedCollider = hit.collider;
        int colliderIndex = System.Array.IndexOf(allColliders, selectedCollider);
        int boxIndex = System.Array.IndexOf(colorArray, draggedObjectColor);

        // Determines if box landed on correct region
        if (colliderIndex == boxIndex)
        {
          correctMatches.Add(draggedObjectColor);
          puzzleBoard.GetComponent<VoronoiDiagram>().updateNeeded = true;
        }
      }
    }

    draggingItem = false;
    draggedObject.transform.localScale = new Vector3(1f, 1f, 1f);
  }

  private void GenerateVariables()
  {
    GameObject puzzleBoard = GameObject.Find("VoronoiDiagram");
    Transform puzzleTransform = puzzleBoard.transform;
    VoronoiDiagram existingBoard = puzzleBoard.GetComponent<VoronoiDiagram>();

    puzzleSize = existingBoard.regionAmount;
    startColor = existingBoard.startColor;
    endColor = existingBoard.endColor;
    pixelColors = existingBoard.pixelColors;
  }

  private void GenerateTiles()
  {
    GameObject referenceTile = (GameObject)Instantiate(Resources.Load("SquareTile"));
    initialPosition = new Hashtable();
    int tileSize = 1;

    for (int i = 0; i < puzzleSize; i++)
    {
      GameObject tile = (GameObject)Instantiate(referenceTile, transform);

      float posX = i * tileSize + 0.5f;
      float posY = 5;
      if (i >= puzzleSize/2)
      {
        posX = (i - puzzleSize/2) * tileSize + 0.5f;
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
