using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// a basic command that runs an aciotn
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Private Members

        /// <summary>
        /// The action to run
        /// </summary>
        private Action mAction;

        Func<bool> mTargetCanExecuteMethod;

        #endregion

        #region Public Events

        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RelayCommand(Action action)
        {
            mAction = action;
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            mAction = executeMethod;
            mTargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Command Methods

        bool ICommand.CanExecute(object parameter)
        {
            if (mTargetCanExecuteMethod != null)
            {
                return mTargetCanExecuteMethod();
            }
            if (mAction != null)
            {
                return true;
            }
            return false;
        }


        void ICommand.Execute(object parameter)
        {
            if (mAction != null)
            {
                mAction();
            }
        }


        ///// <summary>
        ///// A relay command can always execute
        ///// </summary>
        ///// <param name="parameter"></param>
        ///// <returns></returns>
        //public bool CanExecute(object parameter)
        //{
        //    return true;
        //}

        ///// <summary>
        ///// Executes the commands Action
        ///// </summary>
        ///// <param name="parameter"></param>
        //public void Execute(object parameter)
        //{
        //    mAction();
        //}

        #endregion
    }
}
