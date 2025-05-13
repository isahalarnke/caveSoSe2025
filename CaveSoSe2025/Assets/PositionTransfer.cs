using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTransfer : MonoBehaviour
{
    private GameObject actor;
    private Transform head;
    private Transform neck;
    private Transform hipLeft;
    private Transform hipRight;
    private Transform spineBase;
    private Transform spineMid;
    private Transform shoulderLeft;
    private Transform shoulderRight;
    private Transform elbowLeft;
    private Transform elbowRight;
    private Transform handLeft;
    private Transform handRight;
    private Transform kneeLeft;
    private Transform kneeRight;
    private Transform ankleLeft;
    private Transform ankleRight;
    private Transform footLeft;
    private Transform footRight;

    // Start is called before the first frame update
    void Start()
    {
        // In most cases this doesn't work because the TestActor is instantiated after Start() method is called

        actor = GameObject.FindGameObjectWithTag("Player");
        if (actor != null)
        {
            Debug.Log("Player Position beim Start: " + actor.transform.position);
        }
        
    }

    // Finding the instantiated TestActor 2 Object with the Tag "Player" and transfer the joint positions to cubes, that are instantiated. --> special features for a certain body part in planning
    void Update()
    {
       
        if (actor == null)
        {
            actor = GameObject.FindGameObjectWithTag("Player");
            if (actor != null)
            {
                Debug.Log("Player gefunden und Position: " + actor.transform.position);

                // Finding the positions of the TestActot joints

                head = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Neck/Head");
                neck = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Neck");
                spineBase = actor.transform.Find("Spine Base");
                spineMid = actor.transform.Find("Spine Base/Spine Mid");
                hipRight = actor.transform.Find("Spine Base/Hip Right");
                hipLeft = actor.transform.Find("Spine Base/Hip Left");
                kneeLeft = actor.transform.Find("Spine Base/Hip Left/Knee Left");
                kneeRight = actor.transform.Find("Spine Base/Hip Right/Knee Right");
                ankleLeft = actor.transform.Find("Spine Base/Hip Left/Knee Left/Ankle Left");
                ankleRight = actor.transform.Find("Spine Base/Hip Right/Knee Right/Ankle Right");
                footLeft = actor.transform.Find("Spine Base/Hip Left/Knee Left/Ankle Left/Foot Left");
                footRight = actor.transform.Find("Spine Base/Hip Right/Knee Right/Ankle Right/Foot Right");
                shoulderLeft = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Left");
                shoulderRight = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Right");
                elbowLeft = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Left/Elbow Left");
                elbowRight = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Right/Elbow Right");
                handLeft = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Left/Elbow Left/Wrist Left/Hand Left");
                handRight = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Right/Elbow Right/Wrist Right/Hand Right");

                // Creating cubes with offset to visualize a character

                if (head != null) CreateCubeAtPosition(head.position, "HeadCube", head);
                if (neck != null) CreateCubeAtPosition(neck.position, "NeckCube", neck);
                if (spineBase != null) CreateCubeAtPosition(spineBase.position, "SpineBaseCube", spineBase);
                if (hipLeft != null) CreateCubeAtPosition(hipLeft.position, "HipLeftCube", hipLeft);
                if (hipRight != null) CreateCubeAtPosition(hipRight.position, "HipRightCube", hipRight);
                if (kneeLeft != null) CreateCubeAtPosition(kneeLeft.position, "KneeLeftCube", kneeLeft);
                if (kneeRight != null) CreateCubeAtPosition(kneeRight.position, "KneeRightCube", kneeRight);
                if (ankleLeft != null) CreateCubeAtPosition(ankleLeft.position, "AnkleLeftCube", ankleLeft);
                if (ankleRight != null) CreateCubeAtPosition(ankleRight.position, "AnkleRightCube", ankleRight);
                if (footLeft != null) CreateCubeAtPosition(footLeft.position, "FootLeftCube", footLeft);
                if (footRight != null) CreateCubeAtPosition(footRight.position, "FootRightCube", footRight);
                if (shoulderLeft != null) CreateCubeAtPosition(shoulderLeft.position, "ShoulderLeftCube", shoulderLeft);
                if (shoulderRight != null) CreateCubeAtPosition(shoulderRight.position, "ShoulderRightCube", shoulderRight);
                if (elbowLeft != null) CreateCubeAtPosition(elbowLeft.position, "ElbowLeftCube", elbowLeft);
                if (elbowRight != null) CreateCubeAtPosition(elbowRight.position, "ElbowRightCube", elbowRight);
                if (handLeft != null) CreateCubeAtPosition(handLeft.position, "HandLeftCube", handLeft);
                if (handRight != null) CreateCubeAtPosition(handRight.position, "HandRightCube", handRight);

                //Drawing lines between specific characters

                // head and Upperbody
                if (neck != null && head != null) DrawLineBetweenJoints(neck, head, "Line_Neck_Head");
                if (spineMid != null && neck != null) DrawLineBetweenJoints(spineMid, neck, "Line_SpineMid_Neck");
                if (spineBase != null && spineMid != null) DrawLineBetweenJoints(spineBase, spineMid, "Line_SpineBase_SpineMid");

                // Shoulders and arms
                if (spineMid != null && shoulderLeft != null) DrawLineBetweenJoints(spineMid, shoulderLeft, "Line_SpineMid_ShoulderLeft");
                if (spineMid != null && shoulderRight != null) DrawLineBetweenJoints(spineMid, shoulderRight, "Line_SpineMid_ShoulderRight");

                if (shoulderLeft != null && elbowLeft != null) DrawLineBetweenJoints(shoulderLeft, elbowLeft, "Line_ShoulderLeft_ElbowLeft");
                if (shoulderRight != null && elbowRight != null) DrawLineBetweenJoints(shoulderRight, elbowRight, "Line_ShoulderRight_ElbowRight");

                if (elbowLeft != null && handLeft != null) DrawLineBetweenJoints(elbowLeft, handLeft, "Line_ElbowLeft_HandLeft");
                if (elbowRight != null && handRight != null) DrawLineBetweenJoints(elbowRight, handRight, "Line_ElbowRight_HandRight");

                // Hips, Legs(Knee, Ankle, Feet)
                if (spineBase != null && hipLeft != null) DrawLineBetweenJoints(spineBase, hipLeft, "Line_SpineBase_HipLeft");
                if (spineBase != null && hipRight != null) DrawLineBetweenJoints(spineBase, hipRight, "Line_SpineBase_HipRight");

                if (hipLeft != null && kneeLeft != null) DrawLineBetweenJoints(hipLeft, kneeLeft, "Line_HipLeft_KneeLeft");
                if (hipRight != null && kneeRight != null) DrawLineBetweenJoints(hipRight, kneeRight, "Line_HipRight_KneeRight");

                if (kneeLeft != null && ankleLeft != null) DrawLineBetweenJoints(kneeLeft, ankleLeft, "Line_KneeLeft_AnkleLeft");
                if (kneeRight != null && ankleRight != null) DrawLineBetweenJoints(kneeRight, ankleRight, "Line_KneeRight_AnkleRight");

                if (ankleLeft != null && footLeft != null) DrawLineBetweenJoints(ankleLeft, footLeft, "Line_AnkleLeft_FootLeft");
                if (ankleRight != null && footRight != null) DrawLineBetweenJoints(ankleRight, footRight, "Line_AnkleRight_FootRight");


            }
        }

       
        if (actor != null)
        {
            Vector3 position = actor.transform.position;
            Debug.Log("Aktuelle Position von Player: " + position);

        }
    }

    // Cubed at the position with offset of the TestActor to visualize a character

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

    // Drawing lines with a Line Renderer object between correct joints

    private void DrawLineBetweenJoints(Transform start, Transform end, string lineName)
    {
        GameObject lineObj = new GameObject(lineName);
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, end.position);

        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;

        // Materials 
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
    }
}
