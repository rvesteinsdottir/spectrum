using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Mesh;

public class TriangleManager : MonoBehaviour
{
 
  private GameObject m_goTriangle;

  void Start()
  {
    //GenerateLevel();
    GenerateTriangle();
    //GenerateGrid();
  }

  private void GenerateTriangle()
  {
    m_goTriangle = new GameObject();
     m_goTriangle.AddComponent<MeshFilter>();
    m_goTriangle.AddComponent<MeshRenderer>();

  
    Mesh m_meshTriangle = m_goTriangle.GetComponent<MeshFilter>().mesh;

    m_meshTriangle.Clear();
    // m_meshTriangle.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0.25f, 0), new Vector3(0.25f, 0.25f, 0) };
    // m_meshTriangle.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 0.25f), new Vector2(0.25f, 0.25f) };
    // m_meshTriangle.triangles = new int[] { 0, 1, 2 };
    // m_goTriangle.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default")) {color = Color.red};

    // m_goTriangle.layer = 2;
    // m_meshTriangle.RecalculateNormals ();

    m_meshTriangle.vertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 100, 0), new Vector3(100, 100, 100)};
    m_meshTriangle.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};
    m_meshTriangle.triangles = new int[] { 0,1,2 };
   
  }


  // private void GenerateLevel()
  // {

  // }

 
  // public void GenerateColorArray()
  // {
  //   Color tileWidth = ((endColor - startColor)/(cols * rows));

  //   colorArray = new Color[rows, cols];
  //   int x = 0;

  //   for (int row = 0; row < rows; row++)
  //   {
  //     for (int col = 0; col < cols; col++)
  //     {
  //       colorArray[row,col] = (startColor + (tileWidth * x));
  //       x += 1;
  //     }
  //   }
  // }
}