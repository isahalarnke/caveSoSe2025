using System.Collections.Generic;
using UnityEngine;

public class PositionTransfer : MonoBehaviour
{
    private GameObject actor;
    private Transform head, neck, hipLeft, hipRight, spineBase, spineMid;
    private Transform shoulderLeft, shoulderRight, elbowLeft, elbowRight;
    private Transform handLeft, handRight, kneeLeft, kneeRight;
    private Transform ankleLeft, ankleRight, footLeft, footRight;

    //Dictionariey of the avatar objects and lines that should be updated per frame
    private Dictionary<string, GameObject> jointCubes = new Dictionary<string, GameObject>();
    private Dictionary<string, LineRenderer> jointLines = new Dictionary<string, LineRenderer>();

    private bool jointsInitialized = false;

    void Update()
    {
        // Try to find the actor if not yet found
        if (actor == null)
        {
            actor = GameObject.FindGameObjectWithTag("Player");
            // To update every joint again
            jointsInitialized = false;
            if (actor != null)
                Debug.Log("Player gefunden und Position: " + actor.transform.position);
        }

        // Initialize when needed
        if (actor != null && !jointsInitialized)
        {
            InitializeJoints();
            jointsInitialized = true;
        }

        // Update visual cubes and lines every frame
        if (jointsInitialized)
        {
            UpdateCubesAndLines();
        }
    }

    // Initializing with the correct Filestructure
    void InitializeJoints()
    {
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
    }

    void UpdateCubesAndLines()
    {
        UpdateCube("HeadCube", head);
        UpdateCube("NeckCube", neck);
        UpdateCube("SpineBaseCube", spineBase);
        UpdateCube("SpineMidCube", spineMid);
        UpdateCube("HipLeftCube", hipLeft);
        UpdateCube("HipRightCube", hipRight);
        UpdateCube("KneeLeftCube", kneeLeft);
        UpdateCube("KneeRightCube", kneeRight);
        UpdateCube("AnkleLeftCube", ankleLeft);
        UpdateCube("AnkleRightCube", ankleRight);
        UpdateCube("FootLeftCube", footLeft);
        UpdateCube("FootRightCube", footRight);
        UpdateCube("ShoulderLeftCube", shoulderLeft);
        UpdateCube("ShoulderRightCube", shoulderRight);
        UpdateCube("ElbowLeftCube", elbowLeft);
        UpdateCube("ElbowRightCube", elbowRight);
        UpdateCube("HandLeftCube", handLeft);
        UpdateCube("HandRightCube", handRight);

        UpdateLine("Line_Neck_Head", neck, head);
        UpdateLine("Line_SpineMid_Neck", spineMid, neck);
        UpdateLine("Line_SpineBase_SpineMid", spineBase, spineMid);
        UpdateLine("Line_SpineMid_ShoulderLeft", spineMid, shoulderLeft);
        UpdateLine("Line_SpineMid_ShoulderRight", spineMid, shoulderRight);
        UpdateLine("Line_ShoulderLeft_ElbowLeft", shoulderLeft, elbowLeft);
        UpdateLine("Line_ShoulderRight_ElbowRight", shoulderRight, elbowRight);
        UpdateLine("Line_ElbowLeft_HandLeft", elbowLeft, handLeft);
        UpdateLine("Line_ElbowRight_HandRight", elbowRight, handRight);
        UpdateLine("Line_SpineBase_HipLeft", spineBase, hipLeft);
        UpdateLine("Line_SpineBase_HipRight", spineBase, hipRight);
        UpdateLine("Line_HipLeft_KneeLeft", hipLeft, kneeLeft);
        UpdateLine("Line_HipRight_KneeRight", hipRight, kneeRight);
        UpdateLine("Line_KneeLeft_AnkleLeft", kneeLeft, ankleLeft);
        UpdateLine("Line_KneeRight_AnkleRight", kneeRight, ankleRight);
        UpdateLine("Line_AnkleLeft_FootLeft", ankleLeft, footLeft);
        UpdateLine("Line_AnkleRight_FootRight", ankleRight, footRight);
    }

    // Updates or creates cubes 
    void UpdateCube(string name, Transform joint)
    {
        if (joint == null) return;

        if (!jointCubes.ContainsKey(name))
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Renderer renderer = cube.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.41f, 0.71f);
            jointCubes[name] = cube;
        }

        // Offset z-axis for teh avatar visualization
        Vector3 offsetPosition = joint.position + new Vector3(0, 0, 1.3f);
        jointCubes[name].transform.position = offsetPosition;
    }

    //Visualization between joints, updates start and end position of line or instatiated depending if actor or joints and lines are already instantiated
    void UpdateLine(string name, Transform jointA, Transform jointB)
    {
        if (jointA == null || jointB == null) return;

        if (!jointLines.ContainsKey(name))
        {
            GameObject lineObj = new GameObject(name);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red;
            lr.endColor = Color.green;
            lr.startWidth = 0.02f;
            lr.endWidth = 0.02f;
            lr.positionCount = 2;
            jointLines[name] = lr;
        }

        Vector3 posA = jointA.position + new Vector3(0, 0, 1.3f);
        Vector3 posB = jointB.position + new Vector3(0, 0, 1.3f);

        jointLines[name].SetPosition(0, posA);
        jointLines[name].SetPosition(1, posB);
    }
}
