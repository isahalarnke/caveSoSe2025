using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTransfer : MonoBehaviour
{
    private GameObject actor;
    private Transform head;
    private Transform hipLeft;
    private Transform hipRight;
    private Transform spineBase;

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


                head = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Neck/Head");
                hipRight = actor.transform.Find("Spine Base/Hip Right");
                hipLeft = actor.transform.Find("Spine Base/Hip Left");
                spineBase = actor.transform.Find("Spine Base");

                if (head != null)
                {
                    CreateCubeAtPosition(head.position, "HeadCube", head);
                }

                if (hipRight != null)
                {
                    CreateCubeAtPosition(hipRight.position, "HipRightCube", hipRight);
                }

                if (hipLeft != null)
                {
                    CreateCubeAtPosition(hipLeft.position, "HipLeftCube", hipLeft);
                }

                if (spineBase != null)
                {
                    CreateCubeAtPosition(spineBase.position, "SpineBaseCube", spineBase);
                }
            }
        }

       
        if (actor != null)
        {
            Vector3 position = actor.transform.position;
            Debug.Log("Aktuelle Position von Player: " + position);

        }
    }

    // Kreiere Würfel zum Testen an den Positionen um zu Testen, ob die Positionen tracken 

    private void CreateCubeAtPosition(Vector3 position, string cubeName, Transform parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        position.z += 1.3f;

        cube.transform.position = position;

        cube.transform.SetParent(parent);

        cube.name = cubeName;

        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        Renderer cubeRenderer = cube.GetComponent<Renderer>();

        cubeRenderer.material.color = new Color(1f, 0.41f, 0.71f);
    }
}
