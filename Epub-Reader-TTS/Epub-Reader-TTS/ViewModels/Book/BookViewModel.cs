using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Epub_Reader_TTS.Core;
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
        /// The selected voice
        /// </summary>
        private InstalledVoice selectedVoice;

        /// <summary>
        /// The reading speed
        /// </summary>
        private int readingSpeed;

        /// <summary>
        /// The current page
        /// </summary>
        private PageViewModel currentPage;

        /// <summary>
        /// Detrmine if the book is currently being read to aid with page transitions 
        /// </summary>
        private bool reading;


        private bool isDarkMode;

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
        /// The index of the current opge
        /// </summary>
        public int PageIndex => CurrentPage != null ? CurrentPage.Index : -1;

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
                reading=false;

                if(currentPage!=null)
                {
                    reading = currentPage.IsReading;
                    //TogglePause(true).GetAwaiter().GetResult();
                    CurrentPage.OnClose().GetAwaiter().GetResult();
                }

                currentPage = value;

                currentPage.Initiate(reading);

            }
        }

        /// <summary>
        /// THe text to be displayed in the pause button
        /// </summary>
        public string PauseButtonText { get; set; }

        /// <summary>
        /// The type of the popup to be displayed
        /// </summary>
        public AdditionalContent CurrentAdditionalContent { get; set; }

        /// <summary>
        /// Show the pop up
        /// </summary>
        public bool AdditionalContentVisible { get; set; }

        /// <summary>
        /// The tool responsable of speaking 
        /// </summary>
        public SpeechSynthesizer SpeechSynthesizer { get; set; }

        /// <summary>
        /// List of all the installed voice on this pc
        /// </summary>
        public ReadOnlyCollection<InstalledVoice> InstalledVoices { get; set; }

        /// <summary>
        /// The selected voice to be used in the reading 
        /// </summary>
        public InstalledVoice SelectedVoice
        {
            get => selectedVoice;
            set
            {
                selectedVoice = value;
                TaskManager.Run(()=>UpdateSelectedVoice(selectedVoice, readingSpeed));
            }
        }

        /// <summary>
        /// The reading speed 
        /// </summary>
        public int ReadingSpeed
        {
            get => readingSpeed;
            set
            {
                readingSpeed = value;
                UpdateSelectedVoice(selectedVoice, readingSpeed);
            }
        }


        public bool IsDarkMode
        {
            get { return isDarkMode; }
            set { 
                isDarkMode = value;
                ViewModelApplication.SetDarkMode(IsDarkMode);
            }
        }

        private int fontSize;

        public int FontSize
        {
            get { return fontSize; }
            set { 
                fontSize = value;
                ViewModelApplication.SetFontSize(FontSize);
            }
        }


        #endregion

        #region Commands

        /// <summary>
        /// Command to start reading or toggle pause resume...
        /// </summary>
        public ICommand PlayCommand { get; set; }

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

        public ICommand HidePopUpCommand { get; set; }

        #endregion

        #region Default Constructor

        /// <summary>
        /// The default constructor
        /// </summary>
        public BookViewModel()
        {
            PlayCommand = new RelayCommand(async () => await TogglePause());

            PreviousParagraphCommand = new RelayCommand(async () => await PreviousParagraph());

            NextParagraphCommand = new RelayCommand(async () => await NextParagraph());     
            
            PreviousPageCommand = new RelayCommand(() => PriviousPage());

            NextPageCommand = new RelayCommand(() => NextPage(CurrentPage.Index));

            CloseBookCommand = new RelayCommand(async () => await CloseBook());

            ToggleBookmarksCommand = new RelayCommand(ToggleBookmarks);

            ToggleSettingsCommand = new RelayCommand(ToggleSettings);

            HidePopUpCommand = new RelayCommand(() => AdditionalContentVisible = false);

            this.PageViewModels = new ObservableCollection<PageViewModel>();
            
            Initiate();
        }

        #endregion

        #region Initiation

        /// <summary>
        /// The default initiation
        /// </summary>
        private void Initiate()
        {
            IsDarkMode = DI.SettingsManager.IsDarkMode();

            FontSize = DI.SettingsManager.GetFontSize();

            SpeechSynthesizer = new SpeechSynthesizer();

            SpeechSynthesizer.SetOutputToDefaultAudioDevice();

            InstalledVoices = SpeechSynthesizer.GetInstalledVoices();

            PauseButtonText = "\uf04b";

            SetSelectedVoice(DI.SettingsManager.GetSelectedVoice(), DI.SettingsManager.GetReadingSpeed());
        }

        /// <summary>
        /// Initiate and go to the last saved position of this book
        /// </summary>
        /// <param name="book"></param>
        internal void Initialize(Book book)
        {
            CurrentPage = PageViewModels.First(p => p.Index == book.CurrentPageIndex);
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
            Debug.WriteLine(CurrentPage.IsReading);
            PauseButtonText = CurrentPage.IsReading ? "\uf04c" : "\uf04b";

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

        private async Task CloseBook()
        {
            await CurrentPage.TogglePause(true);

            ViewModelApplication.GoToPage(ApplicationPage.Dashboard);
        }

        private async Task NextParagraph()
        {
            await CurrentPage.GoToNextParagraph();
        }

        private async Task PreviousParagraph()
        {
            await CurrentPage.GoToPreviousParagraph();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a page to this book
        /// </summary>
        /// <param name="pageViewModel"></param>
        public void AddPage(PageViewModel pageViewModel)
        {
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
            }
            else
                Finnished();
        }     
        
        /// <summary>
        /// Go to the next page and focus the last paragraph of that page
        /// </summary>
        /// <param name="currentPage">the index of the current page</param>
        public void PriviousPage(int currentPage)
        {
            var page = PageViewModels.FirstOrDefault(p => p.Index == currentPage - 1);

            if (page != null)
            {
                CurrentPage = page;

                CurrentPage.SelectParagraph(CurrentPage.ParagraphViewModels.Last().Index);
            }
        }    
        
        /// <summary>
        /// Go to the start of the previous page
        /// </summary>
        /// <param name="currentPage">the index of the current page</param>
        public void PriviousPage()
        {
            var page = PageViewModels.FirstOrDefault(p => p.Index == CurrentPage.Index - 1);

            if (page != null)
            {
                CurrentPage = page;
            }
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

        /// <summary>
        /// Select voice based on the installed voice
        /// </summary>
        /// <param name="selectedVoice"></param>
        /// <param name="readingSpeed"></param>
        private void UpdateSelectedVoice(InstalledVoice selectedVoice, int readingSpeed)
        {
            SpeechSynthesizer.SelectVoice(selectedVoice.VoiceInfo.Name);

            SpeechSynthesizer.Rate = readingSpeed;

            DI.SettingsManager.SetSelectedVoice(selectedVoice.VoiceInfo.Name);
            DI.SettingsManager.SetReadingSpeed(readingSpeed);
        }

        /// <summary>
        /// Set the selected voice 
        /// </summary>
        /// <param name="selectedVoice"></param>
        /// <param name="readingSpeed"></param>
        private void SetSelectedVoice(string selectedVoice, int readingSpeed)
        {
            if (string.IsNullOrEmpty(selectedVoice))
                selectedVoice = InstalledVoices.First().VoiceInfo.Name;

            SelectedVoice = InstalledVoices.First(v => v.VoiceInfo.Name == selectedVoice);

            ReadingSpeed = readingSpeed;
        }

        #endregion

    }
}
