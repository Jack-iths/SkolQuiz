using SkolQuiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SkolQuiz
{

    public partial class StartView : Window
    {
        public List<Question> questions { get; set; }
        public StartView()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            QuizView quiz = new QuizView();

            Random random = new Random();
            quiz.questions = questions.OrderBy(q => random.Next()).ToList();

            quiz.Show();
            Close();
        }
    }
}