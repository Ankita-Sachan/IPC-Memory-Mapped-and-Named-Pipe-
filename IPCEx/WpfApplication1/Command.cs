using System;
using System.Windows.Input;

namespace WpfApplication1
{
    public class Command : ICommand
    {
        private readonly Action<object> _action;

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public Command(Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _action?.Invoke(parameter);
        }
    }
}
