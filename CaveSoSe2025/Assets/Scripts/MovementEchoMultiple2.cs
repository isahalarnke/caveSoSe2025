using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
/// Records all movements of the mirrored avatars and creates echos within a set recording duration
public class MovementEchoMultiple2 : MonoBehaviour
{

    public PositionTransferMultiple tracker; // Takes positions of the mirrored avatars
    public float recordingDuration;
    public float fps; // The smaller the fps the slower the echos are moving
    public AudioClip recordSound;
    public BoxCollider spawnArea;

    private Dictionary<string, List<Dictionary<string, Vector3>>> recordedFrames = new();
    private Dictionary<string, List<List<Dictionary<string, Vector3>>>> allRecordedFrames = new();
    private Dictionary<string, int> echoIndices = new();

    private Dictionary<string, List<Vector3>> echoOffsets = new();
    private Dictionary<string, List<List<(Transform from, Transform to)>>> allLineEndpoints = new();
    private Dictionary<string, List<List<LineRenderer>>> allLineRenderers = new();

    private bool isRecording = false;
    private float timer = 0f;

    public ParticleSystem particlePrefab;
    public AudioClip collisionSound;

    private List<(string from, string to)> boneConnections = new List<(string, string)> {
        ("Head", "Neck"),
        ("Neck", "SpineMid"),
        ("SpineMid", "SpineBase"),
        ("SpineMid", "ShoulderLeft"),
        ("SpineMid", "ShoulderRight"),
        ("SpineBase", "HipLeft"),
        ("SpineBase", "HipRight"),
        ("ShoulderLeft", "ElbowLeft"),
        ("ElbowLeft", "HandLeft"),
        ("ShoulderRight", "ElbowRight"),
        ("ElbowRight", "HandRight"),
        ("HipLeft", "KneeLeft"),
        ("KneeLeft", "AnkleLeft"),
        ("AnkleLeft", "FootLeft"),
        ("HipRight", "KneeRight"),
        ("KneeRight", "AnkleRight"),
        ("AnkleRight", "FootRight"),
    };

    void Start()
    {
        Invoke("StartRecording", 10f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T key recording starting");
            StartRecording();
        }

        if (isRecording)
        {
            timer += Time.deltaTime;
            RecordFrame();

            if (timer >= recordingDuration)
            {
                isRecording = false;
                StartCoroutine(HandleReplayAndNextRecording());
            }
        }
        UpdateLineRenderer();
    }
 
    public void StartRecording()
    {
        recordedFrames.Clear();
        timer = 0f;
        isRecording = true;
        Debug.Log("Recording started.");

        if (recordSound == null) return;

        GameObject soundObj = new GameObject("RecordSound");
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip = recordSound;
        audioSource.Play();
        Destroy(soundObj, recordSound.length + 0.1f);
    }

    void RecordFrame()
    {
        foreach (var actorId in tracker.joints.Keys)
        {
            if (!recordedFrames.ContainsKey(actorId))
                recordedFrames[actorId] = new();

            Dictionary<string, Vector3> frame = new();

            foreach (var joint in tracker.joints[actorId])
            {
                frame[joint.Key] = joint.Value.transform.position;
            }

            recordedFrames[actorId].Add(frame);
        }
    }

    IEnumerator HandleReplayAndNextRecording()
    {
        foreach (var actorId in recordedFrames.Keys)
        {
            if (!allRecordedFrames.ContainsKey(actorId))
                allRecordedFrames[actorId] = new();
            if (!echoIndices.ContainsKey(actorId))
                echoIndices[actorId] = 0;
            if (!echoOffsets.ContainsKey(actorId))
                echoOffsets[actorId] = new();
            if (!allLineEndpoints.ContainsKey(actorId))
                allLineEndpoints[actorId] = new();
            if (!allLineRenderers.ContainsKey(actorId))
                allLineRenderers[actorId] = new();

            var frames = recordedFrames[actorId];
            allRecordedFrames[actorId].Add(frames);

            var echo = CreateEcho(actorId, echoIndices[actorId], frames);
            StartCoroutine(ReplayEcho(actorId, echo, frames, echoIndices[actorId]));

            echoIndices[actorId]++;
        }

        yield return new WaitForSeconds(1f);
        StartRecording();
    }

