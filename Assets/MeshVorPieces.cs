﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        TestMesh existingBoard = GameObject.Find("MeshParent").GetComponent<TestMesh>();
        colorArray = existingBoard.colorArray;

        colorList = new List<Color>();
        for (int index = 0; index < colorArray.Length; index++)
        {
        colorList.Add(colorArray[index]);
        }

        GenerateTiles();
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

    //Method adapted from Unity School article, November 4, 2015 (http://unity.grogansoft.com/drag-and-drop/)
    private void DragOrPickUp()
    {
    var inputPosition = CurrentTouchPosition;

    if (draggingItem)
    {
        Vector2 newLocation = inputPosition + touchOffset;
        draggedObject.transform.position = new Vector3( newLocation.x, newLocation.y, -1);
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
            draggedObject.transform.localScale = new Vector3(1.2f, 1.2f, -1.2f);
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
    Color draggedObjectColor = draggedObject.GetComponent<Renderer>().material.color;

    // see if dropping item on a collider
    var inputPosition = CurrentTouchPosition;
    Collider2D selectedCollider;

    // Is this layer mask doing anything?
    RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.2f);

    if (touches.Length > 0)
    {

        var hit = touches[0];
        if (hit.collider != null)
        {
            selectedCollider = hit.collider;
            Texture2D colliderTexture = (Texture2D)selectedCollider.gameObject.GetComponent<Renderer>().material.mainTexture;
            Color colliderColor = colliderTexture.GetPixel(0,0);

            // Dont need this
            Debug.Log(RGBdiff (colliderColor, draggedObjectColor));
            
            //Determines if box landed on correct region

            if ((RGBdiff (colliderColor, draggedObjectColor)) < 0.005)
            {
                Debug.Log("match");
                correctMatches.Add(draggedObjectColor);
                Destroy(draggedObject);
            //   puzzleBoard.GetComponent<VoronoiDiagram>().updateIndex = colliderIndex;
            //   puzzleBoard.GetComponent<VoronoiDiagram>().updateNeeded = true;
            }
        }
    }

    draggingItem = false;
    draggedObject.transform.localScale = new Vector3(1f, 1f, -1f);
    }
  
    float RGBdiff(Color c1, Color c2)
    {
        return Mathf.Abs(c1.r - c2.r) + Mathf.Abs(c1.g - c2.g) + Mathf.Abs(c1.b - c2.b);
    }

    private void GenerateVariables()
    {
    TestMesh existingBoard = GameObject.Find("MeshParent").GetComponent<TestMesh>();

    puzzleSize = existingBoard.polygonNumber;
    startColor = existingBoard.startColor;
    endColor = existingBoard.endColor;
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

            tile.transform.position = new Vector3(posX, posY, -1f);
            tile.name = "SquareTile";
            tile.layer = LayerMask.NameToLayer("Piece");

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
        transform.position = new Vector3(-gridWidth/2 + tileSize/2, (gridHeight/2 - tileSize/2)-2, -1);
    }
}
