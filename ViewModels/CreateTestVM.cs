using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask2.DataAccess;
using TestTask2.Models.Core;

namespace TestTask2.ViewModels
{
    class CreateTestVM : ViewModelBase
    {
        public CreateTestVM()
        {
            CreatingInProgress = false;
        }

        private string _errorMessage = "Введите название и нажмите \"Создать тест\"";
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        private string _savingStatus = "";
        public string SavingStatus
        {
            get => _savingStatus;
            set
            {
                _savingStatus = value;
                OnPropertyChanged();
            }
        }


        private bool _creatingInProgress;
        public bool CreatingInProgress
        {
            get => _creatingInProgress;
            set
            {
                _creatingInProgress = value;
                CreateSaveButton = "Создать тест";
                OnPropertyChanged();

                //вот сюда еще что-то можно задать, что активно в разных состояниях
            }
        }


        private string _createSaveButton;
        public string CreateSaveButton
        {
            get => _createSaveButton;
            set
            {
                _createSaveButton = value;
                OnPropertyChanged();
            }
        }

        private Test _newTest;
        public Test NewTest
        {
            get
            {
                if(_newTest == null)
                {
                    _newTest = new Test();
                }
                return _newTest;
            }
            set
            {
                _newTest = value;
                OnPropertyChanged();
            }
        }

        private Question _newQuestion;
        public Question NewQuestion
        {
            get
            {
                if(_newQuestion== null)
                {
                    _newQuestion = new Question();
                }

                return _newQuestion;
            }
            set
            {
                _newQuestion = value;
                OnPropertyChanged();
            }
        }

        //private string _questionNumber;
        //public string QuestionNumber
        //{
        //    get
        //    {
        //        return _questionNumber;
               
        //    }
        //    set
        //    {
        //        _questionNumber = $"Вопрос {NewTest.currentQuestion} из {NewTest.QuestionsList.Count}";
        //        OnPropertyChanged();
        //    }
        //}




        private RelayCommand _createNewTestCommand;
        public RelayCommand CreateNewTestCommand => _createNewTestCommand ?? (_createNewTestCommand = new RelayCommand(CreateNewTest));

        private void CreateNewTest()
        {
            ClearCheckForBlanks();
            
            if(SavingStatus!="")
            { SavingStatus = ""; }

            if (string.IsNullOrWhiteSpace(NewTest.title))
            {
                return; //ранний ритёрн на случай незаполненого названия 
            }
            
            if(CreatingInProgress) //то есть сохраняем и вырубаем
            {
                //if(NewTest.QuestionsList.Count==0)
                //{
                //    ErrorMessage = "Нельзя сохранить пустой тест.";
                //}
                //CreatingInProgress = !CreatingInProgress;
                //сохранить в бд и отдельно тест, и пачку вопросов каким-то циклом
                // DBAccess.InsertTest(NewTest);
                // и там цикл на внесение новых вопросов в Questions
            }
            else //то есть начинаем новый тест
            {
                NewTest.CurrentQuestionIndex = 1;
                CreatingInProgress = !CreatingInProgress;
            }
        }


        private RelayCommand _nextQuestionCommand;
        public RelayCommand NextQuestionCommand => _nextQuestionCommand ?? (_nextQuestionCommand = new RelayCommand(NextQuestion));

        private void NextQuestion()
        {
            if(NewTest.CurrentQuestionIndex == NewTest.QuestionsList.Count+1)
            {
                //то есть мы на последнем вопросе из существующих
                //если не отклонялись

                //как-то сохранить отмеченные чекбоксы как ответ

                if (CheckForBlanks())
                { return; }

                NewTest.QuestionsList.Add(NewQuestion);
                NewQuestion = new Question();
                NewTest.CurrentQuestionIndex++;
            }
            else if(NewTest.CurrentQuestionIndex < NewTest.QuestionsList.Count)
            {
                //то есть мы отмотали немного назад
                //как-то сохранить отмеченные чекбоксы как ответ

                if (CheckForBlanks())
                { return; }

                NewQuestion = NewTest.QuestionsList[NewTest.CurrentQuestionIndex++];
                //NewTest.CurrentQuestion++;
            }
            else if(NewTest.CurrentQuestionIndex == NewTest.QuestionsList.Count)
            {
                if (CheckForBlanks())
                { return; }

                ErrorMessage = "";
                NewQuestion = new Question();
                NewTest.QuestionsList.Add(NewQuestion);
                NewTest.CurrentQuestionIndex++;
            }
        }
        private bool CheckForBlanks()
        {
            if(ErrorMessage!="")
            { ErrorMessage = ""; }

            if (string.IsNullOrEmpty(NewQuestion.questionText)) //проверка на пустой вопрос
            {
                ErrorMessage = "Введите вопрос";
                return true;
            }
            else if (string.IsNullOrEmpty(NewQuestion.answer1)
                  || string.IsNullOrEmpty(NewQuestion.answer2))
            {
                ErrorMessage = "Введите хотя бы два первых варианта ответа";
                return true;
            }
            else if (!NewQuestion.ChBStatus[1]
                  && !NewQuestion.ChBStatus[2]
                  && !NewQuestion.ChBStatus[3]
                  && !NewQuestion.ChBStatus[4])
            {
                ErrorMessage = "Отметьте хотя бы один правильный ответ";
                return true;
            }
            else if(CheckInvalidCheckBoxes())
            {
                return true;
            }

                return false;
        }

        private bool CheckInvalidCheckBoxes()
        {
            if ((string.IsNullOrEmpty(NewQuestion.answer1) && NewQuestion.ChBStatus[1]) ||
                (string.IsNullOrEmpty(NewQuestion.answer2) && NewQuestion.ChBStatus[2]) ||
                (string.IsNullOrEmpty(NewQuestion.answer3) && NewQuestion.ChBStatus[3]) ||
                (string.IsNullOrEmpty(NewQuestion.answer4) && NewQuestion.ChBStatus[4]))
            {
                ErrorMessage = "Нельзя отметить правильным пустой ответ.";
                return true;
            }
            return false;
        }

        private void ClearCheckForBlanks()
        {
            if(string.IsNullOrEmpty(NewTest.title))
            { return; }

            if (ErrorMessage != "")
            { ErrorMessage = ""; }
        }

        private RelayCommand _previousQuestionCommand;
        public RelayCommand PreviuousQuestionCommand => _previousQuestionCommand ?? (_previousQuestionCommand = new RelayCommand(PreviousQuestion));

        private void PreviousQuestion()
        {
            ClearCheckForBlanks();

            if (NewTest.QuestionsList.Count == 0)
            { return; }

            NewTest.CurrentQuestionIndex--;

            NewQuestion = NewTest.QuestionsList[NewTest.CurrentQuestionIndex - 1];
        }

        private RelayCommand _saveNewTestCommand;
        public RelayCommand SaveNewTestCommand => (_saveNewTestCommand ?? (_saveNewTestCommand = new RelayCommand(SaveNewTest)));

        private void SaveNewTest()
        {
            ClearCheckForBlanks();

            if (NewTest.QuestionsList.Count == 0)
            {
                ErrorMessage = "Нельзя сохранить пустой тест.";
                return; 
            }

            NewTest.SaveCorrectAnswers();

            if(DBAccess.InsertNewTest(NewTest))
            {
                SavingStatus = "Тест сохранен.";

                NewTest = new Test();
                NewQuestion = new Question();
            }
            else
            {
                SavingStatus = "Ошибка.Тест не сохранен.";
            }
        }
    }
}
