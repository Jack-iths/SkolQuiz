using SkolQuiz.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkolQuiz
{
    public partial class MainWindow : Window
    {
        public List<Quiz> quizes { get; set; } = new List<Quiz>();
        
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadQuizes();
        }

        private async Task LoadQuizes()
        {
            quizes.Clear(); // Rensa listan först för att undvika dubbletter

            // Hämta sökvägen till AppData\Local
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string quizFolderPath = Path.Combine(appDataPath, "SkolQuiz", "Quizes");
            
            // Skapa mappen om den inte finns
            if (!Directory.Exists(quizFolderPath))
            {
                Directory.CreateDirectory(quizFolderPath);
            }

            // Försök kopiera från projektmappen till AppData om det behövs
            string projectQuizPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quizes");
            if (Directory.Exists(projectQuizPath))
            {
                foreach (var file in Directory.GetFiles(projectQuizPath, "*.json"))
                {
                    string destFile = Path.Combine(quizFolderPath, Path.GetFileName(file));
                    // Kopiera bara om filen inte redan finns i AppData
                    if (!File.Exists(destFile))
                    {
                        File.Copy(file, destFile, overwrite: false);
                    }
                }
            }

            // Läs alla JSON-filer från AppData asynkront
            if (Directory.Exists(quizFolderPath))
            {
                foreach (var file in Directory.GetFiles(quizFolderPath, "*.json"))
                {
                    try
                    {
                        // Asynkron filläsning
                        string json = await File.ReadAllTextAsync(file);
                        var quiz = JsonSerializer.Deserialize<Quiz>(json);
                        if (quiz != null)
                        {
                            quizes.Add(quiz);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fel vid läsning av {Path.GetFileName(file)}:{Environment.NewLine}{ex.Message}");
                    }
                }
            }

            // Visa ett meddelande om inga filer hittades
            if (quizes.Count == 0)
            {
                MessageBox.Show($"Inga JSON-filer hittades!{Environment.NewLine}{Environment.NewLine}Kontrollera att filerna finns i:{Environment.NewLine}{quizFolderPath}");
            }
        }

        private void ToSeeCategory_Click(object sender, RoutedEventArgs e)
        {
            // Kontrollera om det finns några quiz inlästa
            if (quizes.Count == 0)
            {
                MessageBox.Show("Inga quiz hittades! Kontrollera att JSON-filerna finns i Quizes-mappen.");
                return;
            }

            // Visa kategorivyn med alla quiz
            QuizSelectionView quizSelectionView = new QuizSelectionView();
            quizSelectionView.quizes = quizes;
            MainContent.Content = quizSelectionView;
        }

        private void CreateQuizButton_Click(object sender, RoutedEventArgs e)
        {
            CreateQuizView createQuizView = new CreateQuizView();
            MainContent.Content = createQuizView;
        }

        private async void EditQuizButton_Click(object sender, RoutedEventArgs e)
        {
            // Ladda om quiz innan vi går till redigeringsvyn
            await LoadQuizes();

            if (quizes.Count == 0)
            {
                MessageBox.Show("Inga quiz hittades att redigera!");
                return;
            }
            
            EditQuizView editQuizView = new EditQuizView();
            editQuizView.quizes = quizes;
            MainContent.Content = editQuizView;
        }
    }
}
