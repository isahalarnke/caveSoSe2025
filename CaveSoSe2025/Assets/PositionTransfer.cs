using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTransfer : MonoBehaviour
{
    private GameObject actor;

    // Start is called before the first frame update
    void Start()
    {
        
        actor = GameObject.FindGameObjectWithTag("Player");
        if (actor != null)
        {
            Debug.Log("Player Position beim Start: " + actor.transform.position);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
       
        if (actor == null)
        {
            actor = GameObject.FindGameObjectWithTag("Player");
            if (actor != null)
            {
                Debug.Log("Player gefunden und Position: " + actor.transform.position);
            }
        }

       
        if (actor != null)
        {
            Vector3 position = actor.transform.position;
            Debug.Log("Aktuelle Position von Player: " + position);
        }
    }
}
