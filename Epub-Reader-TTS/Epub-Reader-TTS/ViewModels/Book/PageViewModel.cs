using Epub_Reader_TTS.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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

        /// <summary>
        /// Action to be stored
        /// </summary>
        public Action<int> OnPreviousPage;

        private double actualHeight = 501;

        private double actualWidth = 392;

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

        public ObservableCollection<ParagraphTextViewModel> ParagraphTextViewModels { get; set; }

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

        #region UI Properties

        public double ActualHeight
        {
            get => actualHeight; set
            {
                actualHeight = value;
            }
        }
        public double ActualWidth
        {
            get => actualWidth; set
            {
                actualWidth = value/2;
                //if(ActualWidth != 0 && ActualHeight !=0)
                //SortParagraphs();

            }
        }

        #endregion

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
        public void Initiate(bool reading = false, int paragraphIndex = 0)
        {
            //if (CurrentParagraph == null)
            //    CurrentParagraph = ParagraphViewModels.First();
            //SortParagraphs();

            SelectParagraph(paragraphIndex);

            if (reading)
                StartReading();
        }

        public void SortParagraphs()
        {
            int currentPage = 0;
            double currentHeight = 0;
            double allowedHeight = ActualHeight;
            double allowedWidth = ActualWidth - 20;

            var minHeight = " ".GetParagraphHeight(allowedWidth, parent.FontSize);

            var paragraphsText = new List<ParagraphTextViewModel>();

            foreach (var paragraph in ParagraphViewModels)
            {
                if(currentHeight + minHeight > allowedHeight)
                {
                    currentPage++;
                    currentHeight = 0;
                }

                var paragraphHeight = paragraph.GetParagraphHeight(allowedWidth, parent.FontSize);

                if(currentHeight + paragraphHeight > allowedHeight)
                {
                    paragraph.Split(paragraph.ParagraphText, allowedHeight - currentHeight, allowedHeight, allowedWidth, parent.FontSize);

                    currentPage++;
                    currentHeight = 0;
                    for(int i =1;i<paragraph.Paragraphs.Count;i++)
                    {
                        if (currentHeight > allowedHeight)
                            currentPage++;
                        paragraphHeight = paragraph.Paragraphs[i].GetParagraphHeight(allowedWidth, parent.FontSize);
                        currentHeight += paragraphHeight;
                    }
                    // Split
                    // If split add the number to the next page 
                }
                else
                {
                    paragraph.Split(paragraph.ParagraphText, allowedHeight - currentHeight, allowedHeight, allowedWidth, parent.FontSize);
                    currentHeight += paragraphHeight;
                }

                paragraphsText.AddRange(paragraph.Paragraphs.Select(paragraph => paragraph));
            }

            ParagraphTextViewModels = new ObservableCollection<ParagraphTextViewModel>(paragraphsText);
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
            DI.SpeechSynthesizer.SpeakAsyncCancelAll();

            if (!IsReading)
            {
                DI.SpeechSynthesizer.SpeakProgress = SpeakProgress;
                DI.SpeechSynthesizer.SpeakComplete = SpeakComplete;
                IsReading = true;
            }

            CurrentParagraph.Active = true;

            CurrentParagraph.OnPropertyChanged(nameof(CurrentParagraph.Active));

            DI.SpeechSynthesizer.SpeakAsync(CurrentParagraph.ParagraphText);

        }



        private async Task StopReading()
        {
            IsReading = false;

            if (CurrentParagraph != null)
                CurrentParagraph.Active = false;

            //DI.SpeechSynthesizer.SpeakProgress -= SpeakProgress;

            //DI.SpeechSynthesizer.SpeakCompleted -= SpeakCompleted;

            DI.SpeechSynthesizer.SpeakAsyncCancelAll();
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
            if (CurrentParagraph.Index == 0)
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
        private void SpeakProgress(int characterPosition, int characterCount)
        {
            CurrentParagraph.WordIndex = characterPosition;

            CurrentParagraph.WordLength = characterCount;
        }

        /// <summary>
        /// Action to hundle the end of paragraph event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeakComplete(CompletionReason reason)
        {
            if (reason == CompletionReason.Cancel)
                return;

            CurrentParagraph.Active = false;

            CurrentParagraph.WordLength = 0;

            NextParagraph();
        }

        #endregion
    }
}
