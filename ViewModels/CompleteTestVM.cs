using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestTask2.DataAccess;
using TestTask2.Models;
using TestTask2.Models.Core;
using TestTask2.Utilities;

namespace TestTask2.ViewModels
{
    class CompleteTestVM : ViewModelBase
    {
        public CompleteTestVM()
        {
            IsTestInProgress = false;
            LoadTests();

        }

        private string _resultStatus = "";
        public string ResultStatus
        {
            get => _resultStatus;
            set
            {
                _resultStatus = value;
                OnPropertyChanged();
                //ResultStatus = $"Ваш результат: \n 20/23 баллов";
            }
        }


        private bool _isTestInProgress;
        private bool IsTestInProgress
        {
            get
            {
                return _isTestInProgress;
            }

            set
            {
                _isTestInProgress = value;

                BtnStatus = value ? "Завершить тест" : "Начать тест";
                TbStatus = value ? "Текущий тест: " : "Выберите тест: ";

                //OnPropertyChanged();
            }
        }

        private bool _isTestFinished;
        public bool IsTestFinished
        {
            get
            {
                return _isTestFinished;
            }
            set
            {
                _isTestFinished = value;
                // и вот тут вот все методы и вообще все, 
                // что должно произойти по окончанию метода, по сути.
            }
        }

        private string _btnStatus;
        public string BtnStatus
        {
            get
            {
                return _btnStatus;
            }
            set
            {
                _btnStatus = value;
                OnPropertyChanged();
            }
        }

        private string _tbStatus;
        public string TbStatus
        {
            get
            {
                return _tbStatus;
            }
            set
            {
                _tbStatus = value;
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

        private Test _currentTest;
        public Test CurrentTest
        {
            get
            {
                if (_currentTest == null)
                {
                    _currentTest = new Test();
                }

                return _currentTest;
            }
            set
            {
                _currentTest = value;
                OnPropertyChanged();
            }
        }

        private void GetQuestionsList()
        {
            foreach (Question q in DBAccess.LoadQuestions(SelectedTest.id))
            {
                CurrentTest.QuestionsList.Add(q);
            }

            if (CurrentTest.QuestionsList.Count > 0)
            {

                CurrentTest.CurrentQuestionIndex = 1;
                CurrentTest.AnswersIntoChBoxes();
                CurrentQuestion = CurrentTest.QuestionsList[0];
            }
        }

        public ObservableCollection<TestInfo> AllTests { get; set; } = new();

        private TestInfo _selectedTest;
        public TestInfo SelectedTest
        {
            get => _selectedTest;
            set
            {
                _selectedTest = value;
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
                TbStatus = "Нет доступных тестов.";
            }
        }
        


        private RelayCommand _startSelectedTestCommand;
        public RelayCommand StartSelectedTestCommand => _startSelectedTestCommand ?? (_startSelectedTestCommand = new RelayCommand(StartSelectedTest));
        private void StartSelectedTest()
        {
            if(IsTestInProgress)
            { 
                //вариант досрочного завершения теста
                //...
                return; 
            }

            //вариант начала теста
            IsTestInProgress = true;

            //загружаем из бд список вопросов и дальше по плану
        }

        private RelayCommand _finishTestCommand;
        public RelayCommand FinishTestCommand => _finishTestCommand ?? (_finishTestCommand = new RelayCommand(FinishTest));
        private void FinishTest()
        {
            //перенести в StartSelectedTestCommand по булевскому флагу??? 

        }






    }
}
