using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
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
        public object QCardView;

        private string _resultStatus = "";
        public string ResultStatus
        {
            get
            {
                if(IsTestFinished)
                {
                    return $"Ваш результат: \n{_resultStatus} правильных ответов из {CurrentTest.QuestionsList.Count} ";
                }
                else
                { return _resultStatus; }   
            } 
            set
            {
                _resultStatus = value;
                OnPropertyChanged();
            }
        }

        private bool _isQuestionVisible = false;
        public bool IsQuestionVisible
        {
            get => _isQuestionVisible;
            set
            {
                _isQuestionVisible = value;
                OnPropertyChanged(nameof(IsQuestionVisible));
            }
        }

        private bool _isTestInProgress;
        public bool IsTestInProgress
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
                
                OnPropertyChanged();
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
                OnPropertyChanged() ;
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

        private string _nextButtonStatus = "Следующий";
        public string NextButtonStatus
        {
            get => _nextButtonStatus;
            set
            {
                if(CurrentTest.QuestionsList.Count==CurrentTest.CurrentQuestionIndex)
                {
                    _nextButtonStatus = "Конец";
                }
                else if(CurrentTest.QuestionsList.Count > CurrentTest.CurrentQuestionIndex)
                    _nextButtonStatus = "Следующий";
                OnPropertyChanged();
            }
        }

        private Question _currentQuestion; 
        public Question CurrentQuestion
        {
            get
            {
                {
                    if (_currentQuestion == null)
                    {
                        _currentQuestion = new Question();
                    }

                    return _currentQuestion;
                }
            }
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

        int total { get; set; }

        private void GetQuestionsList()
        {

            foreach (Question q in DBAccess.LoadQuestions(SelectedTest.id))
            {
                CurrentTest.QuestionsList.Add(q);
            }

            if (CurrentTest.QuestionsList.Count > 0)
            {

                CurrentTest.CurrentQuestionIndex = 0;
                //CurrentTest.AnswersIntoChBoxes();
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
            if (IsQuestionVisible)
            {
                FinishTest();
                return;
            }

            if (SelectedTest==null)
            { 
                return; 
            }

            if (CurrentTest==null)
            {
                CurrentTest = new Test();
            }

            IsTestFinished = false;  

            IsQuestionVisible = !IsQuestionVisible;

            GetQuestionsList();

            IsTestInProgress = true;

            ResultStatus = "";
        }

        private RelayCommand _finishTestCommand;
        public RelayCommand FinishTestCommand => _finishTestCommand ?? (_finishTestCommand = new RelayCommand(FinishTest));
        private void FinishTest()
        {
        
            IsTestInProgress = !IsTestInProgress;
            IsQuestionVisible = !IsQuestionVisible;
            IsTestFinished = true;

            CurrentTest.SaveGivenAnswers();
            ResultStatus = (CurrentTest.CheckResult()).ToString();


            CurrentTest = new Test();
            CurrentQuestion = new Question();

        }

        private RelayCommand _showNextQuestionCommand;
        public RelayCommand ShowNextQuestionCommand => _showNextQuestionCommand ?? (_showNextQuestionCommand = new RelayCommand(ShowNextQuestion));
        private void ShowNextQuestion()
        {
            if (CurrentTest.CurrentQuestionIndex == CurrentTest.QuestionsList.Count)
            {
                FinishTest();

                return;
            }

            NextButtonStatus = "Следующий";

            IsQuestionVisible = false; //здесь и далее смена IsQuestionVisible = false
                                       //на IsQuestionVisible=true нужна чтобы запустить анимацию всплытия карточки вопроса

            CurrentTest.CurrentQuestionIndex += 1;
            CurrentQuestion = CurrentTest.QuestionsList[CurrentTest.CurrentQuestionIndex-1];

            IsQuestionVisible=true;
        }

        private RelayCommand _showPreviousQuestionCommand;
        public RelayCommand ShowPreviousQuestionCommand => _showPreviousQuestionCommand ?? (_showPreviousQuestionCommand = new RelayCommand(ShowPrevioustQuestion));
        private void ShowPrevioustQuestion()
        {
            if(CurrentTest.CurrentQuestionIndex==1)
            { return; }

            IsQuestionVisible = false;

            CurrentTest.CurrentQuestionIndex -= 1;
            CurrentQuestion = CurrentTest.QuestionsList[CurrentTest.CurrentQuestionIndex-1];

            IsQuestionVisible = true;
        }




    }
}
