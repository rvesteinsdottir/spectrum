using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDrop : MonoBehaviour
{
  void onTriggerEnter(Collider col)
  {
    Debug.Log("Object triggered");
  }
}
