using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Epub_Reader_TTS
{

    /// <summary>
    /// A base view model that fires Property Changed events as needed
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {

        #region Public Properties

        /// <summary>
        /// It gives indication if this viewmodel has any validation errors
        /// </summary>
        public bool HasErrors { get { return _errors.Any(propErrors => propErrors.Value != null && propErrors.Value.Count > 0); } }

        /// <summary>
        /// If the state of this viewmodel in respect o validation
        /// </summary>
        public bool IsValid { get { return !this.HasErrors; } }

        #endregion

        #region Private Fields

        /// <summary>
        /// List of error resulting from the validation
        /// </summary>
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        /// <summary>
        /// Lock object to be used in thread locking 
        /// </summary>
        private object _lock = new object();


        #endregion

        #region Events

        /// <summary>
        /// The event that is fired when any child property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Event hundler to fire when the error states of this viewmodel changed
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #endregion

        #region Public Methods

        /// <summary>
        /// Call this to fire a <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name)
        {
            var a = GetPropValue(this, name);
            PropertyChanged(this, new PropertyChangedEventArgs(name));
            ValidateProperty(a, name);
        }

        /// <summary>
        /// Check the error and validate the choes property
        /// </summary>
        /// <param name="propertyName">the property to be validated</param>
        /// <returns>the errors of this validation</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (_errors.ContainsKey(propertyName) && (_errors[propertyName] != null) && _errors[propertyName].Count > 0)
                    return _errors[propertyName].ToList();
                else
                    return null;
            }
            else
                return _errors.SelectMany(err => err.Value.ToList());
        }

        /// <summary>
        /// The event to be fired if the error state of this view model changed
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Validate the chosen property
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        public void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            lock (_lock)
            {
                var validationContext = new ValidationContext(this, null, null);
                validationContext.MemberName = propertyName;
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateProperty(value, validationContext, validationResults);

                //clear previous _errors from tested property  
                if (_errors.ContainsKey(propertyName))
                    _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
                HandleValidationResults(validationResults);
            }
        }

        /// <summary>
        /// Validate all the properties
        /// </summary>
        public void Validate()
        {
            lock (_lock)
            {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                //clear all previous _errors  
                var propNames = _errors.Keys.ToList();
                _errors.Clear();
                propNames.ForEach(pn => OnErrorsChanged(pn));
                HandleValidationResults(validationResults);
            }
        }

        #endregion

        #region Command Helpers

        /// <summary>
        /// Runs a command if the updating flag is not set.
        /// If the flag is true (indicating the function is already running) then the action is not run.
        /// If the flag is false (indicating no running function) then the action is run.
        /// Once the action is finished if it was run, then the flag is reset to false
        /// </summary>
        /// <param name="updatingFlag">The boolean property flag defining if the command is already running</param>
        /// <param name="action">The action to run if the command is not already running</param>
        /// <returns></returns>
        protected async Task RunCommandAsync(Expression<Func<bool>> updatingFlag, Func<Task> action)
        {
            // Check if the flag property is true (meaning the function is already running)
            if (updatingFlag.GetPropertyValue())
                return;

            // Set the property flag to true to indicate we are running
            updatingFlag.SetPropertyValue(true);

            try
            {
                // Run the passed in action
                await action();
            }
            finally
            {
                // Set the property flag back to false now it's finished
                updatingFlag.SetPropertyValue(false);
            }
        }

        #endregion

        #region HelperMethod

        /// <summary>
        /// Get the value of a property its name 
        /// </summary>
        /// <param name="obj">The object that host that property</param>
        /// <param name="name">the name of the property</param>
        /// <returns></returns>
        private Object GetPropValue(Object obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        /// <summary>
        /// Arrange the validation result and group the errors in the error property of this viewmodel
        /// </summary>
        /// <param name="validationResults"></param>
        private void HandleValidationResults(List<ValidationResult> validationResults)
        {
            //Group validation results by property names  
            var resultsByPropNames = from res in validationResults
                                     from mname in res.MemberNames
                                     group res by mname into g
                                     select g;
            //add _errors to dictionary and inform binding engine about _errors  
            foreach (var prop in resultsByPropNames)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();
                _errors.Add(prop.Key, messages);
                OnErrorsChanged(prop.Key);
            }
        }

        #endregion
    }

}
