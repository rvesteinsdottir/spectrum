using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    public static int scoreValue;
    Text score;

    void Start()
    {
        scoreValue = PlayerPrefs.GetInt("Score", 0);
        score = GetComponent<Text>();
    }

    void Update()
    {
        score.text = $"{scoreValue}";
    }
}
