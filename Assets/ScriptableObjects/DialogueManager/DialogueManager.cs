using UnityEngine;
using System;
using System.Collections;

[CreateAssetMenu(fileName = "New Dialogue Manager", menuName = "Dialogue Manager")]
public class DialogueManager : ScriptableObject
{
    [Header("STT")]
    public WhisperIntegrated whisper;
    public AudioSource playerAudioSource;
    public event Action OnSpeechToText;

    [Header("TTS")]
    public JetsIntegrated jets;
    public AudioSource avatarAudioSource;
    public event Action OnTextToSpeech;
    public bool avatarSpeaking = false;

    [Header("Transcript")]
    public Transcript transcript;

    // STT + TTS Methods
    public void SpeechToText()
    {
        // Announce Event
        OnSpeechToText?.Invoke();
        // STT
        whisper.ConvertSpeechToText(playerAudioSource.clip);
        Debug.Log("SPEECH TO TEXT EVENT");
    }

    public void TextToSpeech()
    {
        // Text Processing
        OnTextToSpeech?.Invoke();

        // TTS
        jets.ConvertTextToSpeech(transcript.latestAvatarResponse);
        Debug.Log("TEXT TO SPEECH EVENT");
    }

    public IEnumerator WaitForAvatarAudioToFinish()
    {
        avatarSpeaking = true;
        while (avatarAudioSource.isPlaying)
        {
            yield return null;
        }
        avatarSpeaking = false;
    }
}
