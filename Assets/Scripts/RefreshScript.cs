using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RefreshScript : MonoBehaviour
{
    public void refreshGame() 
    {
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetString("ColorOne", ColorToHex(Color.red));
        PlayerPrefs.SetString("ColorTwo", ColorToHex(Color.blue));

        if (SceneManager.GetActiveScene().name == "ColorPickScene")
        {
            GameObject.Find("ColorPicker").GetComponent<ColorPickerSpriteScript>().RefreshGame();
        }
    }

    string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        Debug.Log(hex);
        return hex;
    }
}
