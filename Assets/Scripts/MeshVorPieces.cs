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
        GenerateVariables();
    }

    private void Update()
    {   
        // Store Dictionary of desired tile position
        if (!onetime)
        {
            onetime = true;

            TestMesh existingBoard = GameObject.Find("MeshParent").GetComponent<TestMesh>();
            puzzleSize = existingBoard.polygonNumber;
            colorArray = existingBoard.colorArray;
            allColliders = existingBoard.allColliders;

            colorList = new List<Color>();
            for (int index = 0; index < colorArray.Length; index++)
            {
                colorList.Add(colorArray[index]);
            }

            GenerateTiles();
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

    public void PlayClick()
    {
        Click.Play();
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

    //Method adapted from Unity School article, November 4, 2015 (http://unity.grogansoft.com/drag-and-drop/)
    private void DragOrPickUp()
    {
        var inputPosition = CurrentTouchPosition;

        if (draggingItem)
        {
            Vector2 newLocation = inputPosition + touchOffset;
            draggedObject.transform.position = new Vector3( newLocation.x, newLocation.y, -1f);

            //draggedObject.transform.position = newLocation;
        }
        else
        {
            int BoardLayerMask =~ LayerMask.GetMask("Board");

            RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.2f, BoardLayerMask);

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
        TestMesh existingBoard = GameObject.Find("MeshParent").GetComponent<TestMesh>();
        draggedObjectColor = draggedObject.GetComponent<Renderer>().material.color;

        // see if dropping item on a collider
        var inputPosition = CurrentTouchPosition;
        Collider2D selectedCollider;

        // draggedObject.transform.localPosition = new Vector3(draggedObject.transform.localPosition.x, draggedObject.transform.localPosition.y, -2);

        // Is this layer mask doing anything?
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

                    ChangeObjectColor(selectedCollider);
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

    private void ChangeObjectColor(Collider2D selectedCollider)
    {
        Material objectMaterial = selectedCollider.gameObject.GetComponent<Renderer>().material;
        Texture2D texture = new Texture2D(128, 128);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
                texture.SetPixel(x, y, draggedObjectColor);
        }
        texture.Apply();

        objectMaterial.mainTexture = texture;
        selectedCollider.gameObject.GetComponent<MeshRenderer>().material = objectMaterial; 
    }
  
    float RGBdiff(Color c1, Color c2)
    {
        return Mathf.Abs(c1.r - c2.r) + Mathf.Abs(c1.g - c2.g) + Mathf.Abs(c1.b - c2.b);
    }

    private void GenerateVariables()
    {
        TestMesh existingBoard = GameObject.Find("MeshParent").GetComponent<TestMesh>();

        //puzzleSize = existingBoard.polygonNumber;
        startColor = existingBoard.startColor;
        endColor = existingBoard.endColor;
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

            //initialPosition.Add(tileColor, tile.transform.position);
        }

        Destroy(referenceTile);

        //Changes pivot point for tiles is in the center
        transform.position = new Vector3((-(gridWidth/2 + tileSize/2)), (gridHeight/2 - tileSize/2)-3, transform.position.z);

        GameObject puzzleBank = (GameObject)Instantiate(Resources.Load("PuzzleBank2"), transform);
        puzzleBank.GetComponent<Renderer>().material.color = new Color (1, 1, 1);
        puzzleBank.transform.position = new Vector3(0f, 2.52f, 0);

        puzzleBank.name = "PuzzleBank";
        puzzleBank.layer = LayerMask.NameToLayer("Board");
        puzzleBank.transform.localScale = new Vector3((1f + (0.5f * puzzleSize/2)), 3.5f, 0);


        for (int j = 0; j < transform.childCount; j++)
        {
            Transform child = transform.GetChild(j); 
            initialPosition.Add(child.GetComponent<Renderer>().material.color, child.position);
        }

    }

    private void DisplayWinningScreen()
    {
        DontDestroyOnLoad(GameObject.Find("MeshParent"));
        SceneManager.LoadScene("WinningScene");
    }
}
