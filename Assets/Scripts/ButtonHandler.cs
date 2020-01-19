using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{

    public void playgame() 
    {   
        Button button = GetComponent<Button>();
        switch (button.name)
        {
            case "easy":
            PlayerPrefs.SetInt("Level", 1);
            SceneManager.LoadScene("GameScene");
            break;
            case "medium":
            PlayerPrefs.SetInt("Level", 2);
            SceneManager.LoadScene("GameScene");
            break;
            case "hard":
            PlayerPrefs.SetInt("Level", 3);
            SceneManager.LoadScene("GameScene");
            break;
            case "selectColor":
            SceneManager.LoadScene("ColorPickScene");
            break;
        }
    }
}
