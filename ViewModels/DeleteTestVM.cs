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

namespace TestTask2.ViewModels
{
    class DeleteTestVM : ViewModelBase
    {
        public ObservableCollection<Test> AllTests { get; set; } = new();

        private Test _selectedTest;
        public Test SelectedTest
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

            }
        }

        private void DeleteChosenTest()
        {
            if (SelectedTest == null)
                return;

            // логика удаления теста и тд.
            // В View биндится DeleteChosenTestCommand, надо еще команду сделать
        }


    }
}
