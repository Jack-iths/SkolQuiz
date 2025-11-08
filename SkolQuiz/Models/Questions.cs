using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkolQuiz.Models
{
    public class Question
    {
        public string Statement { get; set; }
        public string[] Answers { get; set; }
        public int CorrectAnswers { get; set; }
        public string Category { get; set; }
        public string ImagePath { get; set; }

        // Paramterlös konstruktor för JSON-deserialisering
        public Question()
        {
            Statement = string.Empty;
            Answers = Array.Empty<string>();
            Category = string.Empty;
            ImagePath = string.Empty;
        }

        // Konstruktor med parametrar för att skapa nya frågor
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