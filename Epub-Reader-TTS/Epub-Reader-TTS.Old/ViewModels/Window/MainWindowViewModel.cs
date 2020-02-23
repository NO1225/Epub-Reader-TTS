using System.Windows;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Viewmodel to manage the main winddow
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        #region Private fields

        /// <summary>
        /// The mainwindow
        /// </summary>
        private Window _window;

        #endregion

        #region Default Constructor

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="window"></param>
        public MainWindowViewModel(Window window)
        {
            this._window = window;
        }

        #endregion

    }
}
