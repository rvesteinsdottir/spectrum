using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPickerSpriteScript : MonoBehaviour
{
    public GameObject ColorPickedPrefab;
    public GameObject RoundedTile;
    private ColorPickerTriangle CP;
    private bool isPaint = false;
    private GameObject go;
    private GameObject colorOneGo;
    private GameObject colorTwoGo;
    private Material colorOneMat;
    private Material colorTwoMat;
    private GameObject currentColor;
    private GameObject border;
    // public float spriteBlinkingTimer = 0.0f;
    // public float spriteBlinkingTotalTimer = 0.0f;
    // public float spriteBlinkingTotalDuration = 3.0f;
    // public bool startBlinking = true;
    // float timer = 0.0f;

    void Start()
    {
        SetDisplay();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            OnMouseDown();

        // if (startBlinking == true)
        // {
        //     StartBlinkingEffect();
        // }
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
        colorOneGo = (GameObject)Instantiate(RoundedTile, new Vector3( -1.25f, 1.5f, 0), Quaternion.identity);
        colorOneGo.transform.LookAt(Camera.main.transform);
        colorOneGo.transform.localScale = new Vector3 (3,2,1);
        colorOneMat = colorOneGo.GetComponent<SpriteRenderer>().material;
        colorOneMat.color = HexToColor(PlayerPrefs.GetString("ColorOne", ColorToHex(Color.red)));
        colorOneGo.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        colorOneGo.layer = 9;


        // Display second color
        colorTwoGo = (GameObject)Instantiate(RoundedTile, new Vector3( 1.25f, 1.5f, 0), Quaternion.identity);
        colorTwoGo.transform.LookAt(Camera.main.transform);
        colorTwoGo.transform.localScale = new Vector3 (3,2,1);
        colorTwoMat = colorTwoGo.GetComponent<SpriteRenderer>().material;
        colorTwoMat.color = HexToColor(PlayerPrefs.GetString("ColorTwo", ColorToHex(Color.blue)));
        colorTwoGo.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        colorTwoGo.layer = 9;


        // Display border
        currentColor = colorOneGo;

        border = (GameObject)Instantiate(RoundedTile, transform.position, Quaternion.identity);
        border.transform.position = currentColor.transform.position;
        border.transform.localScale = new Vector3 (3.1f, 2.1f, 1);
        border.GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 0.75f);
        border.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        border.layer = 8;
    }

    // // From stack overflow (https://answers.unity.com/questions/1134985/sprite-blinking-effect-when-player-hit.html)
    // private void StartBlinkingEffect()
    // {
    //     timer += Time.deltaTime;

    //     if (Mathf.Round(timer * 1000) % 200f == 0)
    //     {
    //         Debug.Log(timer);
    //         if (border.gameObject.GetComponent<SpriteRenderer> ().enabled == true) {
    //             border.gameObject.GetComponent<SpriteRenderer> ().enabled = false; 
    //         } 
    //         else 
    //         {
    //             border.gameObject.GetComponent<SpriteRenderer> ().enabled = true;   
    //         }
    //     }
    // }

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

    //CREDIT
    string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        Debug.Log(hex);
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
