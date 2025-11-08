using SkolQuiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SkolQuiz
{

    public partial class StartView : UserControl
    {
        public List<Question> questions { get; set; }

        public StartView()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (questions == null || questions.Count == 0)
            {
                MessageBox.Show("Inga frågor hittades!");
                return;
            }

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                QuizView quiz = new QuizView();
                
                Random random = new Random();
                List<Question> shuffledQuestions = new List<Question>();
                
                foreach (Question question in questions)
                {
                    shuffledQuestions.Add(question);
                }
                
                shuffledQuestions = shuffledQuestions.OrderBy(q => random.Next()).ToList();
                
                quiz.questions = shuffledQuestions;
                mainWindow.MainContent.Content = quiz;
            }
        }
    }
}