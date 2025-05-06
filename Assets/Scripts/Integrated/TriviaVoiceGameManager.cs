using UnityEngine;
using LLMUnity;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class TriviaVoiceGameManager : MonoBehaviour
{
    [SerializeField] private LLMManager llmManager;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private LLMCharacter llmCharacter;
    [SerializeField] private Transcript transcript;

    void Start()
    {
        transcript.OnNewPlayerResponse += HandlePlayerInput;

        // Limit what the LLM can return as valid functions
        llmCharacter.grammarString = null;
    }

    private void HandlePlayerInput(string playerMessage)
    {
        _ = HandleLLMWithFunctionCalling(playerMessage.Trim().ToLower());
    }

    private async System.Threading.Tasks.Task HandleLLMWithFunctionCalling(string playerMessage)
    {
        string prompt =
        "You are the enthusiastic and witty host of a trivia game show. Your job is to talk to the contestant (player) naturally, responding to their input with host-like speech.\n\n" +
        "At the END of your message, on a new line by itself, output ONLY ONE of the following function names:\n" +
        "AnswerQuestion\nNextQuestion\nRestartGame\n\n" +
        "Don't explain the function call. Just say the function name as the last line. Here's what the player said:\n\n" +
        $"Player: {playerMessage}";

        // Get full LLM output (e.g., host banter + function name)
        string llmOutput = await llmCharacter.Chat(prompt);
        Debug.Log("[LLM] Raw Output:\n" + llmOutput);

        // Extract function call
        string functionName = ParseFunctionFromLLMOutput(llmOutput);
        string hostSpeech = RemoveFunctionFromEnd(llmOutput);

        // Actually call the function
        string functionResult = CallFunction(functionName, playerMessage);

        // Combine host banter + function outcome
        string fullResponse = hostSpeech + "\n\n" + functionResult;

        Speak(fullResponse);
    }
    private string CallFunction(string functionName, string input = null)
    {
        var method = typeof(LLMUnitySamples.TriviaGame).GetMethod(functionName, BindingFlags.Public | BindingFlags.Static);
        if (method == null)
        {
            Debug.LogError("[TRIVIA] Invalid function: " + functionName);
            return "Sorry, something went wrong.";
        }

        string result = method.GetParameters().Length == 0
            ? (string)method.Invoke(null, null)
            : (string)method.Invoke(null, new object[] { input });

        Debug.Log($"[TRIVIA] Called {functionName} → {result}");
        return result;
    }


    private string ParseFunctionFromLLMOutput(string response)
    {
        if (string.IsNullOrWhiteSpace(response)) return "AnswerQuestion";

        string[] lines = response.Trim().Split('\n');
        string last = lines[lines.Length - 1].Trim();

        if (last == "NextQuestion" || last == "AnswerQuestion" || last == "RestartGame")
            return last;

        return "AnswerQuestion"; // fallback
    }

    private string RemoveFunctionFromEnd(string response)
    {
        if (string.IsNullOrWhiteSpace(response)) return "";

        string[] lines = response.Trim().Split('\n');
        string last = lines[lines.Length - 1].Trim();

        if (last == "NextQuestion" || last == "AnswerQuestion" || last == "RestartGame")
            return string.Join("\n", lines, 0, lines.Length - 1).Trim();

        return response;
    }

    private void Speak(string response)
    {
        transcript.latestAvatarResponse = response;
        dialogueManager.TextToSpeech();
    }
}
