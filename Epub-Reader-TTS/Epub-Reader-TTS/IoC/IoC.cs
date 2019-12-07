//using Ninject;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// The IoC container for our application
    /// </summary>
    public static class IoC
    {
        #region Public Properties

        /// <summary>
        /// The kernel for our IoC container
        /// </summary>
        //public static IKernel Kernel { get; private set; } = new StandardKernel();

        #endregion

        #region Construction

        /// <summary>
        /// Sets up IoC container, binds all information required and is ready for use
        /// NOTE: Must be called as soon as your application starts up to ensure all 
        ///        service are running 
        /// </summary>
        public static void Setup()
        {
            //SettingUpFiles();

            // Bind all required view models
            BindViewModels();

            // Initiate Database
            InitiateDataBase();

            // Setup the color of the styles
            SetupColor();
        }

        private static void SetupColor()
        {
            ViewModelLocator.ApplicationViewModel.SetColor((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#242425"));
        }

        /// <summary>
        /// Binds all singleton view models
        /// </summary>
        private static void BindViewModels()
        {
            // Bind to a single instance of application view model
            //Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
        }

        /// <summary>
        /// Create database if not existed
        /// </summary>
        private static void InitiateDataBase()
        {

            //// DataBase Initializer 
            //using (DataService db = new DataService())
            //{
            //    // if doesn't exist, 
            //    if (!db.Database.Exists())
            //    {
            //        // then create new a input the default.
            //        db.Database.Initialize(true);
            //    }
            //}
        }
        #endregion

        /// <summary>
        /// Get's a service from the IoC, of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        //public static T Get<T>()
        //{
        //    return Kernel.Get<T>();
        //}

        #region Helping Methods



        #endregion

    }
}
