using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ReturnHome : MonoBehaviour
{
    public void returnHome() 
    {
        if (SceneManager.GetActiveScene().name == "WinningScene"){
            Destroy(GameObject.Find("MeshParent"));
        }

        SceneManager.LoadScene("OpeningScene");

    }
}
