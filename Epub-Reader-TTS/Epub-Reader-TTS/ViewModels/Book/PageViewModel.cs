using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        #region Public Properties

        /// <summary>
        /// The containing book of this page
        /// </summary>
        public BookViewModel parent;

        /// <summary>
        /// Action to be stored to be fired when the reading of this page is finnished
        /// </summary>
        public Action<int> OnFinnished;

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
        /// The index of the current paragraph
        /// </summary>
        public int ParagraphIndex { get; set; }

        /// <summary>
        /// The current active paragraph 
        /// </summary>
        public ParagraphViewModel CurrentParagraph { get => ParagraphViewModels != null && ParagraphViewModels.Count > 0 ? ParagraphViewModels[ParagraphIndex] : null; }

        /// <summary>
        /// If the applicaiton is reading 
        /// </summary>
        public bool IsReading { get => parent.SpeechSynthesizer.State == SynthesizerState.Speaking; }

        /// <summary>
        /// If this page is closing or first run 
        /// </summary>
        public bool IsClosing { get; set; }

        #endregion

        #region Default Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PageViewModel()
        {
            this.ParagraphViewModels = new ObservableCollection<ParagraphViewModel>();

            OnPropertyChanged(nameof(CurrentParagraph));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initiate this page
        /// </summary>
        /// <param name="reading">weither if the application is reading or not</param>
        public void Initiate(bool reading = false)
        {
            IsClosing = true;

            Debug.WriteLine($"Initiating: {this.GetHashCode()} son of {this.parent.GetHashCode()}");

            parent.SpeechSynthesizer.SpeakProgress += SpeakProgress;

            parent.SpeechSynthesizer.SpeakCompleted += SpeakCompleted;

            CurrentParagraph.Active = true;
        }
        
        /// <summary>
        /// Start reading or toggle between pause play ... 
        /// </summary>
        /// <param name="forcePause"></param>
        /// <returns></returns>
        public async Task TogglePause(bool forcePause)
        {
            IsClosing = false;

            if (forcePause)
            {
                parent.SpeechSynthesizer.Pause();
                return;
            }
            if (parent.SpeechSynthesizer.State == SynthesizerState.Speaking)
                parent.SpeechSynthesizer.Pause();
            else if (parent.SpeechSynthesizer.State == SynthesizerState.Paused)
            {
                parent.SpeechSynthesizer.Resume();
                if (parent.SpeechSynthesizer.State == SynthesizerState.Ready)
                    await ReadParagraph();

            }
            else
                await ReadParagraph();
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

        /// <summary>
        /// Go to the next paragraph
        /// </summary>
        /// <param name="currentParagraph"></param>
        public void NextParagraph(int currentParagraph)
        {
            if (ParagraphViewModels.Count <= currentParagraph + 1)
                Finnished();
            // TODO: ;
            else
            {
                CurrentParagraph.Active = false;

                ParagraphIndex = currentParagraph + 1;

                ReadParagraph();

                TaskManager.Run(async () => ViewModelApplication.SavePosition(this.Index, this.ParagraphIndex));                
            }
        }

        /// <summary>
        /// To be fired when closing this page and navigation to other page
        /// </summary>
        /// <returns></returns>
        public async Task OnClose()
        {
            IsClosing = true;

            Debug.WriteLine($"Closing: {this.GetHashCode()} son of {this.parent.GetHashCode()} isclosibg is {IsClosing}");

            parent.SpeechSynthesizer.SpeakProgress -= SpeakProgress;

            parent.SpeechSynthesizer.SpeakCompleted -= SpeakCompleted;

            parent.SpeechSynthesizer.SpeakAsyncCancelAll();

            Debug.WriteLine($"Closed: {this.GetHashCode()} son of {this.parent.GetHashCode()} isclosibg is {IsClosing}");
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Start reading the current paragraph
        /// </summary>
        /// <returns></returns>
        private async Task ReadParagraph()
        {
            parent.SpeechSynthesizer.SpeakAsyncCancelAll();

            CurrentParagraph.Active = true;

            Debug.WriteLine("starting");

            parent.SpeechSynthesizer.SpeakAsync(CurrentParagraph.ParagraphText);

            Debug.WriteLine("finished");
        }
        
        /// <summary>
        /// To be fired when the reading of this page is finnished
        /// </summary>
        private void Finnished()
        {
            if (OnFinnished != null)
                OnFinnished(this.Index);
        }

        #endregion

        #region Event Hundlers

        /// <summary>
        /// Event to hundle the changes on the ui with the progress of the reading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            if (IsClosing)
                return;
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
            if (IsClosing)
                return;
            CurrentParagraph.Active = false;

            CurrentParagraph.WordLength = 0;

            NextParagraph(ParagraphIndex);
        }

        #endregion
    }
}
