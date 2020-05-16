using Epub_Reader_TTS.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using static Epub_Reader_TTS.DI;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Viewmodel to store all the detail of the book
    /// </summary>
    public class BookViewModel : BaseViewModel
    {
        #region Private Fields

        /// <summary>
        /// The current page
        /// </summary>
        private PageViewModel currentPage;

        /// <summary>
        /// detrmine if the book is currently being read to aid with page transitions 
        /// </summary>
        private bool reading;

        #endregion

        #region Public Properties

        /// <summary>
        /// Action place holder to be fired when the book is finneshed
        /// </summary>
        public Action OnFinnished;

        /// <summary>
        /// The title of this book
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Collection of all the pages of this book
        /// </summary>
        public ObservableCollection<PageViewModel> PageViewModels { get; set; }

        /// <summary>
        /// The current displayed book 
        /// </summary>
        public PageViewModel CurrentPage
        {
            get => currentPage;
            set
            {
                reading = false;

                if (currentPage != null)
                {
                    reading = currentPage.IsReading;

                    currentPage.OnClose();
                }
                if (value != null)
                    value.SortParagraphs();
                currentPage = value;
                OnPropertyChanged(nameof(PauseButtonText));

                DI.SpeechSynthesizer.UpdateSystemMediaTrasportControls(
                    Title,
                    CurrentPage.Title,
                    mediaPlaybackStatus: CurrentPage.IsReading ? MediaPlaybackStatus.Playing : MediaPlaybackStatus.Paused);
            }
        }

        /// <summary>
        /// THe text to be displayed in the pause button
        /// </summary>
        public string PauseButtonText
        {
            get
            {
                if (CurrentPage != null)
                {
                    return CurrentPage.IsReading ? "\uf04c" : "\uf04b";
                }
                return reading ? "\uf04c" : "\uf04b";
            }
        }

        /// <summary>
        /// The type of the popup to be displayed
        /// </summary>
        public AdditionalContent CurrentAdditionalContent { get; set; }

        /// <summary>
        /// Show the pop up
        /// </summary>
        public bool AdditionalContentVisible { get; set; }

        #region UI Properties

        private double actualHeight = 501;

        private double actualWidth = 784;

        /// <summary>
        /// The actual height of the page container
        /// </summary>
        public double ActualHeight
        {
            get => actualHeight; set
            {
                actualHeight = value;

                SortCurrent();
            }
        }

        /// <summary>
        /// The actual width of the page container
        /// </summary>
        public double ActualWidth
        {
            get => actualWidth; set
            {
                actualWidth = value;

                SortCurrent();
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to start reading or toggle pause resume...
        /// </summary>
        public ICommand PlayCommand { get; set; }

        public ICommand RepeatCommand { get; set; }

        public ICommand PreviousParagraphCommand { get; set; }

        public ICommand NextParagraphCommand { get; set; }

        public ICommand PreviousPageCommand { get; set; }

        public ICommand NextPageCommand { get; set; }

        public ICommand CloseBookCommand { get; set; }

        /// <summary>
        /// Command to show the bookmarks popup
        /// </summary>
        public ICommand ToggleBookmarksCommand { get; set; }

        /// <summary>
        /// Command to show the settings popup
        /// </summary>
        public ICommand ToggleSettingsCommand { get; set; }

        /// <summary>
        /// Command to hide any popup
        /// </summary>
        public ICommand HidePopUpCommand { get; set; }

        #endregion

        #region Default Constructor

        /// <summary>
        /// The default constructor
        /// </summary>
        public BookViewModel()
        {
            PlayCommand = new RelayCommand(async () => await TogglePause());

            RepeatCommand = new RelayCommand(async () => await RepeatParagraph());

            PreviousParagraphCommand = new RelayCommand(async () => await PreviousParagraph());

            NextParagraphCommand = new RelayCommand(async () => await NextParagraph());

            PreviousPageCommand = new RelayCommand(() => PreviousPage());

            NextPageCommand = new RelayCommand(() => NextPage(CurrentPage.Index));

            CloseBookCommand = new RelayCommand(async () => await CloseBook());

            ToggleBookmarksCommand = new RelayCommand(ToggleBookmarks);

            ToggleSettingsCommand = new RelayCommand(ToggleSettings);

            HidePopUpCommand = new RelayCommand(() => AdditionalContentVisible = false);

            Initiate();
        }


        #endregion

        #region Initiation

        /// <summary>
        /// The default initiation
        /// </summary>
        private void Initiate()
        {
            OnPropertyChanged(nameof(PauseButtonText));

            DI.SpeechSynthesizer.PlayPressed = () => TogglePause();
            DI.SpeechSynthesizer.PausePressed = () => TogglePause();
            DI.SpeechSynthesizer.NextPressed = () => NextParagraph();
            DI.SpeechSynthesizer.PreviousPressed = () => PreviousParagraph();

        }

        /// <summary>
        /// Initiate and go to the last saved position of this book
        /// </summary>
        /// <param name="book"></param>
        public void Initialize(Book book)
        {
            CurrentPage = PageViewModels.First(p => p.Index == book.CurrentPageIndex);

            CurrentPage.Initiate(reading, book.CurrentParagraphIndex);

            OnPropertyChanged(nameof(PauseButtonText));

            DI.SpeechSynthesizer.UpdateSystemMediaTrasportControls(
                Title,
                CurrentPage.Title,
                ViewModelApplication.CurrentBook.BookCoverPath,
                mediaPlaybackStatus: CurrentPage.IsReading ? MediaPlaybackStatus.Playing : MediaPlaybackStatus.Paused);

        }


        #endregion

        #region Commands Methods

        /// <summary>
        /// Start reading or toggle pause/resume ... 
        /// </summary>
        /// <param name="forcePause">Force pausing </param>
        /// <returns></returns>
        private async Task TogglePause(bool forcePause = false)
        {
            if (CurrentPage.CurrentParagraph == null)
                CurrentPage.Initiate();

            await CurrentPage.TogglePause(forcePause);

            DI.SpeechSynthesizer.UpdateSystemMediaTrasportControls(
                Title,
                CurrentPage.Title,
                mediaPlaybackStatus: CurrentPage.IsReading ? MediaPlaybackStatus.Playing : MediaPlaybackStatus.Paused);

            OnPropertyChanged(nameof(PauseButtonText));
        }

        /// <summary>
        /// Show/Hide the settings popup
        /// </summary>
        private void ToggleSettings()
        {
            if (CurrentAdditionalContent == AdditionalContent.Settings)
            {
                AdditionalContentVisible = !AdditionalContentVisible;
            }
            else
            {
                CurrentAdditionalContent = AdditionalContent.Settings;
                AdditionalContentVisible = true;
            }
        }

        /// <summary>
        /// Show/Hide the bookmarks popup
        /// </summary>
        private void ToggleBookmarks()
        {
            if (CurrentAdditionalContent == AdditionalContent.Bookmarks)
            {
                AdditionalContentVisible = !AdditionalContentVisible;
            }
            else
            {
                CurrentAdditionalContent = AdditionalContent.Bookmarks;
                AdditionalContentVisible = true;
            }
        }

        /// <summary>
        /// Close the book and return to the dashboard page
        /// </summary>
        /// <returns></returns>
        private async Task CloseBook()
        {
            await CurrentPage.TogglePause(true);

            ViewModelApplication.GoToPage(ApplicationPage.Dashboard);
        }

        /// <summary>
        /// tell the page to repeat the current paragraph
        /// </summary>
        /// <returns></returns>
        private async Task RepeatParagraph()
        {
            await CurrentPage.RepeatParagraph();

            OnPropertyChanged(nameof(PauseButtonText));
        }

        /// <summary>
        /// tell the page to jump to the next paragraph
        /// </summary>
        /// <returns></returns>
        private async Task NextParagraph()
        {
            await CurrentPage.GoToNextParagraph();

            OnPropertyChanged(nameof(PauseButtonText));
        }

        /// <summary>
        /// tell the page to step back to the previous paragraph
        /// </summary>
        /// <returns></returns>
        private async Task PreviousParagraph()
        {
            await CurrentPage.GoToPreviousParagraph();

            OnPropertyChanged(nameof(PauseButtonText));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sort the current page paragraphs
        /// </summary>
        public void SortCurrent()
        {
            if (CurrentPage != null && actualWidth != 0 && actualHeight != 0)
                CurrentPage.SortParagraphs();
        }

        /// <summary>
        /// Add a page to this book
        /// </summary>
        /// <param name="pageViewModel"></param>
        public void AddPage(PageViewModel pageViewModel)
        {
            if (this.PageViewModels == null)
                this.PageViewModels = new ObservableCollection<PageViewModel>();

            pageViewModel.OnFinnished = NextPage;
            pageViewModel.OnPreviousPage = PriviousPage;

            pageViewModel.parent = this;

            this.PageViewModels.Add(pageViewModel);
        }

        /// <summary>
        /// Go to the next page
        /// </summary>
        /// <param name="currentPage">the index of the current page</param>
        public void NextPage(int currentPage)
        {
            var page = PageViewModels.FirstOrDefault(p => p.Index == currentPage + 1);

            if (page != null)
            {
                CurrentPage = page;

                CurrentPage.Initiate(reading);

                DI.SpeechSynthesizer.UpdateSystemMediaTrasportControls(
                    Title,
                    CurrentPage.Title,
                    mediaPlaybackStatus: CurrentPage.IsReading ? MediaPlaybackStatus.Playing : MediaPlaybackStatus.Paused);

                OnPropertyChanged(nameof(PauseButtonText));
            }
            else
                Finnished();
        }

        /// <summary>
        /// Go to the previous page and focus the last paragraph of that page
        /// </summary>
        /// <param name="currentPage">the index of the current page</param>
        public void PriviousPage(int currentPage)
        {
            var page = PageViewModels.FirstOrDefault(p => p.Index == currentPage - 1);

            if (page != null)
            {
                CurrentPage = page;

                CurrentPage.Initiate(reading, CurrentPage.ParagraphViewModels.Last().Index);

                DI.SpeechSynthesizer.UpdateSystemMediaTrasportControls(
                    Title,
                    CurrentPage.Title,
                    mediaPlaybackStatus: CurrentPage.IsReading ? MediaPlaybackStatus.Playing : MediaPlaybackStatus.Paused);

                OnPropertyChanged(nameof(PauseButtonText));
            }
        }

        /// <summary>
        /// Go to the start of the previous page
        /// </summary>
        /// <param name="currentPage">the index of the current page</param>
        public void PreviousPage()
        {
            var page = PageViewModels.FirstOrDefault(p => p.Index == CurrentPage.Index - 1);

            if (page != null)
            {
                CurrentPage = page;

                CurrentPage.Initiate(reading);
            }

            OnPropertyChanged(nameof(PauseButtonText));
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// To be fired when the book is finnished
        /// </summary>
        private void Finnished()
        {
            if (OnFinnished != null)
                OnFinnished();
        }

        #endregion

    }
}
