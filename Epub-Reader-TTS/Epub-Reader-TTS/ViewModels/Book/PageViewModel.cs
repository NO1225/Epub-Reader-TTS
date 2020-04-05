using Dna;
using Epub_Reader_TTS.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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

        private bool sorting;
        private bool waiting;

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
        /// The list of the sorted paragraphs on this page
        /// </summary>
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

            SortParagraphs();

            SelectParagraph(paragraphIndex);

            if (reading)
                StartReading();
        }

        /// <summary>
        /// Hundle the sorrting of the paragraphs to prevent synchronous updates
        /// </summary>
        public void SortParagraphs()
        {
            Logger.LogDebugSource("sort queed");
            if (sorting)
            {
                waiting = true;
                return;
            }
            else
            {
                Logger.LogDebugSource("start sorting");
                sorting = true;
                Task.Run(async () => StartSorting());
            }
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
            paragraphViewModel.SelectThis = StartFromThisParagraph;
            this.ParagraphViewModels.Add(paragraphViewModel);
        }

  

        /// <summary>
        /// Go to the next paragraph
        /// </summary>
        /// <returns></returns>
        internal async Task GoToNextParagraph()
        {
            NextParagraph();
        }

        /// <summary>
        /// Step back to the previous paragraph
        /// </summary>
        /// <returns></returns>
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


        #endregion

        #region Private Methods

        /// <summary>
        /// Sorting the paragraphs so that there won't be any text hidden 
        /// </summary>
        /// <returns></returns>
        private async Task StartSorting()
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            int currentPage = 0;
            double currentHeight = 0;
            double allowedHeight = parent.ActualHeight;
            double allowedWidth = parent.ActualWidth / 2 - 40;
            if (parent.ActualWidth > 0 && parent.ActualHeight > 0)
            {
                var minHeight = " ".GetParagraphHeight(allowedWidth, DI.SettingsViewModel.FontSize);

                var paragraphsText = new List<ParagraphTextViewModel>();

                foreach (var paragraph in ParagraphViewModels)
                {
                    if (currentHeight + minHeight > allowedHeight)
                    {
                        currentPage++;
                        currentHeight = 0;
                    }

                    var paragraphHeight = paragraph.GetParagraphHeight(allowedWidth, DI.SettingsViewModel.FontSize);

                    if (currentHeight + paragraphHeight > allowedHeight)
                    {
                        paragraph.StartSpliting(paragraph.ParagraphText, allowedHeight - currentHeight, allowedHeight, allowedWidth, DI.SettingsViewModel.FontSize);

                        currentPage++;
                        currentHeight = 0;
                        for (int i = 1; i < paragraph.Paragraphs.Count; i++)
                        {
                            if (currentHeight > allowedHeight)
                                currentPage++;
                            paragraphHeight = paragraph.Paragraphs[i].GetParagraphHeight(allowedWidth, DI.SettingsViewModel.FontSize);
                            currentHeight += paragraphHeight;
                        }

                    }
                    else
                    {
                        paragraph.StartSpliting(paragraph.ParagraphText, allowedHeight - currentHeight, allowedHeight, allowedWidth, DI.SettingsViewModel.FontSize);
                        currentHeight += paragraphHeight;
                    }

                    paragraphsText.AddRange(paragraph.Paragraphs.Select(paragraph => paragraph));
                }

                ParagraphTextViewModels = new ObservableCollection<ParagraphTextViewModel>(paragraphsText);
            }
            stopwatch.Stop();

            Logger.LogDebugSource($"Elapsed Time is {stopwatch.ElapsedMilliseconds} ms");

            if (waiting && (allowedHeight != parent.ActualHeight || allowedWidth != parent.ActualWidth / 2 - 40))
            {
                waiting = false;
                StartSorting();
            }
            else
            {
                sorting = false;
                waiting = false;
            }

        }

        /// <summary>
        /// Start reading 
        /// </summary>
        /// <returns></returns>
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

            DI.SpeechSynthesizer.SpeakAsync(CurrentParagraph.ParagraphText);

        }

        /// <summary>
        /// Stop the reading
        /// </summary>
        /// <returns></returns>
        private async Task StopReading()
        {
            IsReading = false;

            if (CurrentParagraph != null)
                CurrentParagraph.Active = false;

            DI.SpeechSynthesizer.SpeakAsyncCancelAll();
        }

        /// <summary>
        /// Jump to a paragraph
        /// </summary>
        /// <param name="currentParagraphIndex">the index of the paragraph to jump to</param>
        private void SelectParagraph(int currentParagraphIndex)
        {
            CurrentParagraph = ParagraphViewModels.First(p => p.Index == currentParagraphIndex);

            CurrentParagraph.Active = true;

            TaskManager.Run(async () => ViewModelApplication.SavePosition(this.Index, this.CurrentParagraph.Index));

        }

        /// <summary>
        /// Select this paragraph and start reading it
        /// </summary>
        /// <param name="paragraphIndex"></param>
        private void StartFromThisParagraph(int paragraphIndex)
        {
            SelectParagraph(paragraphIndex);
            if (!IsReading)
                TogglePause(false);
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
            CurrentParagraph.SetWordIndexAndLength(characterPosition, characterCount);
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

            NextParagraph();
        }

        #endregion
    }
}
