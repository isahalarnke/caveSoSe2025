using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovementRecord : MonoBehaviour
{
    public PositionTransfer tracker;
    public float recordingDuration;
    public float fps = 30f;
    public AudioClip recordSound;

    private List<Dictionary<string, Vector3>> recordedFrames = new List<Dictionary<string, Vector3>>();
    private bool isRecording = false;
    private float timer = 0f;
    private int replayIndex = 0;
    private GameObject echoParent;

    void Start()
    {
        echoParent = new GameObject("EchoAvatar");
        Invoke("StartRecording", 20f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // oder Trigger durch Kollision oder Sprachbefehl, was auch immer
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
                CreateEcho();
                StartCoroutine(ReplayEcho());
            }
        }
    }

    public void StartRecording()
    {
        recordedFrames.Clear();
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

    void CreateEcho()
    {
        //echoParent = new GameObject("EchoAvatar");

        foreach (var pair in recordedFrames[0])
        {
            var original = tracker.jointCubes[pair.Key];
            var echo = Instantiate(original, pair.Value, Quaternion.identity);
            echo.name = pair.Key + "_Echo";
            echo.transform.SetParent(echoParent.transform, false);
        }
        CreateEchoLine("Line_Neck_Head", "NeckCube_Echo", "HeadCube_Echo");
        CreateEchoLine("Line_SpineMid_Neck", "SpineMidCube_Echo", "NeckCube_Echo");
        CreateEchoLine("Line_SpineBase_SpineMid", "SpineBaseCube_Echo", "SpineMidCube_Echo");
        CreateEchoLine("Line_SpineMid_ShoulderLeft", "SpineMidCube_Echo", "ShoulderLeftCube_Echo");
        CreateEchoLine("Line_SpineMid_ShoulderRight", "SpineMidCube_Echo", "ShoulderRightCube_Echo");
        CreateEchoLine("Line_ShoulderLeft_ElbowLeft", "ShoulderLeftCube_Echo", "ElbowLeftCube_Echo");
        CreateEchoLine("Line_ShoulderRight_ElbowRight", "ShoulderRightCube_Echo", "ElbowRightCube_Echo");
        CreateEchoLine("Line_ElbowLeft_HandLeft", "ElbowLeftCube_Echo", "HandLeftCube_Echo");
        CreateEchoLine("Line_ElbowRight_HandRight", "ElbowRightCube_Echo", "HandRightCube_Echo");
        CreateEchoLine("Line_SpineBase_HipLeft", "SpineBaseCube_Echo", "HipLeftCube_Echo");
        CreateEchoLine("Line_SpineBase_HipRight", "SpineBaseCube_Echo", "HipRightCube_Echo");
        CreateEchoLine("Line_HipLeft_KneeLeft", "HipLeftCube_Echo", "KneeLeftCube_Echo");
        CreateEchoLine("Line_HipRight_KneeRight", "HipRightCube_Echo", "KneeRightCube_Echo");
        CreateEchoLine("Line_KneeLeft_AnkleLeft", "KneeLeftCube_Echo", "AnkleLeftCube_Echo");
        CreateEchoLine("Line_KneeRight_AnkleRight", "KneeRightCube_Echo", "AnkleRightCube_Echo");
        CreateEchoLine("Line_AnkleLeft_FootLeft", "AnkleLeftCube_Echo", "FootLeftCube_Echo");
        CreateEchoLine("Line_AnkleRight_FootRight", "AnkleRightCube_Echo", "FootRightCube_Echo");

        echoParent.transform.position += new Vector3(2f, 0, 0);
    }

    IEnumerator ReplayEcho()
    {
        while (true)
        {
            var frame = recordedFrames[replayIndex];
            foreach (var pair in frame)
            {
                var echoObj = GameObject.Find(pair.Key + "_Echo");
                if (echoObj != null)
                {
                    echoObj.transform.position = pair.Value + new Vector3(1.5f, 0, 0);
                }
            }

            replayIndex = (replayIndex + 1) % recordedFrames.Count;
            yield return new WaitForSeconds(1f / fps);
        }
    }

    void CreateEchoLine(string name, string fromName, string toName)
    {
        GameObject from = GameObject.Find(fromName);
        GameObject to = GameObject.Find(toName);

        if (from == null || to == null) return;

        GameObject lineObj = new GameObject(name);
        lineObj.transform.parent = echoParent.transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        Color ghostColor = new Color(0.5f, 0.9f, 1f, 0.6f);
        lr.startColor = ghostColor;
        lr.endColor = ghostColor;
        lr.startWidth = 0.015f;
        lr.endWidth = 0.015f;
        lr.positionCount = 2;

        // Update im Replay
        StartCoroutine(UpdateLineRenderer(lr, from.transform, to.transform));
    }
    IEnumerator UpdateLineRenderer(LineRenderer lr, Transform a, Transform b)
    {
        while (true)
        {
            lr.SetPosition(0, a.position);
            lr.SetPosition(1, b.position);
            yield return null; // immer pro Frame aktualisieren
        }
    }

}