using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    [Header("DialogueManager")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private WhisperIntegrated whisper;
    [SerializeField] private JetsIntegrated jets;
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioSource avatarAudioSource;

    [Header("Transcript")]
    [SerializeField] private Transcript transcript;

    [Header("LLMManager")]
    [SerializeField] private LLMManager llmManager;

    void Start()
    {
        if (dialogueManager == null || whisper == null || jets == null || playerAudioSource == null || transcript == null)
        {
            Debug.LogWarning("Missing components in LevelSetup");
            return;
        }

        // Setup DialogueManager
        dialogueManager.whisper = whisper;
        dialogueManager.playerAudioSource = playerAudioSource;
        dialogueManager.jets = jets;
        dialogueManager.avatarAudioSource = avatarAudioSource;
        dialogueManager.transcript = transcript;
        
        // Setup Transcript Listeners
        whisper.STTComplete += transcript.HandleSTTComplete;
        llmManager.OnStartTTS += transcript.HandleStartTTS;
    }
}
