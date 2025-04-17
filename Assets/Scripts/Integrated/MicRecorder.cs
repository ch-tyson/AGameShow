using UnityEngine;

public class MicRecorder : MonoBehaviour
{
    private string microphoneName;
    private bool isRecording = false;
    [SerializeField] private DialogueManager dialogueManager;

    void Start()
    {
        microphoneName = Microphone.devices[0];
    }

    void Update()
    {
        // Start recording when space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space) && !isRecording && !dialogueManager.avatarSpeaking)
        {
            StartRecording();
        }

        // Stop recording
        if (Input.GetKeyUp(KeyCode.Space) && isRecording)
        {
            StopRecording();
        }
    }

    void StartRecording()
    {
        isRecording = true;

        // Start Recording: (Non-looping, 20 second audio clip, 16kHz frequency)
        dialogueManager.playerAudioSource.clip = Microphone.Start(microphoneName, false, 20, 16000);
    }

    void StopRecording()
    {
        // Stop Recording
        Microphone.End(microphoneName);

        dialogueManager.SpeechToText();
        isRecording = false;
    }
}