using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTransfer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject actor = GameObject.FindGameObjectWithTag("TestActor");
        if (!actor)
        {
            Debug.LogError("Testactor nicht gefunden");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
