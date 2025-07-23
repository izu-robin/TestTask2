using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TestTask2.DataAccess;
using TestTask2.Models.Core;
using TestTask2.Utilities;

namespace TestTask2.ViewModels
{
    class RedactTestVM : ViewModelBase
    {
        public RedactTestVM()
        {
            LoadTests();
        }
        public ObservableCollection<TestInfo> AllTests { get; set; } = new();

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage = "";
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        private bool _redactingInProgress = false;
        public bool RedactingInProgress
        {
            get => _redactingInProgress;
            set
            {
                _redactingInProgress = value;
                OnPropertyChanged();
            }
        }

        private TestInfo _selectedTest; //из комбобокса
        public TestInfo SelectedTest
        {
            get => _selectedTest;
            set
            {
                _selectedTest = value;
                OnPropertyChanged();
            }
        }

        private Test _redactedTest; //со списком вопросов
        public Test RedactedTest
        {
            get
            {
                if(_redactedTest==null)
                {
                    _redactedTest = new Test();
                }
                return _redactedTest;
            }
            set
            {
                _redactedTest = value;
                OnPropertyChanged();
            }
        }

        private Question _currentQuestion; //текущий вопрос
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                _currentQuestion = value;
                OnPropertyChanged();
            }
        }

        private void LoadTests()
        {
            foreach (var t in DBAccess.GetTests())
            {
                AllTests.Add(t);
            }
            if (AllTests.Count == 0)
            {
                ErrorMessage = "Нет доступных тестов.";
            }
        }

        private RelayCommand _redactSelectedTestCommand;
        public RelayCommand RedactSelectedTestCommand => _redactSelectedTestCommand ?? (_redactSelectedTestCommand = new RelayCommand(RedactSelectedTest));
        private void RedactSelectedTest()
        {
            if(SelectedTest==null)
            { return;  }

            foreach( Question q in DBAccess.LoadQuestions(SelectedTest.id))
            {
                RedactedTest.QuestionsList.Add(q);
            }

            if(RedactedTest.QuestionsList.Count>0)
            {

                RedactedTest.CurrentQuestionIndex = 1;
                RedactedTest.AnswersIntoChBoxes();
                CurrentQuestion = RedactedTest.QuestionsList[0];
                RedactingInProgress = true;
            }
        }

        private RelayCommand _showPreviousQuestionCommand;
        public RelayCommand ShowPreviousQuestionCommand => _showPreviousQuestionCommand ?? (_showPreviousQuestionCommand = new RelayCommand(ShowPreviousQuestion));
        private void ShowPreviousQuestion()
        {
            if (SelectedTest == null)
            { return; }

            ClearCheckForBlanks();

            if (CheckInvalidCheckBoxes() || CheckForBlanks())
            { return; }

            if (StatusMessage!="")
            { StatusMessage = ""; }

            RedactedTest.CurrentQuestionIndex--;
            CurrentQuestion = RedactedTest.QuestionsList[RedactedTest.CurrentQuestionIndex-1];
        }

        private RelayCommand _showNextQuestionCommand;
        public RelayCommand ShowNextQuestionCommand => _showNextQuestionCommand ?? (_showNextQuestionCommand = new RelayCommand(ShowNextQuestion));
        private void ShowNextQuestion()
        {
            ClearCheckForBlanks();

            if (CheckInvalidCheckBoxes() || CheckForBlanks() || (SelectedTest == null))
            { return; }

            if (RedactedTest.CurrentQuestionIndex< RedactedTest.QuestionsList.Count)
            {
                RedactedTest.CurrentQuestionIndex++;
                CurrentQuestion = RedactedTest.QuestionsList[RedactedTest.CurrentQuestionIndex-1];
            }

            if(RedactedTest.CurrentQuestionIndex== RedactedTest.QuestionsList.Count)
            {
                StatusMessage = "Конец списка вопросов";
            }
        }
        
        private RelayCommand _saveChangedTestCommand;
        public RelayCommand SaveChangedTestCommand => _saveChangedTestCommand ?? (_saveChangedTestCommand = new RelayCommand(SaveChangedTest));

        private void SaveChangedTest()
        {
            if (CheckInvalidCheckBoxes() || CheckForBlanks() || (SelectedTest == null))
            { return; }

            RedactedTest.SaveCorrectAnswers();

            if(DBAccess.UpdateChangedTest(RedactedTest))
            {
                StatusMessage = "Тест обновлён";
            }
            else
            {
                StatusMessage = "Ошибка сохранения";
            }
        }

        private bool CheckForBlanks()
        {
            if (ErrorMessage != "")
            { ErrorMessage = ""; }

            if (SelectedTest == null)
            { return false; }

            if (string.IsNullOrEmpty(CurrentQuestion.questionText)) //проверка на пустой вопрос
            {
                ErrorMessage = "Введите вопрос";
                return true;
            }
            else if (string.IsNullOrEmpty(CurrentQuestion.answer1)
                  || string.IsNullOrEmpty(CurrentQuestion.answer2))
            {
                ErrorMessage = "Введите хотя бы два первых варианта ответа";
                return true;
            }
            else if (!CurrentQuestion.ChBStatus[1]
                  && !CurrentQuestion.ChBStatus[2]
                  && !CurrentQuestion.ChBStatus[3]
                  && !CurrentQuestion.ChBStatus[4])
            {
                ErrorMessage = "Отметьте хотя бы один правильный ответ";
                return true;
            }
            else if (CheckInvalidCheckBoxes())
            {
                return true;
            }

            return false;
        }

        private bool CheckInvalidCheckBoxes()
        {
            if (SelectedTest == null)
            { return false; }

            if (CurrentQuestion.ChBStatus!=null &&
                (string.IsNullOrEmpty(CurrentQuestion.answer1) && CurrentQuestion.ChBStatus[1]) ||
                (string.IsNullOrEmpty(CurrentQuestion.answer2) && CurrentQuestion.ChBStatus[2]) ||
                (string.IsNullOrEmpty(CurrentQuestion.answer3) && CurrentQuestion.ChBStatus[3]) ||
                (string.IsNullOrEmpty(CurrentQuestion.answer4) && CurrentQuestion.ChBStatus[4]))
            {
                ErrorMessage = "Нельзя отметить правильным пустой ответ.";
                return true;
            }
            return false;
        }

        private void ClearCheckForBlanks()
        {
            if (string.IsNullOrEmpty(RedactedTest.title))
            { return; }

            if (ErrorMessage != "")
            { ErrorMessage = ""; }
        }
    }
}
