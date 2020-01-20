using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MeshVorPieces : MonoBehaviour
{
    private int puzzleSize;
    private Color startColor;
    private Color endColor;
    private Color[] pixelColors;
    private List<Color> colorList;
    private Color[] colorArray;
    private Hashtable initialPosition;
    private Vector2 touchOffset;
    private Vector3 touchOffset3D;
    private bool draggingItem = false;
    private GameObject draggedObject;
    private Color draggedObjectColor;
    public IList<Color> correctMatches = new List<Color>();
    private bool onetime = false;
    private PolygonCollider2D[] allColliders;
    public AudioSource Click;

   
    void Start()
    {
        MeshVorDiagram existingBoard = GameObject.Find("VoronoiDiagram").GetComponent<MeshVorDiagram>();

        startColor = existingBoard.startColor;
        endColor = existingBoard.endColor;
    }

    private void Update()
    {   
        if (!onetime)
        {
            onetime = true;

            MeshVorDiagram existingBoard = GameObject.Find("VoronoiDiagram").GetComponent<MeshVorDiagram>();
            puzzleSize = existingBoard.polygonNumber;
            colorArray = existingBoard.colorArray;
            allColliders = existingBoard.allColliders;

            colorList = new List<Color>();
            for (int colorIndex = 0; colorIndex < colorArray.Length; colorIndex++)
            {
                colorList.Add(colorArray[colorIndex]);
            }

            GenerateTiles();
        }

        if (HasInput)
            DragOrPickUp();
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

    private void PlayClick()
    {
        Click.Play();
    }

    // Method adapted from Unity School article, November 4, 2015 (http://unity.grogansoft.com/drag-and-drop/)
    private void DragOrPickUp()
    {
        var inputPosition = CurrentTouchPosition;

        if (draggingItem)
        {
            Vector2 newLocation = inputPosition + touchOffset;
            draggedObject.transform.position = new Vector3( newLocation.x, newLocation.y, -3f);
        }
        else
        {
            int BoardLayerMask =~ LayerMask.GetMask("Board");

            RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, -2.2f, BoardLayerMask);

            if (touches.Length > 0)
            {
                var hit = touches[0];
                if (hit.transform != null)
                {
                    draggingItem = true;
                    draggedObject = hit.transform.gameObject;
                    touchOffset = (Vector2)hit.transform.position - inputPosition;

                    // Increase object size when being dragged
                    draggedObject.transform.localScale = new Vector3(1.7f, 1.7f, 1f);
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
        MeshVorDiagram existingBoard = GameObject.Find("VoronoiDiagram").GetComponent<MeshVorDiagram>();
        draggedObjectColor = draggedObject.GetComponent<Renderer>().material.color;
        var inputPosition = CurrentTouchPosition;
        Collider2D selectedCollider;

        RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.2f);

        if (touches.Length > 0)
        {
            var hit = touches[0];
            if (hit.collider != null)
            {
                selectedCollider = hit.collider;
                int colliderIndex = System.Array.IndexOf(allColliders, selectedCollider);
                int boxIndex = System.Array.IndexOf(colorArray, draggedObjectColor);

                // Determines if box was placed on correct region
                if (colliderIndex == boxIndex)
                {
                    correctMatches.Add(draggedObjectColor);
                    selectedCollider.gameObject.GetComponent<MeshRenderer>().enabled = true;

                    PlayClick();
                    Destroy(draggedObject);
                } else 
                {
                    draggedObject.transform.position = ((Vector3)initialPosition[draggedObjectColor]);
                }

                if (correctMatches.Count == puzzleSize)
                {
                    int currentScore = PlayerPrefs.GetInt("Score");
                    PlayerPrefs.SetInt("Score", currentScore + puzzleSize);
                    DisplayWinningScreen();
                }
            }
        }

        draggingItem = false;
        draggedObject.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
    }

    private void GenerateTiles()
    {
        GameObject referenceTile = (GameObject)Instantiate(Resources.Load("RoundedTile"));
        initialPosition = new Hashtable();
        int tileSize = 1;
        float gridWidth = ((puzzleSize)/2) * tileSize;
        float gridHeight = tileSize * 2;

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

            tile.transform.position = new Vector3(posX, posY, -3f);
            tile.transform.localScale = new Vector3(1.5f, 1.5f, 0);
            tile.name = "RoundedTile";
            tile.layer = LayerMask.NameToLayer("Piece");

            // Assign random color to tile
            int randomIndex = Random.Range(0, colorList.Count);
            Color tileColor = colorList[randomIndex];
            colorList.RemoveAt(randomIndex);
            tile.GetComponent<Renderer>().material.color = tileColor;
        }

        Destroy(referenceTile);

        transform.position = new Vector3((-(gridWidth/2 + tileSize/2)), (gridHeight/2 - tileSize/2)-3, transform.position.z);

        CreatePuzzleBank();
        AddChildrenToTranform();
    }

    private void CreatePuzzleBank()
    {
        GameObject puzzleBank = (GameObject)Instantiate(Resources.Load("PuzzleBank2"), transform);
        puzzleBank.name = "PuzzleBank";
        puzzleBank.layer = LayerMask.NameToLayer("Board");
        puzzleBank.GetComponent<Renderer>().material.color = new Color (1, 1, 1);
        puzzleBank.transform.position = new Vector3(0f, 2.52f, 0);
        puzzleBank.transform.localScale = new Vector3((1f + (0.5f * puzzleSize/2)), 3.5f, 0);
    }

    private void AddChildrenToTranform()
    {
        for (int transformChildIndex = 0; transformChildIndex < transform.childCount; transformChildIndex++)
        {
            Transform child = transform.GetChild(transformChildIndex); 
            initialPosition.Add(child.GetComponent<Renderer>().material.color, child.position);
        }
    }

    private void DisplayWinningScreen()
    {
        DontDestroyOnLoad(GameObject.Find("VoronoiDiagram"));
        SceneManager.LoadScene("WinningScene");
    }
}
