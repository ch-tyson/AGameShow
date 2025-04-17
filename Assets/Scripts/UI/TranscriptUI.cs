using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TranscriptUI : MonoBehaviour
{
    public Transcript transcript;
    public TMP_Text contentText;
    public ScrollRect scrollRect;
    private Queue<string> texts = new Queue<string>();
    private bool isUpdatingTextHistory = false;

    void Start()
    {
        if (transcript == null)
        {
            Debug.LogWarning("Transcript SO missing from TranscriptUI");
            return;
        }

        // Listeners
        transcript.OnUpdateTextHistory += HandleUpdateTextHistory;
    }

    private void HandleUpdateTextHistory(string text)
    {
        texts.Enqueue(text);

        if (!isUpdatingTextHistory)
        {
            StartCoroutine(UpdateTextHistory());
        }
    }

    private IEnumerator UpdateTextHistory()
    {
        isUpdatingTextHistory = true;

        while (texts.Count > 0)
        {
            string text = texts.Dequeue();
            yield return StartCoroutine(StreamText(text, 0.05f));
        }
        isUpdatingTextHistory = false;
    }

    private IEnumerator StreamText(string text, float delay)
    {
        foreach (char c in text)
        {
            contentText.text += c;

            yield return new WaitForSeconds(delay);
        }
    }
}