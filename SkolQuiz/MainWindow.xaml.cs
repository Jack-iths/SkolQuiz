using SkolQuiz.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace SkolQuiz
{
    public partial class MainWindow : Window
    {
        public List<Quiz> quizes { get; set; } = new List<Quiz>();
        
        public MainWindow()
        {
            InitializeComponent();
            ReadAllQuestionWithJSON();
        }

        private void ToSeeCategory_Click(object sender, RoutedEventArgs e)
        {
            // Kontrollera om det finns några quiz inlästa
            if (quizes.Count == 0)
            {
                MessageBox.Show("Inga quiz hittades! Kontrollera att JSON-filerna finns i Quizes-mappen.");
                return;
            }

            // Skapa ett nytt fönster för att visa kategorier
            CategoriesView theCategories = new CategoriesView();

            // Slå samman alla frågor från alla quiz-filer till ett enda Quiz-objekt
            Quiz mergedQuiz = new Quiz
            {
                Title = "Kombinerat Quiz",
                Questions = quizes.SelectMany(q => q.Questions).ToList()
            };

            // Skicka med det sammanslagna quizzet till kategorivyn
            theCategories.selectedQuiz = mergedQuiz;
            theCategories.Show();
            Close();
        }

        private void ReadAllQuestionWithJSON()
        {
            // Hämta sökvägen till AppData\Local
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string quizFolderPath = Path.Combine(appDataPath, "QUIZGAME", "Quizes");
            DirectoryInfo QuizFolder = new DirectoryInfo(quizFolderPath);

            // Om mappen inte finns i AppData, skapa den
            if (!QuizFolder.Exists)
            {
                QuizFolder.Create();
            }

            // Kopiera JSON-filerna från projektmappen till AppData
            string projectQuizPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quizes");
            if (Directory.Exists(projectQuizPath))
            {
                // Gå igenom alla JSON-filer i projektmappen
                foreach (var file in Directory.GetFiles(projectQuizPath, "*.json"))
                {
                    // Skapa destinationssökväg i AppData
                    string destFile = Path.Combine(quizFolderPath, Path.GetFileName(file));
                    
                    // Kopiera filen
                    File.Copy(file, destFile, overwrite: true);
                }
            }

            // Läs alla JSON-filer från AppData-mappen
            foreach (FileInfo file in QuizFolder.EnumerateFiles("*.json"))
            {
                using var fs = File.OpenRead(file.FullName);
                var quiz = JsonSerializer.Deserialize<Quiz>(fs);
                if (quiz != null)
                    quizes.Add(quiz);
            }

            // Visa ett meddelande om inga filer hittades
            if (quizes.Count == 0)
            {
                MessageBox.Show($"Inga JSON-filer hittades! Kontrollera att filerna finns i:\n{quizFolderPath}\n\neller i projektmappen: {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quizes")}");
            }
        }
    }
}
