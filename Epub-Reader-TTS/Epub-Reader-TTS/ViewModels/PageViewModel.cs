using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Epub_Reader_TTS
{
    public class PageViewModel : BaseViewModel
    {
        #region Private Fields

        public BookViewModel Parent;

        #endregion

        #region Public Properties

        public Action<int> OnFinnished;

        public bool Focused { get; set; }

        public int Index { get; set; }

        public string Title { get; set; }

        public ObservableCollection<ParagraphViewModel> ParagraphViewModels { get; set; }

        public int ParagraphIndex { get; set; }

        public ParagraphViewModel CurrentParagraph { get => ParagraphViewModels != null && ParagraphViewModels.Count > 0 ? ParagraphViewModels[ParagraphIndex] : null; }

        public bool IsReading { get => Parent.SpeechSynthesizer.State == SynthesizerState.Speaking; }

        public bool IsClosing { get; set; }

        public bool FirstRun { get; set; }

        #endregion

        #region Default Constructor

        public PageViewModel()
        {
            this.ParagraphViewModels = new ObservableCollection<ParagraphViewModel>();

            for (int i = 0; i < 31; i++)
            {
                AddParagraph(new ParagraphViewModel()
                {
                    Active = false,
                    Index = i,
                    ParagraphText = $"{this.GetHashCode()} The experienced publisher misdirects the downhill dragon. When will a suite object? Can the credible ideal nose? When will a dustbin collapse underneath a trained politician?"
                });
            }

            OnPropertyChanged(nameof(CurrentParagraph));
        }

        #endregion

        #region Public Methods

        public void Initiate(bool reading = false)
        {
            IsClosing = true;

            Debug.WriteLine($"Initiating: {this.GetHashCode()} son of {this.Parent.GetHashCode()}");

            Parent.SpeechSynthesizer.SpeakProgress += SpeakProgress;

            Parent.SpeechSynthesizer.SpeakCompleted += SpeakCompleted;

            CurrentParagraph.Active = true;
        }

        public async Task StartReading()
        {
            await ReadParagraph();
        }

        public async Task StopReading()
        {

            Debug.WriteLine($"Stoping: {this.GetHashCode()} son of {this.Parent.GetHashCode()} isclosibg is {IsClosing}");

            Parent.SpeechSynthesizer.SpeakProgress -= SpeakProgress;

            Parent.SpeechSynthesizer.SpeakCompleted -= SpeakCompleted;

            // TODO: Cancel the event
            Parent.SpeechSynthesizer.SpeakAsyncCancelAll();

            Debug.WriteLine($"stoped: {this.GetHashCode()} son of {this.Parent.GetHashCode()} isclosibg is {IsClosing}");

        }

        public async Task TogglePause(bool forcePause)
        {
            IsClosing = false;

            if (forcePause)
            {
                Parent.SpeechSynthesizer.Pause();
                return;
            }
            if (Parent.SpeechSynthesizer.State == SynthesizerState.Speaking)
                Parent.SpeechSynthesizer.Pause();
            else if (Parent.SpeechSynthesizer.State == SynthesizerState.Paused)
            {
                Parent.SpeechSynthesizer.Resume();
                if (Parent.SpeechSynthesizer.State == SynthesizerState.Ready)
                    await ReadParagraph();

            }
            else
                await ReadParagraph();
        }

        public void AddParagraph(ParagraphViewModel paragraphViewModel)
        {
            paragraphViewModel.OnFinnished = NextParagraph;

            this.ParagraphViewModels.Add(paragraphViewModel);
        }

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
            }
        }

        public async Task OnClose()
        {
            IsClosing = true;

            Debug.WriteLine($"Closing: {this.GetHashCode()} son of {this.Parent.GetHashCode()} isclosibg is {IsClosing}");

            Parent.SpeechSynthesizer.SpeakProgress -= SpeakProgress;

            Parent.SpeechSynthesizer.SpeakCompleted -= SpeakCompleted;

            await StopReading();

            Debug.WriteLine($"Closed: {this.GetHashCode()} son of {this.Parent.GetHashCode()} isclosibg is {IsClosing}");

        }


        #endregion

        #region Private Methods


        private async Task ReadParagraph()
        {
            Parent.SpeechSynthesizer.SpeakAsyncCancelAll();

            CurrentParagraph.Active = true;

            Debug.WriteLine("starting");

            Parent.SpeechSynthesizer.SpeakAsync(CurrentParagraph.ParagraphText);

            Debug.WriteLine("finished");
        }


        private void Finnished()
        {
            if (OnFinnished != null)
                OnFinnished(this.Index);
        }

        #endregion

        #region Event Hundlers

        private void SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            if (IsClosing)
                return;
            CurrentParagraph.WordIndex = e.CharacterPosition;

            CurrentParagraph.WordLength = e.CharacterCount;
        }

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
