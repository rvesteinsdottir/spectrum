using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletedBoard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DisplayBoard();
    }

    private void DisplayBoard()
    {
        TestMesh existingBoard = GameObject.Find("MeshParent").GetComponent<TestMesh>();

    }

}
