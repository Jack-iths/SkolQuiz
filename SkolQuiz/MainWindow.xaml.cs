using SkolQuiz.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            string quizFolderPath = Path.Combine(appDataPath, "SkolQuiz", "Quizes");
            
            // Försök först läsa från projektmappen (där .exe ligger)
            string projectQuizPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quizes");
            
            // Om projektmappen finns, använd den direkt (enklare för utveckling och tester)
            if (Directory.Exists(projectQuizPath))
            {
                // Läs direkt från projektmappen
                foreach (var file in Directory.GetFiles(projectQuizPath, "*.json"))
                {
                    try
                    {
                        string json = File.ReadAllText(file);
                        var quiz = JsonSerializer.Deserialize<Quiz>(json);
                        if (quiz != null)
                        {
                            quizes.Add(quiz);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fel vid läsning av {Path.GetFileName(file)}:\n{ex.Message}");
                    }
                }
            }
            else
            {
                // Om projektmappen inte finns, försök läsa från AppData (backup)
                DirectoryInfo QuizFolder = new DirectoryInfo(quizFolderPath);
                
                if (QuizFolder.Exists)
                {
                    foreach (FileInfo file in QuizFolder.EnumerateFiles("*.json"))
                    {
                        try
                        {
                            string json = File.ReadAllText(file.FullName);
                            var quiz = JsonSerializer.Deserialize<Quiz>(json);
                            if (quiz != null)
                            {
                                quizes.Add(quiz);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Fel vid läsning av {file.Name}:\n{ex.Message}");
                        }
                    }
                }
            }

            // Visa ett meddelande om inga filer hittades
            if (quizes.Count == 0)
            {
                MessageBox.Show($"Inga JSON-filer hittades!\n\nKontrollera att filerna finns i:\n{projectQuizPath}\n\nOBS: Se till att projektet har byggts (Build) så att JSON-filerna kopieras till output-mappen!");
            }
        }
    }
}
