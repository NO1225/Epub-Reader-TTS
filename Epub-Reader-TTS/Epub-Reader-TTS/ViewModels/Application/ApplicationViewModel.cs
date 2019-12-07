using Epub_Reader_TTS.Core;
using System;
using System.Threading.Tasks;
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
        public ApplicationPage CurrentPage
        {
            get;
            private set;
        } = ApplicationPage.Dashboard;

        public BookViewModel CurrentBookViewModel { get; set; }

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
        /// Back button visability switch
        /// </summary>
        public bool BackVisible { get => CurrentPage != ApplicationPage.Dashboard; }



        /// <summary>
        /// Colors properties
        /// </summary>
        #region Colors

        public Color MainColor { get; set; }

        public Brush MainColorBrush { get; set; }

        public Color MainHoverColor { get; set; }

        public Brush MainHoverColorBrush { get; set; }

        public Color MainPressedColor { get; set; }

        public Brush MainPressedColorBrush { get; set; }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to go back to the dashboard
        /// </summary>
        public ICommand BackCommand { get; set; }

        #endregion

        #region Default Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationViewModel()
        {
            BackCommand = new RelayCommand(() => BackToHome());
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Go back to the dashboard page
        /// </summary>
        private void BackToHome()
        {
            GoToPage(ApplicationPage.Dashboard);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Navigates to the specified page
        /// </summary>
        /// <param name="page">The page to go to</param>
        public void GoToPage(ApplicationPage page)
        {
            //switch (page)
            //{
            //    case ApplicationPage.Members:
            //        if (!HRAccessLevel)
            //            return;
            //        break;
            //    case ApplicationPage.Custmers:
            //        if (!HRAccessLevel)
            //            return;
            //        break;
            //    case ApplicationPage.ESCoin:
            //        if (!HRAccessLevel)
            //            return;
            //        break;
            //    case ApplicationPage.Backups:
            //        if (!HRAccessLevel)
            //            return;
            //        break;
            //    case ApplicationPage.Satistics:
            //        if (!AdminAccessLevel)
            //            return;
            //        break;
            //    default:
            //        break;
            //}
            CurrentPage = page;
        }

        /// <summary>
        /// Setting the main color in the resources of our programm
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            //MainColor = color;
            //MainHoverColor = color.ChangeBrightness(Constants.BrightnessFactor);
            //MainPressedColor = color.ChangeBrightness(-Constants.BrightnessFactor / 2);

            //MainColorBrush = new SolidColorBrush(MainColor);
            //MainHoverColorBrush = new SolidColorBrush(MainHoverColor);
            //MainPressedColorBrush = new SolidColorBrush(MainPressedColor);

            //Application.Current.Resources["MainColor"] = MainColor;
            //Application.Current.Resources["MainHoverColor"] = MainHoverColor;
            //Application.Current.Resources["MainPressedColor"] = MainPressedColor;

            //Application.Current.Resources["MainColorBrush"] = MainColorBrush;
            //Application.Current.Resources["MainHoverColorBrush"] = MainHoverColorBrush;
            //Application.Current.Resources["MainPressedColorBrush"] = MainPressedColorBrush;

        }

        internal async Task SavePosition(int pageIndex, int paragraphIndex)
        {
            this.CurrentBook.CurrentPageIndex = pageIndex;
            this.CurrentBook.CurrentParagraphIndex = paragraphIndex;

            await DI.ClientDataStore.AddBook(this.CurrentBook);
        }

        #endregion


    }
}
