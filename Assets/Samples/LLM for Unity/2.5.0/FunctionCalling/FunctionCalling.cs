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
        public Text aiText;

        public AudioSource audioSource;
        public AudioClip correctSound;
        public AudioClip incorrectSound;

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

            bool isCorrect = TriviaGame.EvaluateAnswer(input, out int correctIndex, out int selectedIndex, out string feedback);

            Debug.Log($"Evaluated answer — Correct: {isCorrect}, Selected: {selectedIndex}, CorrectIndex: {correctIndex}");

            // Game feedback
            ColorBlockPanels(correctIndex, selectedIndex);
            if (audioSource != null)
                audioSource.PlayOneShot(isCorrect ? correctSound : incorrectSound);

            // Send to LLM for conversational reaction
            string prompt = $"The player answered: \"{input}\".\nYour response as the host should match whether it's right or wrong.\n" +
                            $"\"{feedback}\"";
            string hostReply = await llmCharacter.Chat(prompt);
            aiText.text = hostReply;

            StartCoroutine(NextQuestionAfterDelay());
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
            {
                if (i < question.Value.options.Length)
                    answerTexts[i].text = question.Value.options[i];
                else
                    answerTexts[i].text = "";
            }
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
    }
}
