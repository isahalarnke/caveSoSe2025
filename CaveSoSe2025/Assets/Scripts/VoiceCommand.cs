using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceCommand : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;

    void Start()
    {
        // Nur ein einzelnes Wort
        string[] keywords = new string[] { "baum" };

        keywordRecognizer = new KeywordRecognizer(keywords);
        keywordRecognizer.OnPhraseRecognized += OnKeywordRecognized;
        keywordRecognizer.Start();

        Debug.Log("KeywordRecognizer gestartet");
    }

    private void OnKeywordRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log(args.text);
        if (args.text.ToLower() == "baum")
        {
            Debug.Log("Das Wort 'Baum' wurde erkannt!");
            // Instanziieren von Bäumen zum Beispiel
        }
    }

    // Stoppt und zerstört KeyWordRecognizer beim Schließen der Anwendung
    private void OnApplicationQuit()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }
}
