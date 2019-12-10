using Epub_Reader_TTS.Core;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {

        #region Public Properties

        /// <summary>
        /// The current page of the applcation 
        /// </summary>
        public ApplicationPage CurrentPage { get; private set; }

        /// <summary>
        /// Place holder for the viewmodel of the displayed book
        /// </summary>
        public BookViewModel CurrentBookViewModel { get; set; }

        /// <summary>
        /// The model of the currently display book
        /// </summary>
        public Book CurrentBook { get; set; }

        /// <summary>
        /// The logo of the application
        /// </summary>
        public byte[] Logo
        {
            get;
            private set;
        }// = Properties.Resources.Logo.ToByteArray(true);


        /// <summary>
        /// Colors properties
        /// </summary>
        #region Colors

        //public Color MainColor { get; set; }

        //public Brush MainColorBrush { get; set; }

        //public Color MainHoverColor { get; set; }

        //public Brush MainHoverColorBrush { get; set; }

        //public Color MainPressedColor { get; set; }

        //public Brush MainPressedColorBrush { get; set; }

        #endregion

        #endregion


        #region Default Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationViewModel()
        {
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Navigates to the specified page
        /// </summary>
        /// <param name="page">The page to go to</param>
        public void GoToPage(ApplicationPage page)
        {
            CurrentPage = page;
        }

        /// <summary>
        /// Setting the colors of the application to be in dark mode or in light mode
        /// </summary>
        /// <param name="color"></param>
        public void SetDarkMode(bool isDarkMode)
        {

            if(isDarkMode)
            {
                var primaryFontColor = ColorTranslator.FromHtml("#ffffff").ToMediaColor();
                var secondaryFontColor = ColorTranslator.FromHtml("#f0f0f0").ToMediaColor();
                var primaryBackGroundColor = ColorTranslator.FromHtml("#000000").ToMediaColor();
                var secondaryBackGroundColor = ColorTranslator.FromHtml("#2b2b2b").ToMediaColor();

                var primaryFontColorBrush = new SolidColorBrush(primaryFontColor);
                var secondaryFontColorBrush = new SolidColorBrush(secondaryFontColor);
                var primaryBackGroundColorBrush = new SolidColorBrush(primaryBackGroundColor);
                var secondaryBackGroundColorBrush = new SolidColorBrush(secondaryBackGroundColor);

                Application.Current.Resources["PrimaryFontColor"] = primaryFontColor;
                Application.Current.Resources["SecondaryFontColor"] = secondaryFontColor;
                Application.Current.Resources["PrimaryBackGroundColor"] = primaryBackGroundColor;
                Application.Current.Resources["SecondaryBackGroundColor"] = secondaryBackGroundColor;

                Application.Current.Resources["PrimaryFontColorBrush"] = primaryFontColorBrush;
                Application.Current.Resources["SecondaryFontColorBrush"] = secondaryFontColorBrush;
                Application.Current.Resources["PrimaryBackGroundColorBrush"] = primaryBackGroundColorBrush;
                Application.Current.Resources["SecondaryBackGroundColorBrush"] = secondaryBackGroundColorBrush;
            }
            else
            {
                var primaryFontColor = ColorTranslator.FromHtml("#000000").ToMediaColor();
                var secondaryFontColor = ColorTranslator.FromHtml("#2b2b2b").ToMediaColor();
                var primaryBackGroundColor = ColorTranslator.FromHtml("#ffffff").ToMediaColor();
                var secondaryBackGroundColor = ColorTranslator.FromHtml("#f0f0f0").ToMediaColor();

                var primaryFontColorBrush = new SolidColorBrush(primaryFontColor);
                var secondaryFontColorBrush = new SolidColorBrush(secondaryFontColor);
                var primaryBackGroundColorBrush = new SolidColorBrush(primaryBackGroundColor);
                var secondaryBackGroundColorBrush = new SolidColorBrush(secondaryBackGroundColor);

                Application.Current.Resources["PrimaryFontColor"] = primaryFontColor;
                Application.Current.Resources["SecondaryFontColor"] = secondaryFontColor;
                Application.Current.Resources["PrimaryBackGroundColor"] = primaryBackGroundColor;
                Application.Current.Resources["SecondaryBackGroundColor"] = secondaryBackGroundColor;

                Application.Current.Resources["PrimaryFontColorBrush"] = primaryFontColorBrush;
                Application.Current.Resources["SecondaryFontColorBrush"] = secondaryFontColorBrush;
                Application.Current.Resources["PrimaryBackGroundColorBrush"] = primaryBackGroundColorBrush;
                Application.Current.Resources["SecondaryBackGroundColorBrush"] = secondaryBackGroundColorBrush;
            }

            DI.TaskManager.Run(() => DI.SettingsManager.SetDarkMode(isDarkMode));
        }

        /// <summary>
        /// Save the location of the reading of this book in the database 
        /// </summary>
        /// <param name="pageIndex">Current page</param>
        /// <param name="paragraphIndex">Current paragraph</param>
        /// <returns></returns>
        internal async Task SavePosition(int pageIndex, int paragraphIndex)
        {
            this.CurrentBook.CurrentPageIndex = pageIndex;
            this.CurrentBook.CurrentParagraphIndex = paragraphIndex;

            await DI.ClientDataStore.AddBook(this.CurrentBook);
        }

        #endregion
        
    }
}
