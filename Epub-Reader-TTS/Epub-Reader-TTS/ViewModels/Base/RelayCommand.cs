using System;
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

        /// <summary>
        /// The function to be run to check if this command is able to be excuted
        /// </summary>
        Func<bool> mTargetCanExecuteMethod;

        #endregion

        #region Public Events

        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        /// <summary>
        /// Used to raise the event to check if this command can be excuted
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RelayCommand(Action action)
        {
            mAction = action;
        }

        /// <summary>
        /// Constructor with the ability to add an action to check if this command can be excuted
        /// </summary>
        /// <param name="executeMethod"></param>
        /// <param name="canExecuteMethod"></param>
        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            mAction = executeMethod;
            mTargetCanExecuteMethod = canExecuteMethod;
        }


        #endregion

        #region Command Methods

        /// <summary>
        /// Run the action stored as the can excution function
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Run the action stored in this command
        /// </summary>
        /// <param name="parameter"></param>
        void ICommand.Execute(object parameter)
        {
            if (mAction != null)
            {
                mAction();
            }
        }

        #endregion
    }
}
