using SkolQuiz.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SkolQuiz
{
    public partial class CategoriesView : Window
    {
        public Quiz selectedQuiz { get; set; }
        
        public CategoriesView()
        {
            InitializeComponent();
        }

        private void SportQuizButton_Click(object sender, RoutedEventArgs e)
        {
            List<Question> sportQuestion = selectedQuiz.Questions.Where(q => q.Category == "Sport").ToList();

            if (sportQuestion.Count == 0)
            {
                MessageBox.Show("Inga frågor hittades för Sport-kategorin!");
                return;
            }

            StartView startSport = new StartView();
            startSport.questions = sportQuestion;
            startSport.Show();
            Close();
        }

        private void MusicQuizButton_Click(object sender, RoutedEventArgs e)
        {
            List<Question> filmQuestion = selectedQuiz.Questions.Where(q => q.Category == "Film").ToList();

            if (filmQuestion.Count == 0)
            {
                MessageBox.Show("Inga frågor hittades för Film-kategorin!");
                return;
            }

            StartView startFilm = new StartView();
            startFilm.questions = filmQuestion;
            startFilm.Show();
            Close();
        }

        private void MathQuizButton_Click(object sender, RoutedEventArgs e)
        {
            List<Question> spelQuestion = selectedQuiz.Questions.Where(q => q.Category == "Spel").ToList();

            if (spelQuestion.Count == 0)
            {
                MessageBox.Show("Inga frågor hittades för Spel-kategorin!");
                return;
            }

            StartView startSpel = new StartView();
            startSpel.questions = spelQuestion;
            startSpel.Show();
            Close();
        }

        private void GeographyQuizButton_Click(object sender, RoutedEventArgs e)
        {
            List<Question> animeQuestion = selectedQuiz.Questions.Where(q => q.Category == "Anime").ToList();

            if (animeQuestion.Count == 0)
            {
                MessageBox.Show("Inga frågor hittades för Anime-kategorin!");
                return;
            }

            StartView startAnime = new StartView();
            startAnime.questions = animeQuestion;
            startAnime.Show();
            Close();
        }

        private void AllCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            // Hämta alla frågor från alla kategorier
            List<Question> allQuestions = selectedQuiz.Questions.ToList();

            if (allQuestions.Count == 0)
            {
                MessageBox.Show("Inga frågor hittades!");
                return;
            }

            // Starta quiz med alla frågor blandat
            StartView startAll = new StartView();
            startAll.questions = allQuestions;
            startAll.Show();
            Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
