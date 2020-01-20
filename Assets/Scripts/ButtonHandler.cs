using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public void ChooseLevel() 
    {   
        Button button = GetComponent<Button>();
        switch (button.name)
        {
            case "Easy":
                PlayerPrefs.SetInt("Level", 1);
                SceneManager.LoadScene("GameScene");
                break;
            case "Medium":
                PlayerPrefs.SetInt("Level", 2);
                SceneManager.LoadScene("GameScene");
                break;
            case "Hard":
                PlayerPrefs.SetInt("Level", 3);
                SceneManager.LoadScene("GameScene");
                break;
        }
    }

    public void SelectGameColors()
    {
        SceneManager.LoadScene("ColorPickScene");
    }

    public void ReturnHome() 
    {
        if (SceneManager.GetActiveScene().name == "WinningScene"){
            Destroy(GameObject.Find("VoronoiDiagram"));
        }

        SceneManager.LoadScene("OpeningScene");
    }

    public void RefreshGame() 
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
