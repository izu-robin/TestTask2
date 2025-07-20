using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestTask2.ViewModels
{
    class RelayCommand : ICommand
    {
        //Fields
        private readonly Action<object> _execute; //делегат для передачи метода как параметра
        private readonly Func<object, bool> _canExecute; //проверка а можно ли вообще выполнять метод

        //выделить нужные поля, по ним пр.к.мыши и рефакторинг - создать конструктор с этими полями, классный снипет
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);


        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
