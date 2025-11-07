using SkolQuiz.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SkolQuiz
{
    public partial class QuizView : Window
    {
        private int currentIndex = 0;
        public string CounterQuestion { get; set; }
        public Question CurrentQuestion { get; set; }
        public List<Question> questions { get; set; }
        private int score = 0;

        public QuizView()
        {
            InitializeComponent();
            ScoreText.Text = $"Poäng: {score}";
        }

        private void ShowQuestion()
        {
            if (currentIndex >= questions.Count)
            {
                MessageBox.Show($"Quiz avslutat! Du fick {score} av {questions.Count} rätt!");
                MainWindow newQuizGame = new MainWindow();
                newQuizGame.Show();
                Close();
                return;
            }

            CurrentQuestion = questions[currentIndex];
            CurrentQuestionText.Text = CurrentQuestion.Statement;
            AnswerA.Content = CurrentQuestion.Answers[0];
            AnswerB.Content = CurrentQuestion.Answers[1];
            AnswerC.Content = CurrentQuestion.Answers[2];
            AnswerD.Content = CurrentQuestion.Answers[3];
            CounterQuestion = $"Fråga {currentIndex + 1} av {questions.Count}";
            CounterQuestionText.Text = CounterQuestion;
            DataContext = CurrentQuestion;
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse((sender as Button).Tag.ToString()) == CurrentQuestion.CorrectAnswers)
            {
                MessageBox.Show("Bra jobbat!");
                score++;
                ScoreText.Text = $"Poäng: {score}";
            }
            else
            {
                MessageBox.Show("Fel svar");
            }
            
            currentIndex++;
            ShowQuestion();
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            currentIndex++;
            ShowQuestion();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowQuestion();
        }

        private void GoBackToCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow newQuizGame = new MainWindow();
            newQuizGame.Show();
            Close();
        }
    }
}
