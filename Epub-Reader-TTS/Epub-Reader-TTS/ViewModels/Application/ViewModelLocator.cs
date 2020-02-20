namespace Epub_Reader_TTS
{
    /// <summary>
    /// Locates view models from the IoC for use in binding in Xaml files
    /// </summary>
    public class ViewModelLocator
    {
        #region Public Properties

        /// <summary>
        /// Singleton instance of the locator
        /// </summary>
        public static ViewModelLocator Instance = new ViewModelLocator();

        /// <summary>
        /// The application view model
        /// </summary>
        public static ApplicationViewModel ApplicationViewModel => DI.ViewModelApplication;

        /// <summary>
        /// The dashboard view model
        /// </summary>
        public static DashboardViewModel DashboardViewModel => DI.ViewModelDashboard;

        #endregion
    }
}
