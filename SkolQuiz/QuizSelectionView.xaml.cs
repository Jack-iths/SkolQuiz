using SkolQuiz.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkolQuiz
{
    public partial class QuizSelectionView : UserControl
    {
        public List<Quiz> quizes { get; set; } = new List<Quiz>();

        public QuizSelectionView()
        {
            InitializeComponent();
            Loaded += QuizSelectionView_Loaded;
        }

        private void QuizSelectionView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadQuizButtons();
        }

        private void LoadQuizButtons()
        {
            QuizListPanel.Children.Clear();

            var colors = new[] { "#FF3498DB", "#FF9B59B6", "#FFE74C3C", "#FF2ECC71", "#FF1ABC9C", "#FFF39C12", "#FFE67E22", "#FF34495E" };
            int colorIndex = 0;

            foreach (var quiz in quizes)
            {
                var button = new Button
                {
                    Content = $"{quiz.Title.ToUpper()}\n({quiz.Questions.Count} frågor)",
                    Height = 80,
                    Margin = new Thickness(10),
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.White),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[colorIndex % colors.Length])),
                    BorderThickness = new Thickness(0),
                    Tag = quiz
                };

                button.Click += QuizButton_Click;
                QuizListPanel.Children.Add(button);
                colorIndex++;
            }
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedQuiz = button?.Tag as Quiz;

            if (selectedQuiz == null) return;

            // Navigate to categories view
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new CategoriesView { selectedQuiz = selectedQuiz };
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = null;
            }
        }
    }
}
