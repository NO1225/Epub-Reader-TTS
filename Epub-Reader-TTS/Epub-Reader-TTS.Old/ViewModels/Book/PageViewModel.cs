using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using static Epub_Reader_TTS.DI;


namespace Epub_Reader_TTS
{
    /// <summary>
    /// Viewmodel to store all the details of this page
    /// </summary>
    public class PageViewModel : BaseViewModel
    {
        #region Private Fields
        
        private ParagraphViewModel currentParagraph;
        
        #endregion

        #region Public Properties

        /// <summary>
        /// The containing book of this page
        /// </summary>
        public BookViewModel parent;

        /// <summary>
        /// Action to be stored to be fired when the reading of this page is finnished
        /// </summary>
        public Action<int> OnFinnished;

        public Action<int> OnPreviousPage;

        /// <summary>
        /// The index of this page
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The title of this page
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The list of all the paragragh of this page
        /// </summary>
        public ObservableCollection<ParagraphViewModel> ParagraphViewModels { get; set; }

        /// <summary>
        /// The current active paragraph 
        /// </summary>
        public ParagraphViewModel CurrentParagraph
        {
            get => currentParagraph; 
            set
            {
                if (currentParagraph != null)
                    currentParagraph.Active = false;

                currentParagraph = value;

                if (IsReading)
                    StartReading().GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// If the applicaiton is reading 
        /// </summary>
        public bool IsReading { get; set; }

        #endregion

        #region Default Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PageViewModel()
        {
            this.ParagraphViewModels = new ObservableCollection<ParagraphViewModel>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initiate this page
        /// </summary>
        /// <param name="reading">weither if the application is reading or not</param>
        public void Initiate(bool reading = false,int paragraphIndex = 0)
        {
            //if (CurrentParagraph == null)
            //    CurrentParagraph = ParagraphViewModels.First();
            
            SelectParagraph(paragraphIndex);

            if (reading)
                StartReading();
        }

        /// <summary>
        /// Start reading or toggle between pause play ... 
        /// </summary>
        /// <param name="forcePause"></param>
        /// <returns></returns>
        public async Task TogglePause(bool forcePause)
        {
            if (forcePause)
            {
                await StopReading();
                return;
            }

            if (IsReading)
            {
                await StopReading();
            }
            else
            {
                await StartReading();
            }


        }

        /// <summary>
        /// Add paragraph to this page
        /// </summary>
        /// <param name="paragraphViewModel"></param>
        public void AddParagraph(ParagraphViewModel paragraphViewModel)
        {
            paragraphViewModel.OnFinnished = NextParagraph;

            this.ParagraphViewModels.Add(paragraphViewModel);
        }

 

        internal async Task GoToNextParagraph()
        {
            NextParagraph();
        }

        internal async Task GoToPreviousParagraph()
        {
            PreviousParagraph();
        }

        /// <summary>
        /// To be fired when closing this page and navigation to other page
        /// </summary>
        /// <returns></returns>
        public async Task OnClose()
        {
            await StopReading();            
        }

        private void SelectParagraph(int currentParagraphIndex)
        {
            CurrentParagraph = ParagraphViewModels.First(p => p.Index == currentParagraphIndex);

            CurrentParagraph.Active = true;

            TaskManager.Run(async () => ViewModelApplication.SavePosition(this.Index, this.CurrentParagraph.Index));

        }

        #endregion

        #region Private Methods

        private async Task StartReading()
        {
            parent.SpeechSynthesizer.SpeakAsyncCancelAll();

            if (!IsReading)
            {
                parent.SpeechSynthesizer.SpeakProgress += SpeakProgress;
                parent.SpeechSynthesizer.SpeakCompleted += SpeakCompleted;
                IsReading = true;
            }

            CurrentParagraph.Active = true;

            CurrentParagraph.OnPropertyChanged(nameof(CurrentParagraph.Active));

            parent.SpeechSynthesizer.SpeakAsync(CurrentParagraph.ParagraphText);

        }

        private async Task StopReading()
        {
            IsReading = false;

            if(CurrentParagraph!=null)
                CurrentParagraph.Active = false;
            
            parent.SpeechSynthesizer.SpeakProgress -= SpeakProgress;

            parent.SpeechSynthesizer.SpeakCompleted -= SpeakCompleted;

            parent.SpeechSynthesizer.SpeakAsyncCancelAll();
        }

        /// <summary>
        /// Go to the next paragraph
        /// </summary>
        /// <param name="currentParagraph"></param>
        private void NextParagraph()
        {
            if (ParagraphViewModels.Count <= CurrentParagraph.Index + 1)
                Finnished();
            else
            {
                SelectParagraph(CurrentParagraph.Index + 1);
            }
        }

        /// <summary>
        /// Go to the next paragraph
        /// </summary>
        /// <param name="currentParagraph"></param>
        private void PreviousParagraph()
        {
            if (CurrentParagraph.Index==0)
                PreviousPage();
            else
            {
                SelectParagraph(CurrentParagraph.Index - 1);
            }
        }

        /// <summary>
        /// To be fired when the reading of this page is finnished
        /// </summary>
        private void Finnished() => OnFinnished?.Invoke(Index);
        private void PreviousPage() => OnPreviousPage?.Invoke(Index);

        #endregion

        #region Event Hundlers

        /// <summary>
        /// Event to hundle the changes on the ui with the progress of the reading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            CurrentParagraph.WordIndex = e.CharacterPosition;

            CurrentParagraph.WordLength = e.CharacterCount;
        }

        /// <summary>
        /// Action to hundle the end of paragraph event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {

            if (e.Cancelled)
                return;

            CurrentParagraph.Active = false;

            CurrentParagraph.WordLength = 0;

            NextParagraph();
        }

        #endregion
    }
}
