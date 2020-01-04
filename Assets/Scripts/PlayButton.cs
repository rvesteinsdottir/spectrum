using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
  void OnMouseDown() {
    SceneManager.LoadScene("SampleScene");
  }

}
