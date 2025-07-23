using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TestTask2.DataAccess;
using TestTask2.Models.Core;
using TestTask2.Utilities;

namespace TestTask2.ViewModels
{
    class DeleteTestVM : ViewModelBase
    {
        public ObservableCollection<TestInfo> AllTests { get; set; } = new();

        private string _tbStatus;

        public string TbStatus
        {
            get => _tbStatus;
            set
            {
                _tbStatus = value;
                OnPropertyChanged();
            }
        }

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

        public DeleteTestVM()
        {
            LoadTests();
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

        private RelayCommand _deleteChosenTestCommand;
        public RelayCommand DeleteChosenTestCommand => _deleteChosenTestCommand ?? (_deleteChosenTestCommand = new RelayCommand(DeleteChosenTest));
        private void DeleteChosenTest()
        {
            if (SelectedTest == null)
            {
                return;
            }

            if(DBAccess.DropTest(SelectedTest.id))
            {
                TbStatus = $"Тест {SelectedTest.title} удален.";

                AllTests.Clear();
                LoadTests();
            }
            else
            {
                Console.WriteLine("Ошибка удаления.");
            }
        }


    }
}
