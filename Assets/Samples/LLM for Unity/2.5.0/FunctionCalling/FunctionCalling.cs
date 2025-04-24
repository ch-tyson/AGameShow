using UnityEngine;
using LLMUnity;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;

namespace LLMUnitySamples
{
    public static class Functions
    {
        static System.Random random = new System.Random();


        public static string Weather()
        {
            string[] weather = new string[] { "sunny", "rainy", "cloudy", "snowy" };
            return "The weather is " + weather[random.Next(weather.Length)];
        }

        public static string Time()
        {
            return "The time is " + random.Next(24).ToString("D2") + ":" + random.Next(60).ToString("D2");
        }

        public static string Emotion()
        {
            string[] emotion = new string[] { "happy", "sad", "exhilarated", "ok" };
            return "I am feeling " + emotion[random.Next(emotion.Length)];
        }
    }

    public class FunctionCalling : MonoBehaviour
    {
        public LLMCharacter llmCharacter;
        public InputField playerText;
        public Text AIText;

        void Start()
        {
            playerText.onSubmit.AddListener(onInputFieldSubmit);
            playerText.Select();
            llmCharacter.grammarString = MultipleChoiceGrammar();
        }

        string[] GetFunctionNames()
        {
            return new string[] { "NextQuestion", "AnswerQuestion", "RestartGame" };
        }

        string MultipleChoiceGrammar()
        {
            return "root ::= (\"" + string.Join("\" | \"", GetFunctionNames()) + "\")";
        }

string ConstructPrompt(string message)
{
    return $"You are in a trivia game. The player input is below.\n" +
           "If it's a number (1, 2, etc.), call AnswerQuestion with that input.\n" +
           "Otherwise, just call NextQuestion to start or move forward.\n\n" +
           $"Player Input: {message}";
}

string CallFunction(string functionName, string input = null)
{
    var method = typeof(TriviaGame).GetMethod(functionName);
    if (method == null) return "Unknown function.";
    if (method.GetParameters().Length == 0)
        return (string)method.Invoke(null, null);
    else
        return (string)method.Invoke(null, new object[] { input });
}

async void onInputFieldSubmit(string message)
{
    playerText.interactable = false;

    string functionName = await llmCharacter.Chat(ConstructPrompt(message));

    string result;
    if (functionName.StartsWith("AnswerQuestion"))
    {
        string answer = message;
        result = CallFunction("AnswerQuestion", answer);
    }
    else
    {
        result = CallFunction(functionName);
    }

    AIText.text = result;
    playerText.interactable = true;
}

        public void CancelRequests()
        {
            llmCharacter.CancelRequests();
        }

        public void ExitGame()
        {
            Debug.Log("Exit button clicked");
            Application.Quit();
        }

        bool onValidateWarning = true;
        void OnValidate()
        {
            if (onValidateWarning && !llmCharacter.remote && llmCharacter.llm != null && llmCharacter.llm.model == "")
            {
                Debug.LogWarning($"Please select a model in the {llmCharacter.llm.gameObject.name} GameObject!");
                onValidateWarning = false;
            }
        }
    }
}
