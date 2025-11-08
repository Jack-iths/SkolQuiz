using SkolQuiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SkolQuiz
{
    public partial class QuizView : UserControl
    {
        private int currentIndex = 0;
        public string CounterQuestion { get; set; }
        public Question CurrentQuestion { get; set; }
        public List<Question> questions { get; set; }
        private int score = 0;
        private int totalAnswered = 0;

        public QuizView()
        {
            InitializeComponent();
            Loaded += QuizView_Loaded;
            UpdateScoreDisplay();
        }

        private void UpdateScoreDisplay()
        {
            if (totalAnswered > 0)
            {
                double percentage = (score / (double)totalAnswered) * 100;
                ScoreText.Text = $"Poäng: {score}/{totalAnswered} ({percentage:F1}%)";
            }
            else
            {
                ScoreText.Text = $"Poäng: {score}/{totalAnswered} (0%)";
            }
        }

        private void ShowQuestion()
        {
            if (currentIndex >= questions.Count)
            {
                double finalPercentage = (score / (double)questions.Count) * 100;
                MessageBox.Show($"Quiz avslutat!{Environment.NewLine}{Environment.NewLine}Du fick {score} av {questions.Count} rätt!{Environment.NewLine}Resultat: {finalPercentage:F1}%");
                
                // Gå tillbaka till huvudmenyn
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.MainContent.Content = null;
                }
                return;
            }

            CurrentQuestion = questions[currentIndex];
            CurrentQuestionText.Text = CurrentQuestion.Statement;
            AnswerA.Content = CurrentQuestion.Answers[0];
            AnswerB.Content = CurrentQuestion.Answers[1];
            AnswerC.Content = CurrentQuestion.Answers[2];
            CounterQuestion = $"Fråga {currentIndex + 1} av {questions.Count}";
            CounterQuestionText.Text = CounterQuestion;
            DataContext = CurrentQuestion;

            // Visa bild om den finns
            if (!string.IsNullOrEmpty(CurrentQuestion.ImagePath) && System.IO.File.Exists(CurrentQuestion.ImagePath))
            {
                try
                {
                    var bitmap = new BitmapImage(new Uri(CurrentQuestion.ImagePath));
                    QuestionImage.Source = bitmap;
                    ImageBorder.Visibility = Visibility.Visible;
                }
                catch
                {
                    ImageBorder.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ImageBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            totalAnswered++;

            Button clickedButton = sender as Button;
            string buttonTag = clickedButton.Tag.ToString();
            int selectedAnswer = int.Parse(buttonTag);

            if (selectedAnswer == CurrentQuestion.CorrectAnswers)
            {
                MessageBox.Show("Bra jobbat!");
                score++;
            }
            else
            {
                string correctAnswerText = CurrentQuestion.Answers[CurrentQuestion.CorrectAnswers];
                MessageBox.Show($"Fel svar{Environment.NewLine}Rätt svar var: {correctAnswerText}");
            }
            
            UpdateScoreDisplay();
            currentIndex++;
            ShowQuestion();
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            totalAnswered++;
            currentIndex++;
            ShowQuestion();
        }

        private void QuizView_Loaded(object sender, RoutedEventArgs e)
        {
            ShowQuestion();
        }

        private void GoBackToCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = null;
            }
        }
    }
}
