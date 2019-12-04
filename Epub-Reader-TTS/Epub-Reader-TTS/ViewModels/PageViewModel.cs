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

        #region MyRegion

        public Action<int> OnFinnished;

        public bool Focused { get; set; }

        public int Index { get; set; }

        public int ParagraphIndex { get; set; }

        public ObservableCollection<ParagraphViewModel> ParagraphViewModels { get; set; }

        public ParagraphViewModel CurrentParagraph { get=> ParagraphViewModels != null && ParagraphViewModels.Count > 0 ? ParagraphViewModels[ParagraphIndex] : null; }

        private SpeechSynthesizer SpeechSynthesizer;

        #endregion


        #region Default Constructor

        public PageViewModel()
        {
            this.ParagraphViewModels = new ObservableCollection<ParagraphViewModel>();

            for(int i =0;i<5;i++)
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

        private async Task Load()
        {
            ReadCurrent();
        }

        public async Task ReadCurrent()
        {
            SpeechSynthesizer.SpeakAsyncCancelAll();

            CurrentParagraph.Active = true;

            Debug.WriteLine("starting");

            SpeechSynthesizer.SpeakAsync(CurrentParagraph.ParagraphText);

            Debug.WriteLine("finished");
        }

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


        #region Public Methods

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
                ReadCurrent();
            }
        }

        #endregion

        #region Private Methods

        private void Finnished()
        {
            if (OnFinnished != null)
                OnFinnished(this.Index);
        }

        #endregion
    }
}
