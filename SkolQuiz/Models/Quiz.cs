using System;
using System.Collections.Generic;

namespace SkolQuiz.Models
{
    public class Quiz
    {
        public List<Question> Questions { get; set; }
        public string Title { get; set; }

        public Quiz()
        {
            Questions = new List<Question>();
            Title = string.Empty;
        }

        public Quiz(string title)
        {
            Title = title;
            Questions = new List<Question>();
        }

        public Question GetRandomQuestion()
        {
            var random = new Random();
            if (Questions.Count == 0)
                throw new InvalidOperationException("Inga frågor finns i quizet");
            return Questions[random.Next(Questions.Count)];
        }

        public void AddQuestion(string statement, int correctAnswer, params string[] answers)
        {
            var newQuestion = new Question(statement, answers, correctAnswer, Title);
            Questions.Add(newQuestion);
        }

        public void RemoveQuestion(int index)
        {
            if (index >= 0 && index < Questions.Count)
            {
                Questions.RemoveAt(index);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index är utanför gränserna");
            }
        }
    }
}