    Dictionary<string, GameObject> CreateEcho(string actorId, int index, List<Dictionary<string, Vector3>> frames)
    {
        GameObject echoParent = new GameObject($"Echo_{actorId}_{index}");
        Vector3 offset = GetRandomPositionInBox(spawnArea);
        echoOffsets[actorId].Add(offset);

        Dictionary<string, GameObject> echoDict = new();
        var baseFrame = frames[0];

        List<(Transform, Transform)> echoLineEndpoints = new();
        List<LineRenderer> echoLines = new();

        foreach (var pair in baseFrame)
        {
            GameObject original = tracker.joints[actorId][pair.Key];
            GameObject echo = Instantiate(original, pair.Value + offset, Quaternion.identity);

            BodyCollision bodyCollision = echo.GetComponent<BodyCollision>();
            if (bodyCollision == null)
            {
                bodyCollision = echo.AddComponent<BodyCollision>();
            }
            if (tracker.particlePrefab != null)
            {
                bodyCollision.SetParticleEffect(particlePrefab);
                bodyCollision.SetCollisionSound(collisionSound);
            }

            echo.name = $"{pair.Key}_Echo_{actorId}_{index}";
            echo.transform.SetParent(echoParent.transform);
            echoDict[pair.Key] = echo;
        }

        foreach (var (from, to) in boneConnections)
        {
            if (echoDict.ContainsKey(from) && echoDict.ContainsKey(to))
            {
                var fromTf = echoDict[from].transform;
                var toTf = echoDict[to].transform;
                var lr = CreateEchoLine($"{from}_{to}_Line_{actorId}_{index}", fromTf, toTf, echoParent.transform);
                echoLines.Add(lr);
                echoLineEndpoints.Add((fromTf, toTf));
            }
        }

        allLineEndpoints[actorId].Add(echoLineEndpoints);
        allLineRenderers[actorId].Add(echoLines);

        return echoDict;
    }

    IEnumerator ReplayEcho(string actorId, Dictionary<string, GameObject> echoDict, List<Dictionary<string, Vector3>> frames, int echoIdx)
    {
        int localReplayIndex = 0;
        Vector3 offset = echoOffsets[actorId][echoIdx];

        while (true)
        {
            var frame = frames[localReplayIndex];

            foreach (var pair in frame)
            {
                if (echoDict.TryGetValue(pair.Key, out var echoObj))
                {
                    echoObj.transform.position = pair.Value + offset;
                }
            }

            localReplayIndex = (localReplayIndex + 1) % frames.Count;
            yield return new WaitForSeconds(1f / fps);
        }
    }

    Vector3 GetRandomPositionInBox(BoxCollider box)
    {
        Vector3 center = box.transform.position;
        Vector3 size = box.size;
        Vector3 scale = box.transform.lossyScale;

        Vector3 randomPosition = new Vector3(
            Random.Range(-size.x * 0.5f * scale.x, size.x * 0.5f * scale.x),
            -1f,
            Random.Range(-size.z * 0.5f * scale.z, size.z * 0.5f * scale.z)
        );

        return center + randomPosition;
    }

    LineRenderer CreateEchoLine(string name, Transform from, Transform to, Transform parent)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(parent);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        Color ghostColor = new Color(0.5f, 0.9f, 1f, 0.6f);
        lr.startColor = ghostColor;
        lr.endColor = ghostColor;
        lr.startWidth = 0.015f;
        lr.endWidth = 0.015f;
        lr.positionCount = 2;

        return lr;
    }

    void UpdateLineRenderer()
    {
        foreach (var actorId in allLineRenderers.Keys)
        {
            for (int i = 0; i < allLineRenderers[actorId].Count; i++)
            {
                var lineRenderers = allLineRenderers[actorId][i];
                var endpoints = allLineEndpoints[actorId][i];

                for (int j = 0; j < lineRenderers.Count; j++)
                {
                    var lr = lineRenderers[j];
                    var (from, to) = endpoints[j];

                    if (from != null && to != null)
                    {
                        lr.SetPosition(0, from.position);
                        lr.SetPosition(1, to.position);
                    }
                }
            }
        }
    }
}
