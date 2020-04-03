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
        /// The title of the application
        /// </summary>
        public string Title => CurrentBook == null ? "Epub-Reader-TTS" : $"Epub-Reader-TTS - {CurrentBook.BookName}";

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

            if (page == ApplicationPage.Dashboard)
                CurrentBook = null;
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
