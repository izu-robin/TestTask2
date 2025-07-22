using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TestTask2.Models.Core
{
    public class Test : INotifyPropertyChanged
    {
        public int id { get; set; }
        public string title { get; set; }

        private int _currentQuestion;
        

        //------------------------------
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //-------------------------------

        public int CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                _currentQuestion = value>=1 ? value : 1;
                OnPropertyChanged();
            }
        }

        public List<Question> QuestionsList { get; } = new();

        public Test(string title)
        {
            this.title = title;
        }

        public Test() {}

        public void SaveCorrectAnswers()
        {
            StringBuilder sb;

            foreach(Question q in this.QuestionsList)
            {
                sb = new StringBuilder();

                for (int i=1; i< q.ChBStatus.Length; i++)
                {
                    if (q.ChBStatus[i])
                    { 
                        sb.Append(i); 
                    }
                }

                q.correctAnswer = sb.ToString();
            }
        }

        public void SaveGivenAnswers()
        {
            StringBuilder sb;

            foreach (Question q in this.QuestionsList)
            {
                sb = new StringBuilder();

                for (int i = 1; i < q.ChBStatus.Length; i++)
                {
                    if (q.ChBStatus[i])
                    {
                        sb.Append(i);
                    }
                }

                q.givenAnswer = sb.ToString();
            }
        }

        public int CheckResult()
        {
            int total = 0;

            foreach(Question q in QuestionsList)
            {
                if (q.givenAnswer==q.correctAnswer)
                {
                    total++;
                }
            }

            return total;
        }

    }
}
