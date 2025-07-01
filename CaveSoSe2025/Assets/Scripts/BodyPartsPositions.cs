using System.Collections.Generic;
using UnityEngine;

public class BodyPartsPositions : MonoBehaviour
{
    private GameObject actor;
    private Transform head, neck, hipLeft, hipRight, spineBase, spineMid;
    private Transform shoulderLeft, shoulderRight, elbowLeft, elbowRight;
    private Transform wristLeft, wirstRight, handLeft, handRight, kneeLeft, kneeRight;
    private Transform ankleLeft, ankleRight, footLeft, footRight;


    public Vector3 mirrorPlanePoint = new Vector3(0, 0, 0);
    public Vector3 mirrorNormal = Vector3.forward;

    //Prefabs to be used
    public GameObject handLeftPrefab, handRightPrefab, forearmLeftPrefab, forearmRightPrefab, upperArmLeftPrefab, upperarmRightPrefab;
    public GameObject headprefab;
    public GameObject upperbodyPrefab;
    public GameObject lowerlegLeftPrefab, lowerlegRightPrefab;
    public GameObject thighRightPrefab, thighLeftPrefab, footRightPrefab, footLeftPrefab;

    //Dictionariey of the avatar objects that should be updated per frame
    private Dictionary<string, GameObject> bodyparts = new Dictionary<string, GameObject>();

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
            UpdateBodyPartsPosition();
        }
    }

    // Initializing position with the correct Filestructure
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
        wirstRight = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Right/Elbow Right/Wrist Right");
        wristLeft = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Left/Elbow Left/Wrist Left");
    }

    void UpdateBodyPartsPosition()
    {
        UpdateBodyPart("Head", head, headprefab);
        UpdateBodyPart("SpineMid", spineMid, upperbodyPrefab);
        UpdateBodyPart("HandLeft", handLeft, handLeftPrefab);
        UpdateBodyPart("HandRight", handRight, handRightPrefab);
        UpdateBodyPart("FootLeft", footLeft, footLeftPrefab);
        UpdateBodyPart("FootRight", footRight, footRightPrefab);

        UpdateBodyPartBetweenJoints("ForearmLeft", elbowLeft, handLeft, forearmLeftPrefab);
        UpdateBodyPartBetweenJoints("ForearmRight", elbowRight, handRight, forearmRightPrefab);
        UpdateBodyPartBetweenJoints("UpperArmLeft", shoulderLeft, elbowLeft, upperArmLeftPrefab);
        UpdateBodyPartBetweenJoints("UpperArmRight", shoulderRight, elbowRight, upperarmRightPrefab);
        UpdateBodyPartBetweenJoints("ThighLeft", hipLeft, kneeLeft, thighLeftPrefab);
        UpdateBodyPartBetweenJoints("ThighRight", hipRight, kneeRight, thighRightPrefab);
        UpdateBodyPartBetweenJoints("LowerLegLeft", kneeLeft, ankleLeft, lowerlegLeftPrefab);
        UpdateBodyPartBetweenJoints("LowerLegRight", kneeRight, ankleRight, lowerlegRightPrefab);
    }

    // Updates or instantiates Bodypart prefab. Name is required to be the key in the dictionary bodyparts.

    void UpdateBodyPart(string name, Transform joint, GameObject prefab)
    {
        if (joint == null || prefab == null) return;

        if (!bodyparts.ContainsKey(name))
        {
            GameObject bodyPart = Instantiate(prefab);
            bodyPart.name = name;
            bodyPart.AddComponent<BodyCollision>();
            bodyparts[name] = bodyPart;
        }

        Vector3 mirroredPosition = MirrorJoint(joint.position, mirrorPlanePoint, mirrorNormal);
        bodyparts[name].transform.position = mirroredPosition;
    }

    //For the Prefabs which are in the middle position between the joints, needed to be tested whether prefab is really in the middle of the two joints.

    void UpdateBodyPartBetweenJoints(string name, Transform jointA, Transform jointB, GameObject prefab)
    {
        if (jointA == null || jointB == null) return;

        if (!bodyparts.ContainsKey(name))
        {
            GameObject bodyPart = Instantiate(prefab);
            bodyPart.name = name;
            bodyPart.AddComponent<BodyCollision>();
            bodyparts[name] = bodyPart;
        }

        GameObject part = bodyparts[name];

        Vector3 posA = jointA.position;
        Vector3 posB = jointB.position;

        // Center point between the joints
        Vector3 centerPos = (posA + posB) / 2f;

        // Rotation
        Vector3 direction = (posB - posA).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        // Modification of length of the prefab (need to test if this is really necessary)
        float length = Vector3.Distance(posA, posB);
        Vector3 originalScale = prefab.transform.localScale;
        Vector3 newScale = new Vector3(originalScale.x, originalScale.y, length);

        part.transform.position = MirrorJoint(centerPos, mirrorPlanePoint, mirrorNormal);
        part.transform.rotation = rotation;
        //part.transform.localScale = newScale;
    }


   
    //Mirror at set mirrorPoint

    Vector3 MirrorJoint(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
    {
        Vector3 n = planeNormal.normalized;
        Vector3 toPoint = point - planePoint;
        float projection = Vector3.Dot(toPoint, n);
        Vector3 mirrored = point - 2 * projection * n;
        return mirrored;
    }

}