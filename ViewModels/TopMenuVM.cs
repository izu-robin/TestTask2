using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask2.Utilities;
using System.Windows.Input;

namespace TestTask2.ViewModels
{
    class TopMenuVM : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get
            {
                return _currentView;
            }

            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand CompleteCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand RedactCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        private void Complete() => CurrentView = new CompleteTestVM();
        private void Create() => CurrentView = new CreateTestVM();
        private void Redact() => CurrentView = new RedactTestVM();
        private void Delete() => CurrentView = new DeleteTestVM();

        public TopMenuVM()
        {
            CompleteCommand = new RelayCommand(Complete);
            CreateCommand = new RelayCommand(Create);
            RedactCommand = new RelayCommand(Redact);
            DeleteCommand = new RelayCommand(Delete);

            CurrentView = new CompleteTestVM();
        }
    }
}
