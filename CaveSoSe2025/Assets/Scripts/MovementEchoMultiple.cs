using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementEchoMultiple : MonoBehaviour
{
    // Positionstransfer enthält die aktuellen Positionen des Spiegel-Avatars
    public PositionTransfer tracker;

    public float recordingDuration;
    public float fps;
    public AudioClip recordSound;

    List<List<Dictionary<string, Vector3>>> allRecordedFrames = new List<List<Dictionary<string, Vector3>>>();
    private bool isRecording = false;
    private float timer = 0f;

    // Liste aller aktuellen aufgenommenen Positionen
    private List<Dictionary<string, Vector3>> recordedFrames = new List<Dictionary<string, Vector3>>();
    public BoxCollider spawnArea;

 
    // Speichern der mehreren Echo Instanzen
    private int echoIndex = 0;

    // Offsets für alle jewweiligen Instanzen
    private List<Vector3> echoOffsets = new List<Vector3>();

    private List<(string from, string to)> boneConnections = new List<(string, string)>
{
    ("HeadCube", "NeckCube"),
    ("NeckCube", "SpineMidCube"),
    ("SpineMidCube", "SpineBaseCube"),
    ("SpineBaseCube", "HipLeftCube"),
    ("SpineBaseCube", "HipRightCube"),
    ("ShoulderLeftCube", "ElbowLeftCube"),
    ("ElbowLeftCube", "HandLeftCube"),
    ("ShoulderRightCube", "ElbowRightCube"),
    ("ElbowRightCube", "HandRightCube"),
    ("HipLeftCube", "KneeLeftCube"),
    ("KneeLeftCube", "AnkleLeftCube"),
    ("AnkleLeftCube", "FootLeftCube"),
    ("HipRightCube", "KneeRightCube"),
    ("KneeRightCube", "AnkleRightCube"),
    ("AnkleRightCube", "FootRightCube"),
};
    // Alle Punkte zum Linine rendern speichern
    private List<List<(Transform from, Transform to)>> allLineEndpoints = new List<List<(Transform, Transform)>>();
    private List<List<LineRenderer>> allLineRenderers = new List<List<LineRenderer>>();

    //Lösungsansatz wie man from to so aktualisiert, dass der Lihne Renderer korrekt gesetzt wird...

    void Start()
    {
        Invoke("StartRecording", 10f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // nur zum Testen im Unity Editor
        {
            Debug.Log("T key recording starting");
            StartRecording();
        }

        if (isRecording)
        {
            timer += Time.deltaTime;
            RecordFrame();

            //Wenn isRecording auf false gesetzt wird --> keine weitere Speicherung der Bewegungsdaten in Liste Recorded Frames deshalb dann allen allRecordedFrames hinzufügen
            if (timer >= recordingDuration)
            {
                isRecording = false;
                allRecordedFrames.Add(recordedFrames);
                StartCoroutine(HandleReplayAndNextRecording());
            }
        }
        UpdateLineRenderer();
    }

    public void StartRecording()
    {
        recordedFrames = new List<Dictionary<string, Vector3>>();
        timer = 0f;
        isRecording = true;
        Debug.Log("Aufzeichnung gestartet.");
        if (recordSound == null) return;

        GameObject soundObj = new GameObject("RecordSound");

        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip = recordSound;
        audioSource.Play();

        Destroy(soundObj, recordSound.length + 0.1f);
    }

    void RecordFrame()
    {

        Dictionary<string, Vector3> frame = new Dictionary<string, Vector3>();
        foreach (var pair in tracker.jointCubes)
        {
            frame[pair.Key] = pair.Value.transform.position;
        }
        recordedFrames.Add(frame);
    }

    Dictionary<string, GameObject> CreateEcho(int index)
    {
        GameObject echoParent = new GameObject();
        echoParent.name = "_Echo_" + index;
        Vector3 offset = GetRandomPositionInBox(spawnArea);
        echoOffsets.Add(offset);

        Dictionary<string, GameObject> echoDict = new Dictionary<string, GameObject>();
        var baseFrame = recordedFrames[0];

        List<(Transform, Transform)> echoLineEndpoints = new List<(Transform, Transform)>();
        List<LineRenderer> echoLines = new List<LineRenderer>();

        foreach (var pair in baseFrame)
        {
            var original = tracker.jointCubes[pair.Key];
            Vector3 worldPosWithOffset = pair.Value + offset;
            var echo = Instantiate(original, worldPosWithOffset, Quaternion.identity);
            echo.name = pair.Key + "_Echo_" + index;
            echo.transform.SetParent(echoParent.transform);
            echoDict[pair.Key] = echo;
        }

        foreach (var (from, to) in boneConnections)
        {
           Debug.Log($"Checking bone connection {from} {to}: FoundFrom={echoDict.ContainsKey(from)}, FoundTo={echoDict.ContainsKey(to)}");

            if (echoDict.ContainsKey(from) && echoDict.ContainsKey(to))
            {
                var fromTf = echoDict[from].transform;
                var toTf = echoDict[to].transform;
                var lr = CreateEchoLine($"{from}_{to}_Line_{index}", fromTf, toTf, echoParent.transform);
                echoLines.Add(lr);
                echoLineEndpoints.Add((fromTf, toTf));
            }
        }

        allLineEndpoints.Add(echoLineEndpoints);
        allLineRenderers.Add(echoLines);

        return echoDict;
    }


    IEnumerator ReplayEcho(
       Dictionary<string, GameObject> echoDict,
       List<Dictionary<string, Vector3>> framesToReplay,
       int echoIdx)
    {
        int localReplayIndex = 0;
        Vector3 offset = echoOffsets[echoIdx];

        while (true)
        {
            var frame = framesToReplay[localReplayIndex];

            foreach (var pair in frame)
            {
                if (echoDict.TryGetValue(pair.Key, out var echoObj))
                {
                    echoObj.transform.position = pair.Value + offset;
                }
            }

            localReplayIndex = (localReplayIndex + 1) % framesToReplay.Count;
            yield return new WaitForSeconds(1f / fps);
        }
    }

    // Zufällige Position in dem Boxcollider Spawn Area, um die neuen Avatare zu instanziieren
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
    IEnumerator HandleReplayAndNextRecording()
    {
        var echo = CreateEcho(echoIndex);

        // Kopie der aktuellen Aufzeichnung übergeben
        var framesToReplay = allRecordedFrames.Last();

        StartCoroutine(ReplayEcho(echo, framesToReplay, echoIndex));
        echoIndex++;
        yield return new WaitForSeconds(1f);
        StartRecording();
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
        for (int echo = 0; echo < allLineRenderers.Count; echo++)
        {
            var lineRenderers = allLineRenderers[echo];
            var endpoints = allLineEndpoints[echo];

            for (int i = 0; i < lineRenderers.Count; i++)
            {
                var lr = lineRenderers[i];
                var (from, to) = endpoints[i];

                if (from != null && to != null)
                {
                    lr.SetPosition(0, from.position);
                    lr.SetPosition(1, to.position);
                }
            }
        }
    }
}
