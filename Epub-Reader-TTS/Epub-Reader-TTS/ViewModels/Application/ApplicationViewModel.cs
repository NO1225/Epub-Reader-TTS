﻿using Epub_Reader_TTS.Core;
using System.Threading.Tasks;
using System.Windows.Input;

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
        /// Back button visability switch
        /// </summary>
        public bool BackVisible { get => CurrentPage != ApplicationPage.Dashboard; }

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
            CurrentPage = page;
        }

        /// <summary>
        /// Setting the colors of the application to be in dark mode or in light mode
        /// </summary>
        /// <param name="color"></param>
        public void SetDarkMode(bool isDarkMode)
        {
            // TODO: 

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
