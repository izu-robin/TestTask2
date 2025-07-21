using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestTask2.Models;

namespace TestTask2.ViewModels
{
    class CompleteTestVM : ViewModelBase
    {
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

       
        
        
        public CompleteTestVM()
        {
            IsTestInProgress = false;
           
            



        }


    }
}
