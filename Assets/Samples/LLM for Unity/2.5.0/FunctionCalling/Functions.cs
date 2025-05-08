using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace LLMUnitySamples
{
    public static class TriviaGame
    {
        public struct Question
        {
            public string questionText;
            public string[] options;
            public int correctIndex;

            public Question(string questionText, string[] options, int correctIndex)
            {
                this.questionText = questionText;
                this.options = options;
                this.correctIndex = correctIndex;
            }
        }

        static int currentLevel = 1;
        static int currentQuestionIndex = 0;
        static System.Random random = new System.Random();

        static List<Question>[] levels = new List<Question>[]
        {
        new List<Question>
        {
            new Question("What is 1 + 1?", new string[] {"6", "11", "2", "4"}, 2),
            new Question("What shape has four sides?", new string[] {"Square", "Triangle", "Circle", "Hexagon"}, 0),
            new Question("Which of the following is a mammal?", new string[] {"Spider", "Crocodile", "Otter", "Eagle"}, 2),
            new Question("What subject is the most creative?", new string[] {"History", "Math", "Art", "Social Studies"}, 2),
            new Question("Which of the following has four legs?", new string[] {"Seal", "Cassowary", "Dog", "Centipede"}, 2),
            new Question("How many days are in a week?", new string[] {"5", "99999999999", "2", "7"}, 3),
            new Question("Which is the correct horse?", new string[] {"Honse", "Hawrse", "Horse", "Horsey"}, 2),
            new Question("Which is the correct 7?", new string[] {"7", "7", "7", "7"}, 0),
            new Question("Who are you saving if you saw your loved ones tied to separate train tracks?", new string[] {"Loved one A", "Loved one B"}, 0)
        }
        };

        static string[] correctResponses = new string[]
        {
        "The player got the question correct."
        };

        static string[] incorrectResponses = new string[]
        {
        "The player got the question wrong."
        };

        static string[] invalidResponses = new string[]
        {
        "That wasn't even an option, the player got it wrong.",
        };

        public static Question? GetCurrentQuestion()
        {
            if (currentLevel - 1 >= levels.Length) return null;
            var questions = levels[currentLevel - 1];
            if (currentQuestionIndex >= questions.Count) return null;
            return questions[currentQuestionIndex];
        }

        public static bool EvaluateAnswer(string input, out int correctIndex, out int selectedIndex, out string feedback)
        {
            var question = GetCurrentQuestion();
            if (question == null)
            {
                feedback = "No more questions!";
                correctIndex = -1;
                selectedIndex = -1;
                return false;
            }

            input = input.Trim().ToLower();
            string[] options = question.Value.options;

            selectedIndex = Array.FindIndex(options, o => o.Trim().ToLower() == input);
            correctIndex = question.Value.correctIndex;

            if (selectedIndex == -1)
            {
                feedback = invalidResponses[random.Next(invalidResponses.Length)];
                currentQuestionIndex++;
                return false;
            }

            bool isCorrect = selectedIndex == correctIndex;
            feedback = isCorrect ? correctResponses[random.Next(correctResponses.Length)] : incorrectResponses[random.Next(incorrectResponses.Length)];

            currentQuestionIndex++;
            return isCorrect;
        }
    }
}

