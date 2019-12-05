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

        private SpeechSynthesizer SpeechSynthesizer;

        #endregion

        #region Public Properties

        public Action<int> OnFinnished;

        public bool Focused { get; set; }

        public int Index { get; set; }

        public string Title { get; set; }

        public ObservableCollection<ParagraphViewModel> ParagraphViewModels { get; set; }

        public int ParagraphIndex { get; set; }
               
        public ParagraphViewModel CurrentParagraph { get=> ParagraphViewModels != null && ParagraphViewModels.Count > 0 ? ParagraphViewModels[ParagraphIndex] : null; }

        public bool IsReading { get => SpeechSynthesizer.State==SynthesizerState.Speaking; }

        #endregion


        #region Default Constructor

        public PageViewModel()
        {
            this.ParagraphViewModels = new ObservableCollection<ParagraphViewModel>();

            for(int i =0;i<51;i++)
            {
                AddParagraph(new ParagraphViewModel()
                {
                    Active = false,
                    Index = i,
                    ParagraphText = @"The experienced publisher misdirects the downhill dragon. When will a suite object? Can the credible ideal nose? When will a dustbin collapse underneath a trained politician?"
                });
            }

            OnPropertyChanged(nameof(CurrentParagraph));

            SpeechSynthesizer = new SpeechSynthesizer();

            // Configure the audio output.   
            SpeechSynthesizer.SetOutputToDefaultAudioDevice();

            SpeechSynthesizer.SelectVoice("Microsoft Zira Desktop");

            SpeechSynthesizer.Rate = 4;
            //var a = SpeechSynthesizer.GetInstalledVoices();

            SpeechSynthesizer.SpeakProgress += SpeakProgress;

            SpeechSynthesizer.SpeakCompleted += SpeakCompleted;

            //Load().GetAwaiter().GetResult();
        }

        #endregion


        #region Public Methods

        public async Task StartReading()
        {
            await ReadParagraph();
        }

        public async Task StopReading()
        {
            // TODO: Cancel the event
            SpeechSynthesizer.SpeakAsyncCancelAll();
        }

        public async Task TogglePause()
        {
            if (SpeechSynthesizer.State == SynthesizerState.Speaking)
                SpeechSynthesizer.Pause();
            else if (SpeechSynthesizer.State == SynthesizerState.Paused)
                SpeechSynthesizer.Resume();
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
                ParagraphIndex = currentParagraph + 1;
                ReadParagraph();
            }
        }

        #endregion

        #region Private Methods


        private async Task ReadParagraph()
        {
            SpeechSynthesizer.SpeakAsyncCancelAll();

            CurrentParagraph.Active = true;

            Debug.WriteLine("starting");

            SpeechSynthesizer.SpeakAsync(CurrentParagraph.ParagraphText);

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
            CurrentParagraph.WordIndex = e.CharacterPosition;

            CurrentParagraph.WordLength = e.CharacterCount;
        }

        private void SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            CurrentParagraph.Active = false;

            CurrentParagraph.WordLength = 0;

            NextParagraph(ParagraphIndex);
        }

        #endregion
    }
}
