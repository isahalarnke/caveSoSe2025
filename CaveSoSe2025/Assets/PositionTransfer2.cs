using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PositionTransfer2 : MonoBehaviour
{
    private Dictionary<string, GameObject> actors = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> avatarRoots = new Dictionary<string, GameObject>();
    private Dictionary<string, Dictionary<string, GameObject>> jointCubes = new Dictionary<string, Dictionary<string, GameObject>>();
    private Dictionary<string, Dictionary<string, LineRenderer>> jointLines = new Dictionary<string, Dictionary<string, LineRenderer>>();
    private HashSet<string> initializedActors = new HashSet<string>();

    public Vector3 mirrorPlanePoint = new Vector3(0, 0, 0);
    public Vector3 mirrorNormal = Vector3.forward;
    public GameObject handPrefab;
    public GameObject headPrefab;
    public GameObject bodyPrefab;

    public ParticleSystem particlePrefab;
    public AudioClip collisionSound;


    void Update()
    {
        GameObject[] foundActors = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject actor in foundActors)
        {
            if (actor == null || !actor.name.StartsWith("Kinect Actor #"))
                continue;

            string actorId = actor.name;

            // Register new actor
            if (!actors.ContainsKey(actorId))
            {
                actors[actorId] = actor;
                initializedActors.Remove(actorId);
                Debug.Log("Actor gefunden: " + actorId);
            }

            if (!initializedActors.Contains(actorId))
            {
                InitializeJoints(actorId, actor);
                initializedActors.Add(actorId);
            }

            // Update for specific avatar
            UpdateBodypartsAndLines(actorId);
        }

        // If actor not in the scene or tracked anymore, destroy the avatar
        HashSet<string> currentActorIds = new HashSet<string>();

        foreach (GameObject actor in foundActors)
        {
            if (actor != null && actor.name.StartsWith("Kinect Actor #"))
            {
                string actorId = actor.name;
                currentActorIds.Add(actorId);
            }
        }
        List<string> knownActorIds = new List<string>(actors.Keys);
        foreach (string actorId in knownActorIds)
        {
            if (!currentActorIds.Contains(actorId))
            {
                Debug.Log("Actor removed: " + actorId);

                if (avatarRoots.ContainsKey(actorId))
                {
                    Destroy(avatarRoots[actorId]);
                    avatarRoots.Remove(actorId);
                }

                // Remove from all dictionaries
                actors.Remove(actorId);
                jointCubes.Remove(actorId);
                jointLines.Remove(actorId);
                initializedActors.Remove(actorId);
            }
        }
    }

    void InitializeJoints(string actorId, GameObject actor)
    {
        Dictionary<string, Transform> joints = new Dictionary<string, Transform>
        {
            ["Head"] = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Neck/Head"),
            ["Neck"] = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Neck"),
            ["SpineBase"] = actor.transform.Find("Spine Base"),
            ["SpineMid"] = actor.transform.Find("Spine Base/Spine Mid"),
            ["HipLeft"] = actor.transform.Find("Spine Base/Hip Left"),
            ["HipRight"] = actor.transform.Find("Spine Base/Hip Right"),
            ["KneeLeft"] = actor.transform.Find("Spine Base/Hip Left/Knee Left"),
            ["KneeRight"] = actor.transform.Find("Spine Base/Hip Right/Knee Right"),
            ["AnkleLeft"] = actor.transform.Find("Spine Base/Hip Left/Knee Left/Ankle Left"),
            ["AnkleRight"] = actor.transform.Find("Spine Base/Hip Right/Knee Right/Ankle Right"),
            ["FootLeft"] = actor.transform.Find("Spine Base/Hip Left/Knee Left/Ankle Left/Foot Left"),
            ["FootRight"] = actor.transform.Find("Spine Base/Hip Right/Knee Right/Ankle Right/Foot Right"),
            ["ShoulderLeft"] = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Left"),
            ["ShoulderRight"] = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Right"),
            ["ElbowLeft"] = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Left/Elbow Left"),
            ["ElbowRight"] = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Right/Elbow Right"),
            ["HandLeft"] = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Left/Elbow Left/Wrist Left/Hand Left"),
            ["HandRight"] = actor.transform.Find("Spine Base/Spine Mid/Spine Shoulder/Shoulder Right/Elbow Right/Wrist Right/Hand Right"),
        };

        jointCubes[actorId] = new Dictionary<string, GameObject>();
        jointLines[actorId] = new Dictionary<string, LineRenderer>();

        GameObject avatarRoot = new GameObject("Avatar_" + actorId);
        avatarRoots[actorId] = avatarRoot;

        foreach (var kvp in joints)
        {
            UpdateBodypart(actorId, kvp.Key, kvp.Value);
        }
    }


    void UpdateBodypartsAndLines(string actorId)
    {
        GameObject actor = actors[actorId];

        Transform Get(string path) => actor.transform.Find(path);

        Dictionary<string, Transform> joints = new Dictionary<string, Transform>
        {
            ["Head"] = Get("Spine Base/Spine Mid/Spine Shoulder/Neck/Head"),
            ["Neck"] = Get("Spine Base/Spine Mid/Spine Shoulder/Neck"),
            ["SpineBase"] = Get("Spine Base"),
            ["SpineMid"] = Get("Spine Base/Spine Mid"),
            ["HipLeft"] = Get("Spine Base/Hip Left"),
            ["HipRight"] = Get("Spine Base/Hip Right"),
            ["KneeLeft"] = Get("Spine Base/Hip Left/Knee Left"),
            ["KneeRight"] = Get("Spine Base/Hip Right/Knee Right"),
            ["AnkleLeft"] = Get("Spine Base/Hip Left/Knee Left/Ankle Left"),
            ["AnkleRight"] = Get("Spine Base/Hip Right/Knee Right/Ankle Right"),
            ["FootLeft"] = Get("Spine Base/Hip Left/Knee Left/Ankle Left/Foot Left"),
            ["FootRight"] = Get("Spine Base/Hip Right/Knee Right/Ankle Right/Foot Right"),
            ["ShoulderLeft"] = Get("Spine Base/Spine Mid/Spine Shoulder/Shoulder Left"),
            ["ShoulderRight"] = Get("Spine Base/Spine Mid/Spine Shoulder/Shoulder Right"),
            ["ElbowLeft"] = Get("Spine Base/Spine Mid/Spine Shoulder/Shoulder Left/Elbow Left"),
            ["ElbowRight"] = Get("Spine Base/Spine Mid/Spine Shoulder/Shoulder Right/Elbow Right"),
            ["HandLeft"] = Get("Spine Base/Spine Mid/Spine Shoulder/Shoulder Left/Elbow Left/Wrist Left/Hand Left"),
            ["HandRight"] = Get("Spine Base/Spine Mid/Spine Shoulder/Shoulder Right/Elbow Right/Wrist Right/Hand Right")
        };

        foreach (var kvp in joints)
        {
            UpdateBodypart(actorId, kvp.Key, kvp.Value);
        }

        // Update lines
        UpdateLine(actorId, "Line_Neck_Head", joints["Neck"], joints["Head"]);
        UpdateLine(actorId, "Line_SpineMid_Neck", joints["SpineMid"], joints["Neck"]);
        UpdateLine(actorId, "Line_SpineBase_SpineMid", joints["SpineBase"], joints["SpineMid"]);
        UpdateLine(actorId, "Line_SpineMid_ShoulderLeft", joints["SpineMid"], joints["ShoulderLeft"]);
        UpdateLine(actorId, "Line_SpineMid_ShoulderRight", joints["SpineMid"], joints["ShoulderRight"]);
        UpdateLine(actorId, "Line_ShoulderLeft_ElbowLeft", joints["ShoulderLeft"], joints["ElbowLeft"]);
        UpdateLine(actorId, "Line_ShoulderRight_ElbowRight", joints["ShoulderRight"], joints["ElbowRight"]);
        UpdateLine(actorId, "Line_ElbowLeft_HandLeft", joints["ElbowLeft"], joints["HandLeft"]);
        UpdateLine(actorId, "Line_ElbowRight_HandRight", joints["ElbowRight"], joints["HandRight"]);
        UpdateLine(actorId, "Line_SpineBase_HipLeft", joints["SpineBase"], joints["HipLeft"]);
        UpdateLine(actorId, "Line_SpineBase_HipRight", joints["SpineBase"], joints["HipRight"]);
        UpdateLine(actorId, "Line_HipLeft_KneeLeft", joints["HipLeft"], joints["KneeLeft"]);
        UpdateLine(actorId, "Line_HipRight_KneeRight", joints["HipRight"], joints["KneeRight"]);
        UpdateLine(actorId, "Line_KneeLeft_AnkleLeft", joints["KneeLeft"], joints["AnkleLeft"]);
        UpdateLine(actorId, "Line_KneeRight_AnkleRight", joints["KneeRight"], joints["AnkleRight"]);
        UpdateLine(actorId, "Line_AnkleLeft_FootLeft", joints["AnkleLeft"], joints["FootLeft"]);
        UpdateLine(actorId, "Line_AnkleRight_FootRight", joints["AnkleRight"], joints["FootRight"]);
    }


    // Updates or instantiates bodyparts
    void UpdateBodypart(string actorId, string name, Transform joint)
    {
        if (joint == null) return;

        var cubes = jointCubes[actorId];
        if (!cubes.ContainsKey(name))
        {
            GameObject bodypart;

            if (name == "HandLeft" || name == "HandRight")
                bodypart = Instantiate(handPrefab);
            else if (name == "Head")
                bodypart = Instantiate(headPrefab);
            else
                bodypart = Instantiate(bodyPrefab);

            bodypart.name = name;
            bodypart.transform.parent = avatarRoots[actorId].transform;

            if (bodypart.GetComponent<Rigidbody>() == null)
                bodypart.AddComponent<Rigidbody>();
            if (bodypart.GetComponent<Collider>() == null)
                bodypart.AddComponent<BoxCollider>();

            BodyCollision bodyCollision = bodypart.AddComponent<BodyCollision>();
            if (particlePrefab != null)
            {
                bodyCollision.SetParticleEffect(particlePrefab);
                bodyCollision.SetCollisionSound(collisionSound);
            }

            cubes[name] = bodypart;
        }

        Vector3 mirroredPosition = MirrorJoint(joint.position, mirrorPlanePoint, mirrorNormal);
        cubes[name].transform.position = mirroredPosition;
    }


    // Visualization between joints, updates start and end position of line or instatiated depending if actor or joints and lines are already instantiated
    void UpdateLine(string actorId, string name, Transform jointA, Transform jointB)
    {
        if (jointA == null || jointB == null) return;

        var lines = jointLines[actorId];
        if (!lines.ContainsKey(name))
        {
            GameObject lineObj = new GameObject(name);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();

            lineObj.transform.parent = avatarRoots[actorId].transform;

            lr.material = new Material(Shader.Find("Sprites/Default"));
            Color silver = new Color(0.75f, 0.75f, 0.75f);
            lr.startColor = silver;
            lr.endColor = silver;
            lr.startWidth = 0.02f;
            lr.endWidth = 0.02f;
            lr.positionCount = 2;
            lines[name] = lr;
        }
        Vector3 mirroredA = MirrorJoint(jointA.position, mirrorPlanePoint, mirrorNormal);
        Vector3 mirroredB = MirrorJoint(jointB.position, mirrorPlanePoint, mirrorNormal);
        lines[name].SetPosition(0, mirroredA);
        lines[name].SetPosition(1, mirroredB);
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
