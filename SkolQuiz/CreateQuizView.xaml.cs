using SkolQuiz.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            LoadCategories();
        }

        private void LoadCategories()
        {
            CategoryComboBox.Items.Clear();
            
            var categories = new List<string> { "Sport", "Film", "Spel", "Anime", "Allmänt", "Geografi", "Historia", "Vetenskap" };
            
            foreach (var category in categories)
            {
                CategoryComboBox.Items.Add(category);
            }
            
            CategoryComboBox.SelectedIndex = 4;
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

        private string CopyImageToAppFolder(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
                return string.Empty;

            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string imagesFolderPath = Path.Combine(appDataPath, "SkolQuiz", "Images");
                
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(sourcePath)}";
                string destPath = Path.Combine(imagesFolderPath, fileName);
                
                File.Copy(sourcePath, destPath, true);
                
                return destPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kunde inte kopiera bilden:{Environment.NewLine}{ex.Message}");
                return string.Empty;
            }
        }

        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuestionTextBox.Text))
            {
                MessageBox.Show("Frågan får inte vara tom!");
                return;
            }

            bool answer1Empty = string.IsNullOrWhiteSpace(Answer1TextBox.Text);
            bool answer2Empty = string.IsNullOrWhiteSpace(Answer2TextBox.Text);
            bool answer3Empty = string.IsNullOrWhiteSpace(Answer3TextBox.Text);
            bool answer4Empty = string.IsNullOrWhiteSpace(Answer4TextBox.Text);
            
            if (answer1Empty || answer2Empty || answer3Empty || answer4Empty)
            {
                MessageBox.Show("Alla fyra svarsalternativ måste fyllas i!");
                return;
            }

            if (CategoryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Välj en kategori!");
                return;
            }

            int correctAnswer = CorrectAnswerComboBox.SelectedIndex;
            string[] answers = new string[] { Answer1TextBox.Text, Answer2TextBox.Text, Answer3TextBox.Text, Answer4TextBox.Text };
            
            string imagePath = string.IsNullOrEmpty(selectedImagePath) ? string.Empty : CopyImageToAppFolder(selectedImagePath);
            
            Question question = new Question(
                QuestionTextBox.Text,
                answers,
                correctAnswer,
                CategoryComboBox.SelectedItem.ToString(),
                imagePath
            );

            questions.Add(question);
            MessageBox.Show($"Fråga tillagd!{Environment.NewLine}Totalt: {questions.Count} frågor");

            QuestionTextBox.Clear();
            Answer1TextBox.Text = "Alternativ 1";
            Answer2TextBox.Text = "Alternativ 2";
            Answer3TextBox.Text = "Alternativ 3";
            Answer4TextBox.Text = "Alternativ 4";
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
                
                questions.Clear();
                QuizTitleTextBox.Clear();
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
    }
}
