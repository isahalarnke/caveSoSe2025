using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Kollision erkannt");
    }
}
