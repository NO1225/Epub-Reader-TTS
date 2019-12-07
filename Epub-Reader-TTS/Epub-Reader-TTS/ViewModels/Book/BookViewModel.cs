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
    public class BookViewModel : BaseViewModel
    {
        #region Private Fields

        private InstalledVoice selectedVoice;
        private int readingSpeed;
        private PageViewModel currentPage;

        #endregion

        #region Public Properties

        public Action OnFinnished;

        public bool Focused { get; set; }

        public string FilePath { get; set; }

        public string Title { get; set; }

        public int PageIndex => CurrentPage != null ? CurrentPage.Index : -1;

        public ObservableCollection<PageViewModel> PageViewModels { get; set; }

        public PageViewModel CurrentPage
        {
            get => currentPage; 
            set
            {

                if(currentPage!=null)
                {
                    TogglePause(true).GetAwaiter().GetResult();
                    CurrentPage.OnClose().GetAwaiter().GetResult();
                }

                currentPage = value;

                CurrentPage.Initiate();
            }
        }

        public string PauseButtonText { get; set; }

        public AdditionalContent CurrentAdditionalContent { get; set; }

        public bool AdditionalContentVisible { get; set; }

        public SpeechSynthesizer SpeechSynthesizer { get; set; }

        public ReadOnlyCollection<InstalledVoice> InstalledVoices { get; set; }

        public InstalledVoice SelectedVoice
        {
            get => selectedVoice;
            set
            {
                selectedVoice = value;
                TaskManager.Run(()=>SelecteVoice(selectedVoice, readingSpeed));
            }
        }

        public int ReadingSpeed
        {
            get => readingSpeed;
            set
            {
                readingSpeed = value;
                SelecteVoice(selectedVoice, readingSpeed);
            }
        }

        #endregion

        #region Commands

        public ICommand PlayCommand { get; set; }

        public ICommand StopCommand { get; set; }

        public ICommand ToggleBookmarksCommand { get; set; }

        public ICommand ToggleSettingsCommand { get; set; }


        #endregion

        #region Default Constructor

        public BookViewModel()
        {
            PlayCommand = new RelayCommand(async () => await TogglePause());

            StopCommand = new RelayCommand(async () => await Stop());

            ToggleBookmarksCommand = new RelayCommand(ToggleBookmarks);

            ToggleSettingsCommand = new RelayCommand(ToggleSettings);

            this.PageViewModels = new ObservableCollection<PageViewModel>();
            
            SpeechSynthesizer = new SpeechSynthesizer();
            // Configure the audio output.   
            SpeechSynthesizer.SetOutputToDefaultAudioDevice();

            InstalledVoices = SpeechSynthesizer.GetInstalledVoices();

            //AddPage(new PageViewModel()
            //{
            //    Focused = true,
            //    Index = 0,
            //    Title = "Chapter 1",
            //    ParagraphIndex = 0,
            //});

            //AddPage(new PageViewModel()
            //{
            //    Focused = true,
            //    Index = 1,
            //    Title = "Chapter 2",
            //    ParagraphIndex = 0,
            //});

            //AddPage(new PageViewModel()
            //{
            //    Focused = true,
            //    Index = 2,
            //    Title = "Chapter 3",
            //    ParagraphIndex = 0,
            //});

            PauseButtonText = "Play";

            //CurrentPage = PageViewModels.First();

            Initiate();

            //OnPropertyChanged(nameof(CurrentPage));
        }

        private void Initiate()
        {

            //SelectedVoice = InstalledVoices.First();

            //var a = SpeechSynthesizer.GetInstalledVoices();

            SelecteVoice(DI.SettingsManager.GetSelectedVoice(), DI.SettingsManager.GetReadingSpeed());
        }

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

        internal void Initialize(Book book)
        {
            CurrentPage = PageViewModels.First(p=>p.Index == book.CurrentPageIndex);

            CurrentPage.ParagraphIndex = book.CurrentParagraphIndex;

            CurrentPage.CurrentParagraph.Active = true;
        }

        private void SelecteVoice(InstalledVoice selectedVoice, int readingSpeed)
        {
            //TODO: Save 

            SpeechSynthesizer.SelectVoice(selectedVoice.VoiceInfo.Name);

            SpeechSynthesizer.Rate = readingSpeed;

            DI.SettingsManager.SetSelectedVoice(selectedVoice.VoiceInfo.Name);
            DI.SettingsManager.SetReadingSpeed(readingSpeed);
        }

        private void SelecteVoice(string selectedVoice, int readingSpeed)
        {
            if (string.IsNullOrEmpty(selectedVoice))
                selectedVoice = InstalledVoices.First().VoiceInfo.Name;

            SelectedVoice = InstalledVoices.First(v => v.VoiceInfo.Name == selectedVoice);

            ReadingSpeed = readingSpeed;

        }

        public BookViewModel(string filePath, int page, int paragraph)
        {
            // TODO:
            throw new Exception();
        }


        #endregion

        #region Public Methods

        public void AddPage(PageViewModel pageViewModel)
        {
            pageViewModel.OnFinnished = NextPage;

            pageViewModel.parent = this;

            this.PageViewModels.Add(pageViewModel);
        }

        public void NextPage(int currentPage)
        {
            var page = PageViewModels.FirstOrDefault(p => p.Index == currentPage + 1);

            if (page != null)
            {
                CurrentPage = page;
                TogglePause().GetAwaiter().GetResult();

                TaskManager.Run(async () => ViewModelApplication.SavePosition(CurrentPage.Index, 0));

            }
            else
                Finnished();
        }


        #endregion

        #region Private Methods

        private void Finnished()
        {
            if (OnFinnished != null)
                OnFinnished();
        }


        private async Task Stop()
        {
            //await CurrentPage.StopReading();
        }

        private async Task TogglePause(bool forcePause = false)
        {
            await CurrentPage.TogglePause(forcePause);
            Debug.WriteLine(CurrentPage.IsReading);
            PauseButtonText = CurrentPage.IsReading ? "Pause" : "Resume";

        }


        #endregion
    }
}
