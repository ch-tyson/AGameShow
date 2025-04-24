using UnityEngine;
using System.Collections.Generic;
using System;

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

            public string ToFormattedString()
            {
                string output = $"{questionText}\n";
                for (int i = 0; i < options.Length; i++)
                {
                    output += $"{i + 1}. {options[i]}\n";
                }
                return output.Trim();
            }
        }

        static int currentLevel = 1;
        static int currentQuestionIndex = 0;

        static List<Question>[] levels = new List<Question>[]
        {
            new List<Question>
            {
                new Question("What is 1 + 1?", new string[] {"6", "11", "2", "4"}, 2),
                new Question("What shape has four sides?", new string[] {"Square", "Triangle", "Circle", "Hexagon"}, 0),
                new Question("Which of the following is a mammal?", new string[] {"Spider", "Crocodile", "Otter", "Eagle"}, 2)
            },
            new List<Question>
            {
                new Question("What subject is the most creative?", new string[] {"History", "Math", "Art", "Social Studies"}, 2),
                new Question("Which of the following has four legs?", new string[] {"Seal", "Cassowary", "Dog", "Centipede"}, 2),
                new Question("How many days are in a week?", new string[] {"5", "99999999999", "2", "7"}, 3),
                new Question("Which is the correct horse?", new string[] {"Honse", "Hawrse", "Horse", "Horsey"}, 2),
            },
            new List<Question>
            {
                new Question("Which is the correct 7?", new string[] {"7", "7", "7", "7"}, 0),
                new Question("Who are you saving if you saw your loved ones tied to separate train tracks?", new string[] {"Loved one A", "Loved one B"}, 0) // no right answer here maybe?
            }
        };

        public static string NextQuestion()
        {
            if (currentLevel - 1 >= levels.Length) return "Game over. No more levels.";

            var questions = levels[currentLevel - 1];

            if (currentQuestionIndex >= questions.Count)
            {
                currentLevel++;
                currentQuestionIndex = 0;
                if (currentLevel > levels.Length) return "🎉 You've completed all levels!";
                return $"🎉 Moving to LEVEL {currentLevel}!\n\n{NextQuestion()}";
            }

            return questions[currentQuestionIndex].ToFormattedString();
        }

        public static string AnswerQuestion(string playerAnswer)
        {
            var questions = levels[currentLevel - 1];
            var question = questions[currentQuestionIndex];

            int selectedIndex;
            if (!int.TryParse(playerAnswer, out selectedIndex)) return "Please answer with the number of the correct option.";
            selectedIndex -= 1;

            if (selectedIndex == question.correctIndex)
            {
                currentQuestionIndex++;
                return "✅ Correct!\n\n" + NextQuestion();
            }
            else
            {
                currentQuestionIndex++;
                return $"❌ Incorrect. The correct answer was: {question.options[question.correctIndex]}\n\n{NextQuestion()}";
            }
        }

        public static string RestartGame()
        {
            currentLevel = 1;
            currentQuestionIndex = 0;
            return "🔁 Game restarted!\n\n" + NextQuestion();
        }
    }
}
