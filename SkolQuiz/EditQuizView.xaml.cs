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
    public partial class EditQuizView : UserControl
    {
        public List<Quiz> quizes { get; set; } = new List<Quiz>();
        private Quiz? selectedQuiz;
        private List<QuestionEditControl> questionControls = new List<QuestionEditControl>();

        public EditQuizView()
        {
            InitializeComponent();
            Loaded += EditQuizView_Loaded;
        }

        private void EditQuizView_Loaded(object sender, RoutedEventArgs e)
        {
            QuizComboBox.Items.Clear();
            foreach (var quiz in quizes)
            {
                QuizComboBox.Items.Add(quiz.Title);
            }
        }

        private void QuizComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuizComboBox.SelectedIndex == -1) return;

            selectedQuiz = quizes[QuizComboBox.SelectedIndex];
            LoadQuestions();
            SaveChangesButton.IsEnabled = true;
            DeleteQuizButton.IsEnabled = true;
        }

        private void LoadQuestions()
        {
            if (selectedQuiz == null) return;

            QuestionsPanel.Children.Clear();
            questionControls.Clear();

            var questions = selectedQuiz.Questions;
            InfoTextBlock.Text = $"Quiz: {selectedQuiz.Title} - {questions.Count} fragor";

            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                var control = new QuestionEditControl(question, i + 1, DeleteQuestion);
                questionControls.Add(control);
                QuestionsPanel.Children.Add(control);
            }
        }

        private void DeleteQuestion(QuestionEditControl control)
        {
            var result = MessageBox.Show("Ar du saker pa att du vill ta bort denna fraga?", 
                "Bekrafta borttagning", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                questionControls.Remove(control);
                QuestionsPanel.Children.Remove(control);
                for (int i = 0; i < questionControls.Count; i++)
                {
                    questionControls[i].UpdateQuestionNumber(i + 1);
                }
                if (selectedQuiz != null)
                {
                    InfoTextBlock.Text = $"Quiz: {selectedQuiz.Title} - {questionControls.Count} fragor";
                }
            }
        }

        private void DeleteQuizButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedQuiz == null) return;

            var result = MessageBox.Show(
                $"Ar du SAKER pa att du vill ta bort hela quizet '{selectedQuiz.Title}'?{Environment.NewLine}{Environment.NewLine}Denna handling kan inte angras!",
                "Bekrafta borttagning av quiz",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    string quizFolderPath = Path.Combine(appDataPath, "SkolQuiz", "Quizes");
                    string fileName = $"{selectedQuiz.Title}.json";
                    string filePath = Path.Combine(quizFolderPath, fileName);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    int selectedIndex = QuizComboBox.SelectedIndex;
                    quizes.RemoveAt(selectedIndex);
                    QuizComboBox.Items.RemoveAt(selectedIndex);

                    QuestionsPanel.Children.Clear();
                    questionControls.Clear();
                    selectedQuiz = null;
                    SaveChangesButton.IsEnabled = false;
                    DeleteQuizButton.IsEnabled = false;
                    InfoTextBlock.Text = "Quiz borttaget!";

                    MessageBox.Show($"Quiz '{fileName}' har tagits bort!", "Quiz borttaget", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (quizes.Count == 0)
                    {
                        MessageBox.Show("Inga quiz kvar att redigera. Gar tillbaka till huvudmenyn.");
                        if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
                        {
                            mainWindow.MainContent.Content = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fel vid borttagning av quiz:{Environment.NewLine}{ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedQuiz == null) return;

            if (questionControls.Count == 0)
            {
                MessageBox.Show("Ett quiz maste ha minst en fraga!");
                return;
            }

            var updatedQuestions = new List<Question>();
            foreach (var control in questionControls)
            {
                updatedQuestions.Add(control.GetQuestion());
            }

            selectedQuiz.Questions = updatedQuestions;

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string quizFolderPath = Path.Combine(appDataPath, "SkolQuiz", "Quizes");
            string fileName = $"{selectedQuiz.Title}.json";
            string filePath = Path.Combine(quizFolderPath, fileName);

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(selectedQuiz, options);
                await File.WriteAllTextAsync(filePath, json);

                MessageBox.Show($"Quiz uppdaterat!{Environment.NewLine}{Environment.NewLine}Fil: {fileName}{Environment.NewLine}Antal fragor: {updatedQuestions.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid sparande:{Environment.NewLine}{ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = null;
            }
        }
    }

    public class QuestionEditControl : Border
    {
        private TextBox questionTextBox;
        private TextBox answer1TextBox;
        private TextBox answer2TextBox;
        private TextBox answer3TextBox;
        private TextBox answer4TextBox;
        private ComboBox correctAnswerComboBox;
        private ComboBox categoryComboBox;
        private TextBlock questionNumberBlock;
        private Action<QuestionEditControl> onDelete;

        public QuestionEditControl(Question question, int number, Action<QuestionEditControl> deleteCallback)
        {
            onDelete = deleteCallback;
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF34495E"));
            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF27AE60"));
            BorderThickness = new Thickness(2);
            Margin = new Thickness(0, 0, 0, 15);
            Padding = new Thickness(15);
            CornerRadius = new CornerRadius(5);

            var stackPanel = new StackPanel();

            var headerPanel = new DockPanel
            {
                Margin = new Thickness(0, 0, 0, 10)
            };

            questionNumberBlock = new TextBlock
            {
                Text = $"Fraga {number}",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECF0F1"))
            };
            DockPanel.SetDock(questionNumberBlock, Dock.Left);
            headerPanel.Children.Add(questionNumberBlock);

            var deleteButton = new Button
            {
                Content = "Ta bort",
                Width = 100,
                Height = 30,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE74C3C")),
                Foreground = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            deleteButton.Click += (s, e) => onDelete?.Invoke(this);
            DockPanel.SetDock(deleteButton, Dock.Right);
            headerPanel.Children.Add(deleteButton);

            stackPanel.Children.Add(headerPanel);

            var questionLabel = new TextBlock
            {
                Text = "Fraga:",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECF0F1")),
                Margin = new Thickness(0, 5, 0, 3)
            };
            stackPanel.Children.Add(questionLabel);

            questionTextBox = new TextBox
            {
                Text = question.Statement,
                Height = 35,
                FontSize = 14,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 0, 10)
            };
            stackPanel.Children.Add(questionTextBox);

            var answersLabel = new TextBlock
            {
                Text = "Svarsalternativ:",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECF0F1")),
                Margin = new Thickness(0, 5, 0, 3)
            };
            stackPanel.Children.Add(answersLabel);

            answer1TextBox = new TextBox { Text = question.Answers.Length > 0 ? question.Answers[0] : "", Height = 30, Padding = new Thickness(5), Margin = new Thickness(0, 2, 0, 2) };
            answer2TextBox = new TextBox { Text = question.Answers.Length > 1 ? question.Answers[1] : "", Height = 30, Padding = new Thickness(5), Margin = new Thickness(0, 2, 0, 2) };
            answer3TextBox = new TextBox { Text = question.Answers.Length > 2 ? question.Answers[2] : "", Height = 30, Padding = new Thickness(5), Margin = new Thickness(0, 2, 0, 2) };
            answer4TextBox = new TextBox { Text = question.Answers.Length > 3 ? question.Answers[3] : "", Height = 30, Padding = new Thickness(5), Margin = new Thickness(0, 2, 0, 2) };

            stackPanel.Children.Add(answer1TextBox);
            stackPanel.Children.Add(answer2TextBox);
            stackPanel.Children.Add(answer3TextBox);
            stackPanel.Children.Add(answer4TextBox);

            var correctLabel = new TextBlock
            {
                Text = "Ratt svar:",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECF0F1")),
                Margin = new Thickness(0, 10, 0, 3)
            };
            stackPanel.Children.Add(correctLabel);

            correctAnswerComboBox = new ComboBox
            {
                Height = 30,
                Margin = new Thickness(0, 0, 0, 10)
            };
            correctAnswerComboBox.Items.Add("1");
            correctAnswerComboBox.Items.Add("2");
            correctAnswerComboBox.Items.Add("3");
            correctAnswerComboBox.Items.Add("4");
            correctAnswerComboBox.SelectedIndex = question.CorrectAnswers;
            stackPanel.Children.Add(correctAnswerComboBox);

            var categoryLabel = new TextBlock
            {
                Text = "Kategori:",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECF0F1")),
                Margin = new Thickness(0, 5, 0, 3)
            };
            stackPanel.Children.Add(categoryLabel);

            categoryComboBox = new ComboBox
            {
                Height = 30,
                Padding = new Thickness(5)
            };
            
            var categories = new List<string> { "Sport", "Film", "Spel", "Anime", "Allmänt", "Geografi", "Historia", "Vetenskap" };
            foreach (var category in categories)
            {
                categoryComboBox.Items.Add(category);
            }
            
            int categoryIndex = categories.IndexOf(question.Category);
            if (categoryIndex >= 0)
            {
                categoryComboBox.SelectedIndex = categoryIndex;
            }
            else
            {
                categoryComboBox.Items.Add(question.Category);
                categoryComboBox.SelectedIndex = categoryComboBox.Items.Count - 1;
            }
            
            stackPanel.Children.Add(categoryComboBox);

            Child = stackPanel;
        }

        public void UpdateQuestionNumber(int number)
        {
            questionNumberBlock.Text = $"Fraga {number}";
        }

        public Question GetQuestion()
        {
            return new Question(
                questionTextBox.Text,
                new[] { answer1TextBox.Text, answer2TextBox.Text, answer3TextBox.Text, answer4TextBox.Text },
                correctAnswerComboBox.SelectedIndex,
                categoryComboBox.SelectedItem?.ToString() ?? "Allmänt"
            );
        }
    }
}
