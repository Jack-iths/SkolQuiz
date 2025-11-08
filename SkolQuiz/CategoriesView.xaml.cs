using SkolQuiz.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkolQuiz
{
    public partial class CategoriesView : UserControl
    {
        public Quiz selectedQuiz { get; set; }
        
        public CategoriesView()
        {
            InitializeComponent();
            Loaded += CategoriesView_Loaded;
        }

        private void CategoriesView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDynamicCategories();
        }

        private void LoadDynamicCategories()
        {
            DynamicCategoriesPanel.Children.Clear();

            if (selectedQuiz == null || selectedQuiz.Questions == null || selectedQuiz.Questions.Count == 0)
            {
                MessageBox.Show("Inga frågor hittades i detta quiz!");
                return;
            }

            List<string> allCategories = new List<string>();
            foreach (Question question in selectedQuiz.Questions)
            {
                allCategories.Add(question.Category);
            }
            
            List<string> uniqueCategories = allCategories.Distinct().ToList();
            
            uniqueCategories.Sort();
            
            var colors = new[] { "#FF3498DB", "#FF9B59B6", "#FFE74C3C", "#FF2ECC71", "#FF1ABC9C", "#FFF39C12", "#FFE67E22", "#FF34495E" };
            int colorIndex = 0;

            foreach (string category in uniqueCategories)
            {
                int questionCount = 0;
                foreach (Question question in selectedQuiz.Questions)
                {
                    if (question.Category == category)
                    {
                        questionCount++;
                    }
                }
                
                Color buttonColor = (Color)ColorConverter.ConvertFromString(colors[colorIndex % colors.Length]);
                
                var button = new Button
                {
                    Content = $"{category.ToUpper()}\n({questionCount} frågor)",
                    Height = 80,
                    Margin = new Thickness(10),
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.White),
                    Background = new SolidColorBrush(buttonColor),
                    BorderThickness = new Thickness(0),
                    Tag = category
                };

                button.Click += CategoryButton_Click;
                DynamicCategoriesPanel.Children.Add(button);
                colorIndex++;
            }
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null)
            {
                return;
            }
            
            string category = clickedButton.Tag?.ToString();
            if (string.IsNullOrEmpty(category))
            {
                return;
            }

            List<Question> categoryQuestions = new List<Question>();
            foreach (Question question in selectedQuiz.Questions)
            {
                if (question.Category == category)
                {
                    categoryQuestions.Add(question);
                }
            }

            if (categoryQuestions.Count == 0)
            {
                MessageBox.Show($"Inga frågor hittades för {category}-kategorin!");
                return;
            }

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                StartView startView = new StartView();
                startView.questions = categoryQuestions;
                mainWindow.MainContent.Content = startView;
            }
        }

        private void AllCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            List<Question> allQuestions = new List<Question>();
            foreach (Question question in selectedQuiz.Questions)
            {
                allQuestions.Add(question);
            }

            if (allQuestions.Count == 0)
            {
                MessageBox.Show("Inga frågor hittades!");
                return;
            }

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                StartView startView = new StartView();
                startView.questions = allQuestions;
                mainWindow.MainContent.Content = startView;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                QuizSelectionView quizSelectionView = new QuizSelectionView();
                quizSelectionView.quizes = mainWindow.quizes;
                mainWindow.MainContent.Content = quizSelectionView;
            }
        }
    }
}
