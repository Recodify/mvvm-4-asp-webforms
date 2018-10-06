using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Binding.Helpers
{
    
    public class DelegateCommand : ICommand
    {

    
        private Action<object> execute = null;
        private Func<object, bool> canExecute = null;
        private bool autoRaiseChanges;

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute, bool autoRaiseChanges) : this(execute,canExecute)
        {
            this.autoRaiseChanges = autoRaiseChanges;
        }
        
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return this.canExecute.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this.execute.Invoke(parameter);
            
            if (this.autoRaiseChanges)
                this.RaiseCanExecuteChanged();
        }


        public void RaiseCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
