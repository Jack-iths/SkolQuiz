using SkolQuiz.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SkolQuiz
{
    public partial class CreateQuizView : UserControl
    {
        private List<Question> questions = new List<Question>();
        private string selectedImagePath = string.Empty;

        public CreateQuizView()
        {
            InitializeComponent();
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bildfiler (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|Alla filer (*.*)|*.*";
            openFileDialog.Title = "Välj en bild för frågan";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                selectedImagePath = openFileDialog.FileName;
                string fileName = Path.GetFileName(selectedImagePath);
                ImagePathText.Text = $"Vald bild: {fileName}";
                
                try
                {
                    Uri imageUri = new Uri(selectedImagePath);
                    BitmapImage bitmap = new BitmapImage(imageUri);
                    PreviewImage.Source = bitmap;
                    PreviewImage.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kunde inte ladda bilden:{Environment.NewLine}{ex.Message}");
                    selectedImagePath = string.Empty;
                }
            }
        }

        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            // Validera input
            if (string.IsNullOrWhiteSpace(QuestionTextBox.Text))
            {
                MessageBox.Show("Frågan får inte vara tom!");
                return;
            }

            bool answer1Empty = string.IsNullOrWhiteSpace(Answer1TextBox.Text);
            bool answer2Empty = string.IsNullOrWhiteSpace(Answer2TextBox.Text);
            bool answer3Empty = string.IsNullOrWhiteSpace(Answer3TextBox.Text);
            
            if (answer1Empty || answer2Empty || answer3Empty)
            {
                MessageBox.Show("Alla tre svarsalternativ måste fyllas i!");
                return;
            }

            int correctAnswer = CorrectAnswerComboBox.SelectedIndex;
            string[] answers = new string[] { Answer1TextBox.Text, Answer2TextBox.Text, Answer3TextBox.Text };
            
            Question question = new Question(
                QuestionTextBox.Text,
                answers,
                correctAnswer,
                CategoryTextBox.Text,
                selectedImagePath
            );

            questions.Add(question);
            MessageBox.Show($"Fråga tillagd!{Environment.NewLine}Totalt: {questions.Count} frågor");

            // Rensa fälten
            QuestionTextBox.Clear();
            Answer1TextBox.Text = "Alternativ 1";
            Answer2TextBox.Text = "Alternativ 2";
            Answer3TextBox.Text = "Alternativ 3";
            CorrectAnswerComboBox.SelectedIndex = 0;
            selectedImagePath = string.Empty;
            ImagePathText.Text = string.Empty;
            PreviewImage.Source = null;
            PreviewImage.Visibility = Visibility.Collapsed;
        }

        private async void SaveQuizButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuizTitleTextBox.Text))
            {
                MessageBox.Show("Quiz-namnet får inte vara tomt!");
                return;
            }

            if (questions.Count == 0)
            {
                MessageBox.Show("Du måste lägga till minst en fråga!");
                return;
            }

            Quiz quiz = new Quiz(QuizTitleTextBox.Text);
            quiz.Title = QuizTitleTextBox.Text;
            quiz.Questions = questions;

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string quizFolderPath = Path.Combine(appDataPath, "SkolQuiz", "Quizes");

            if (!Directory.Exists(quizFolderPath))
            {
                Directory.CreateDirectory(quizFolderPath);
            }

            string fileName = $"{QuizTitleTextBox.Text}.json";
            string filePath = Path.Combine(quizFolderPath, fileName);

            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;
                string json = JsonSerializer.Serialize(quiz, options);
                await File.WriteAllTextAsync(filePath, json);

                MessageBox.Show($"Quiz sparat!{Environment.NewLine}{Environment.NewLine}Fil: {fileName}{Environment.NewLine}Plats: {quizFolderPath}{Environment.NewLine}Antal frågor: {questions.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid sparande:{Environment.NewLine}{ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = null;
            }
        }

        private void CategoryTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
