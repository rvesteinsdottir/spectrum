using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPickerSpriteScript : MonoBehaviour
{
    public GameObject ColorPickedPrefab;
    public GameObject ColorPickTile;
    private ColorPickerTriangle CP;
    private bool isPaint = false;
    private GameObject go;
    private GameObject colorOneGo;
    private GameObject colorTwoGo;
    private Material colorOneMat;
    private Material colorTwoMat;
    private GameObject currentColor;
    private GameObject border;


    void Start()
    {
        SetDisplay();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            OnMouseDown();
    }

    private void SetDisplay()
    {
        // Set color wheel
        go = (GameObject)Instantiate(ColorPickedPrefab, transform.position, Quaternion.identity);
        go.transform.position += new Vector3(0,-2,0);
        go.transform.localScale = Vector3.one * 4f;
        go.transform.LookAt(Camera.main.transform);
        CP = go.GetComponent<ColorPickerTriangle>();

        // Display first color
        colorOneGo = (GameObject)Instantiate(ColorPickTile, new Vector3( -1.25f, 1.5f, 0), Quaternion.identity);
        colorOneGo.transform.LookAt(Camera.main.transform);
        colorOneGo.transform.localScale = new Vector3 (0.75f,0.75f,1);
        colorOneMat = colorOneGo.GetComponent<SpriteRenderer>().material;
        colorOneMat.color = HexToColor(PlayerPrefs.GetString("ColorOne", ColorToHex(Color.red)));
        colorOneGo.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        colorOneGo.layer = 9;


        // Display second color
        colorTwoGo = (GameObject)Instantiate(ColorPickTile, new Vector3( 1.25f, 1.5f, 0), Quaternion.identity);
        colorTwoGo.transform.LookAt(Camera.main.transform);
        colorTwoGo.transform.localScale = new Vector3 (0.75f,0.75f,1);
        colorTwoMat = colorTwoGo.GetComponent<SpriteRenderer>().material;
        colorTwoMat.color = HexToColor(PlayerPrefs.GetString("ColorTwo", ColorToHex(Color.blue)));
        colorTwoGo.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        colorTwoGo.layer = 9;


        // Display border
        currentColor = colorOneGo;

        border = (GameObject)Instantiate(ColorPickTile, transform.position, Quaternion.identity);
        border.transform.position = currentColor.transform.position;
        border.transform.localScale = new Vector3 (0.8f, 0.8f, 1);
        border.GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 0.75f);
        border.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        border.layer = 8;
    }

    void OnMouseDown()
    {
        if (isColorBox())
            ChangeColor();
        else if (isPaint)
            StopPaint();
        else
            StartPaint();
    }

    bool isColorBox()
    {        
        var inputPosition = CurrentTouchPosition;
        int BoardLayerMask =~ LayerMask.GetMask("Board");

        RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.2f, BoardLayerMask);        

        if (touches.Length > 0)
        {
            var hit = touches[0];
            if (hit.transform != null)
            {
                currentColor = hit.transform.gameObject;
                return true;
            }
        }

        return false;
    }

    private void ChangeColor()
    {
        border.transform.position = currentColor.transform.position;
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

    private void StartPaint()
    {

        CP.SetNewColor(currentColor.GetComponent<SpriteRenderer>().material.color);
        isPaint = true;
    }

    private void StopPaint()
    {
        currentColor.GetComponent<SpriteRenderer>().material.color = CP.TheColor;
        if(currentColor == colorOneGo)
        {
            PlayerPrefs.SetString("ColorOne", ColorToHex(CP.TheColor));
        }
        else
        {
            PlayerPrefs.SetString("ColorTwo", ColorToHex(CP.TheColor));
        }
        isPaint = false;
    }

    public void RefreshGame()
    {
        colorOneGo.GetComponent<SpriteRenderer>().material.color = HexToColor(PlayerPrefs.GetString("ColorOne"));
        colorTwoGo.GetComponent<SpriteRenderer>().material.color = HexToColor(PlayerPrefs.GetString("ColorTwo"));
    }


    //CREDIT
    string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }
    
    Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        return new Color(r/255f, g/255f, b/255f , 1);
    }
}
