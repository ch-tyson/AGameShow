using UnityEngine;
using UnityEngine.UI;
using LLMUnity;
using System.Collections;

namespace LLMUnitySamples
{
    public class FunctionCalling : MonoBehaviour
    {
        public LLMCharacter llmCharacter;
        public Text questionText;
        public Text[] answerTexts;        // AnswerA to AnswerD
        public Image[] answerPanels;      // PanelA to PanelD
        public InputField playerInput;
        public AudioSource dialogueTypingSource;
        public Text aiText;
        public float textSpeed = 0.01f;
        public AudioSource audioSource;
        public AudioClip correctSound;
        public AudioClip incorrectSound;
        public CameraController cameraController;
        public AudioClip questionAudio;

        void Start()
        {
            playerInput.onSubmit.AddListener(OnPlayerSubmit);
            playerInput.Select();
            DisplayQuestion();
        }

        void OnPlayerSubmit(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;
            playerInput.text = "";
            playerInput.interactable = false;

            HandleAnswer(input);
        }

        async void HandleAnswer(string input)
        {
            Debug.Log($"Player input: {input}");

            var q = TriviaGame.GetCurrentQuestion(); // capture before advancing !IMPORTANT
            bool isCorrect = TriviaGame.EvaluateAnswer(input, out int correctIndex, out int selectedIndex, out string feedback);

            string prompt = $"You are a sarcastic game show host. Here is the current question and its options:\n\n" +
                                        $"Q: {q?.questionText}\n" +
                                        $"A: {string.Join(", ", q?.options)}\n\n" +
                                        $"The player answered: \"{input}\".\n" +
                                        $"Result is:\n\"{feedback}\"\n" +
                                        $"Respond concisely and appropriately based off the feedback, 2 sentences max.";

            string hostReply = await llmCharacter.Chat(prompt);
            string cleanedReply = ExtractFinalFeedback(hostReply);
            StartCoroutine(TypeText(cleanedReply));

            // play answer feedback
            if (audioSource != null && audioSource.clip == questionAudio && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            ColorBlockPanels(correctIndex, selectedIndex);
            if (audioSource != null)
                audioSource.PlayOneShot(isCorrect ? correctSound : incorrectSound);

            StartCoroutine(HandleCameraAndNextQuestion());
        }

        string ExtractFinalFeedback(string response)
        {
            if (string.IsNullOrWhiteSpace(response)) return "";

            if (response.Contains("</think>"))
                return response.Substring(response.LastIndexOf("</think>") + 7).Trim();

            var lines = response.Split('\n');
            for (int i = lines.Length - 1; i >= 0; i--)
                if (!string.IsNullOrWhiteSpace(lines[i]))
                    return lines[i].Trim();

            return response.Trim();
        }

        IEnumerator NextQuestionAfterDelay()
        {
            yield return new WaitForSeconds(2f);
            ResetPanelColors();
            DisplayQuestion();
            playerInput.interactable = true;
            playerInput.Select();
        }
        void DisplayQuestion()
        {
            var question = TriviaGame.GetCurrentQuestion();

            if (question == null)
            {
                questionText.text = "Game Over!";
                foreach (var text in answerTexts) text.text = "";
                return;
            }

            questionText.text = question.Value.questionText;

            for (int i = 0; i < answerTexts.Length; i++)
                answerTexts[i].text = i < question.Value.options.Length ? question.Value.options[i] : "";

            if (audioSource != null && questionAudio != null)
                audioSource.clip = questionAudio;
            audioSource.Play();
        }

        void ColorBlockPanels(int correctIndex, int selectedIndex)
        {
            Color green = new Color(0f, 1f, 0f, 0.5f);
            Color red = new Color(1f, 0f, 0f, 0.5f);

            if (correctIndex >= 0 && correctIndex < answerPanels.Length)
            {
                answerPanels[correctIndex].color = green;
            }

            if (selectedIndex >= 0 && selectedIndex < answerPanels.Length && selectedIndex != correctIndex)
            {
                answerPanels[selectedIndex].color = red;
            }
        }

        void ResetPanelColors()
        {
            foreach (var panel in answerPanels)
                panel.color = new Color(0.8f, 0.8f, 0.8f, 0.5f); // reset
        }

        IEnumerator HandleCameraAndNextQuestion()
        {
            yield return new WaitForSeconds(1f); // wait after feedback

            cameraController?.MoveToThinking();

            yield return new WaitForSeconds(8f); // stay in thinking view

            cameraController?.MoveToDefault();

            yield return new WaitForSeconds(0.5f); // pause for polish

            ResetPanelColors();
            DisplayQuestion();
            playerInput.interactable = true;
            playerInput.Select();
        }

        IEnumerator TypeText(string message)
        {
            aiText.text = "";

            if (dialogueTypingSource != null && dialogueTypingSource.clip != null)
            {
                dialogueTypingSource.Play();
            }

            foreach (char c in message)
            {
                aiText.text += c;
                yield return new WaitForSeconds(textSpeed);
            }

            if (dialogueTypingSource != null && dialogueTypingSource.isPlaying)
            {
                dialogueTypingSource.Stop();
            }
        }
    }
}
