using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PositionTransfer : MonoBehaviour
{
    private GameObject actor;
    private GameObject avatarRoot;
    private Transform head, neck, hipLeft, hipRight, spineBase, spineMid;
    private Transform shoulderLeft, shoulderRight, elbowLeft, elbowRight;
    private Transform handLeft, handRight, kneeLeft, kneeRight;
    private Transform ankleLeft, ankleRight, footLeft, footRight;

    public Vector3 mirrorPlanePoint = new Vector3(0, 0, 0);
    public Vector3 mirrorNormal = Vector3.forward;
    public GameObject handPrefab;
    public GameObject headPrefab;
    public GameObject bodyPrefab;

    public ParticleSystem particlePrefab;
    public AudioClip collisionSound;

    // Dictionary of the avatar objects and lines that should be updated per frame
    public Dictionary<string, GameObject> jointCubes = new Dictionary<string, GameObject>();
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
                Debug.Log("Player found at position: " + actor.transform.position);
        }

        // Initialize when needed
        if (actor != null && !jointsInitialized)
        {
            InitializeJoints();
            jointsInitialized = true;
            if (avatarRoot == null)
            {
                avatarRoot = new GameObject("Avatar");
            }
        }

        // Update visual cubes and lines every frame
        if (jointsInitialized)
        {
            UpdateBodypartsAndLines();
        }


    }

    // Initializing with the correct path
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

    void UpdateBodypartsAndLines()
    {
        UpdateBodypart("Head", head);
        UpdateBodypart("Neck", neck);
        UpdateBodypart("SpineBase", spineBase);
        UpdateBodypart("SpineMid", spineMid);
        UpdateBodypart("HipLeft", hipLeft);
        UpdateBodypart("HipRight", hipRight);
        UpdateBodypart("KneeLeft", kneeLeft);
        UpdateBodypart("KneeRight", kneeRight);
        UpdateBodypart("AnkleLeft", ankleLeft);
        UpdateBodypart("AnkleRight", ankleRight);
        UpdateBodypart("FootLeft", footLeft);
        UpdateBodypart("FootRight", footRight);
        UpdateBodypart("ShoulderLeft", shoulderLeft);
        UpdateBodypart("ShoulderRight", shoulderRight);
        UpdateBodypart("ElbowLeft", elbowLeft);
        UpdateBodypart("ElbowRight", elbowRight);
        UpdateBodypart("HandLeft", handLeft);
        UpdateBodypart("HandRight", handRight);

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

    // Updates or instantiates bodyparts
    void UpdateBodypart(string name, Transform joint)
    {
        if (joint == null) return;

        if (!jointCubes.ContainsKey(name))
        {
            GameObject bodypart;

            if (name == "HandLeft" || name == "HandRight")
            {
                bodypart = Instantiate(handPrefab);
            }
            else if (name == "Head")
            {
                bodypart = Instantiate(headPrefab);
            }
            else
            {
                bodypart = Instantiate(bodyPrefab);
            }

            bodypart.name = name;
            if (bodypart.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = bodypart.AddComponent<Rigidbody>();
            }
            if (bodypart.GetComponent<Collider>() == null)
            {
                bodypart.AddComponent<BoxCollider>();
            }

            BodyCollision bodyCollision = bodypart.AddComponent<BodyCollision>();
            if (particlePrefab != null)
            {
                bodyCollision.SetParticleEffect(particlePrefab);
                bodyCollision.SetCollisionSound(collisionSound);
            }
            bodypart.transform.parent = avatarRoot.transform;
            jointCubes[name] = bodypart;
        }

        Vector3 mirroredPosition = MirrorJoint(joint.position, mirrorPlanePoint, mirrorNormal);
        jointCubes[name].transform.position = mirroredPosition;

    }

    // Visualization between joints, updates start and end position of line or instatiated depending if actor or joints and lines are already instantiated
    void UpdateLine(string name, Transform jointA, Transform jointB)
    {
        if (jointA == null || jointB == null) return;

        if (!jointLines.ContainsKey(name))
        {
            GameObject lineObj = new GameObject(name);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            Color silver = new Color(0.75f, 0.75f, 0.75f); // RGB für silber/grau
            lr.startColor = silver;
            lr.endColor = silver;
            lr.startWidth = 0.02f;
            lr.endWidth = 0.02f;
            lr.positionCount = 2;
            jointLines[name] = lr;
        }
        Vector3 mirroredA = MirrorJoint(jointA.position, mirrorPlanePoint, mirrorNormal);
        Vector3 mirroredB = MirrorJoint(jointB.position, mirrorPlanePoint, mirrorNormal);
        jointLines[name].transform.parent = avatarRoot.transform;
        jointLines[name].SetPosition(0, mirroredA);
        jointLines[name].SetPosition(1, mirroredB);
    }

    // Mirrors a given point i relation to given plane and its normal
    Vector3 MirrorJoint(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
    {
        Vector3 n = planeNormal.normalized;
        Vector3 toPoint = point - planePoint;
        float projection = Vector3.Dot(toPoint, n);
        Vector3 mirrored = point - 2 * projection * n;
        return mirrored;
    }
}
