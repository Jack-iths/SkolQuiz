using System;

namespace SkolQuiz.Models
{
    public class Question
    {
        public string Statement { get; set; }
        public string[] Answers { get; set; }
        public int CorrectAnswers { get; set; }
        public string Category { get; set; }
        public string ImagePath { get; set; }

        public Question()
        {
            Statement = string.Empty;
            Answers = Array.Empty<string>();
            Category = string.Empty;
            ImagePath = string.Empty;
        }

        public Question(string statement, string[] answers, int correctAnswers, string category, string imagePath = "")
        {
            Statement = statement;
            Answers = answers;
            CorrectAnswers = correctAnswers;
            Category = category;
            ImagePath = imagePath;
        }
    }
}