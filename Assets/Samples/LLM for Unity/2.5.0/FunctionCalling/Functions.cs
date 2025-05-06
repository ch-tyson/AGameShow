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

            public string ToFormattedString()
            {
                string output = questionText + "\n";
                for (int i = 0; i < options.Length; i++)
                {
                    output += $"Option {i + 1} is {options[i]}. ";
                }
                return output.Trim();
            }

        }

        static int currentLevel = 1;
        static int currentQuestionIndex = 0;
        static System.Random random = new System.Random();

        static List<Question>[] levels = new List<Question>[]
        {
            new List<Question>
            {
                new Question("What is 1 plus 1?", new string[] {"Six.", "Eleven.", "Two.", "Four"}, 2),
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
                new Question("Who are you saving if you saw your loved ones tied to separate train tracks?", new string[] {"Loved one A", "Loved one B"}, 0)
            }
        };

        static string[] correctResponses = new string[]
        {
            "That's right! Someone studied.",
            "Correct! You're sharper than you look.",
            "Good job! Maybe you won't embarrass yourself after all."
        };

        static string[] incorrectResponses = new string[]
        {
            "Ouch. That's not it. Better luck next time.",
            "Incorrect! Hope you didn't bet your lunch money on that.",
            "Wrong! But hey, at least you're trying."
        };

        static string[] invalidResponses = new string[]
        {
            "Uhh... that's not even an option. Are we even playing the same game?",
            "Wow. That was creative. And wrong.",
            "Impressive confidence. No clue what you just said though."
        };

        public static string NextQuestion()
        {
            if (currentLevel - 1 >= levels.Length) return "The show is over! Thanks for playing.";

            var questions = levels[currentLevel - 1];

            if (currentQuestionIndex >= questions.Count)
            {
                currentLevel++;
                currentQuestionIndex = 0;
                if (currentLevel > levels.Length) return "You've survived all the rounds! Bravo.";
                return $"Moving on to LEVEL {currentLevel}!\n\n{NextQuestion()}";
            }

            return questions[currentQuestionIndex].ToFormattedString();
        }

        public static string AnswerQuestion(string playerAnswer)
        {
            playerAnswer = NormalizeAnswer(playerAnswer);
            var questions = levels[currentLevel - 1];
            var question = questions[currentQuestionIndex];

            int matchedIndex = Array.FindIndex(question.options, option =>
                NormalizeAnswer(option) == playerAnswer
            );

            if (matchedIndex == -1)
            {
                return invalidResponses[random.Next(invalidResponses.Length)] + "\n\n" + question.ToFormattedString();
            }

            bool isCorrect = matchedIndex == question.correctIndex;
            currentQuestionIndex++;

            if (isCorrect)
            {
                return correctResponses[random.Next(correctResponses.Length)] + "\n\n" + NextQuestion();
            }
            else
            {
                return incorrectResponses[random.Next(incorrectResponses.Length)] + "\n\n" + NextQuestion();
            }
        }

        public static string RestartGame()
        {
            currentLevel = 1;
            currentQuestionIndex = 0;
            return "Resetting the stage! Let's start again.\n\n" + NextQuestion();
        }

        private static string NormalizeAnswer(string input)
        {
            input = input.Trim().ToLower();

            // Strip punctuation
            input = new string(input.Where(char.IsLetterOrDigit).ToArray());

            Dictionary<string, string> numerals = new Dictionary<string, string>
            {
                { "zero", "0" }, { "one", "1" }, { "two", "2" }, { "three", "3" },
                { "four", "4" }, { "five", "5" }, { "six", "6" }, { "seven", "7" },
                { "eight", "8" }, { "nine", "9" }
            };

            if (numerals.TryGetValue(input, out var digit))
                return digit;

            return input;
        }
    }
}

