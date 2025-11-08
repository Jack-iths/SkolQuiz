using SkolQuiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
        private SoundPlayer correctSoundPlayer;
        private SoundPlayer incorrectSoundPlayer;

        public QuizView()
        {
            InitializeComponent();
            Loaded += QuizView_Loaded;
            UpdateScoreDisplay();
            InitializeSoundPlayers();
        }

        private void InitializeSoundPlayers()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string correctSoundPath = System.IO.Path.Combine(baseDirectory, "Sounds", "correct.wav");
            string incorrectSoundPath = System.IO.Path.Combine(baseDirectory, "Sounds", "incorrect.wav");

            correctSoundPlayer = new SoundPlayer(correctSoundPath);
            correctSoundPlayer.Load();

            incorrectSoundPlayer = new SoundPlayer(incorrectSoundPath);
            incorrectSoundPlayer.Load();
        }

        private void UpdateScoreDisplay()
        {
            if (totalAnswered > 0)
            {
                double percentage = (score / (double)totalAnswered) * 100;
                string formattedPercentage = percentage.ToString("F1");
                ScoreText.Text = $"Poäng: {score}/{totalAnswered} ({formattedPercentage}%)";
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
                ShowFinalResults();
                return;
            }

            CurrentQuestion = questions[currentIndex];
            DisplayQuestionContent();
            UpdateQuestionCounter();
            DataContext = CurrentQuestion;
            LoadQuestionImage();
        }

        private void ShowFinalResults()
        {
            double finalPercentage = (score / (double)questions.Count) * 100;
            string formattedPercentage = finalPercentage.ToString("F1");
            string message = $"Quiz avslutat!{Environment.NewLine}{Environment.NewLine}Du fick {score} av {questions.Count} rätt!{Environment.NewLine}Resultat: {formattedPercentage}%";
            
            MessageBox.Show(message);

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = null;
            }
        }

        private void DisplayQuestionContent()
        {
            CurrentQuestionText.Text = CurrentQuestion.Statement;
            AnswerA.Content = CurrentQuestion.Answers[0];
            AnswerB.Content = CurrentQuestion.Answers[1];
            AnswerC.Content = CurrentQuestion.Answers[2];

            if (CurrentQuestion.Answers.Length > 3)
            {
                AnswerD.Content = CurrentQuestion.Answers[3];
            }
            else
            {
                AnswerD.Content = "Inget svar";
            }
        }

        private void UpdateQuestionCounter()
        {
            int questionNumber = currentIndex + 1;
            int totalQuestions = questions.Count;
            CounterQuestion = $"Fråga {questionNumber} av {totalQuestions}";
            CounterQuestionText.Text = CounterQuestion;
        }

        private void LoadQuestionImage()
        {
            bool hasImagePath = !string.IsNullOrEmpty(CurrentQuestion.ImagePath);
            bool imageFileExists = hasImagePath && System.IO.File.Exists(CurrentQuestion.ImagePath);

            if (imageFileExists)
            {
                try
                {
                    var imageUri = new Uri(CurrentQuestion.ImagePath);
                    var bitmap = new BitmapImage(imageUri);
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
            Button clickedButton = sender as Button;
            string buttonTag = clickedButton.Tag.ToString();
            int selectedAnswer = int.Parse(buttonTag);

            bool isCorrectAnswer = (selectedAnswer == CurrentQuestion.CorrectAnswers);

            if (isCorrectAnswer)
            {
                HandleCorrectAnswer();
            }
            else
            {
                HandleIncorrectAnswer();
            }
            
            totalAnswered++;
            currentIndex++;
            UpdateScoreDisplay();
            ShowQuestion();
        }

        private void HandleCorrectAnswer()
        {
            correctSoundPlayer.Play();
            MessageBox.Show("Bra jobbat!");
            score++;
        }

        private void HandleIncorrectAnswer()
        {
            incorrectSoundPlayer.Play();
            string correctAnswerText = CurrentQuestion.Answers[CurrentQuestion.CorrectAnswers];
            string message = $"Fel svar{Environment.NewLine}Rätt svar var: {correctAnswerText}";
            MessageBox.Show(message);
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            totalAnswered++;
            currentIndex++;
            UpdateScoreDisplay();
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
