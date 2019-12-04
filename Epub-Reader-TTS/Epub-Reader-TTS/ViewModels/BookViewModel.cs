using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Epub_Reader_TTS
{
    public class BookViewModel : BaseViewModel
    {

        #region MyRegion

        public Action OnFinnished;

        public bool Focused { get; set; }

        public string FilePath { get; set; }

        public int PageIndex { get; set; }

        public ObservableCollection<PageViewModel>  PageViewModels { get; set; }

        public PageViewModel CurrentPage { get => PageViewModels != null && PageViewModels.Count > 0 ? PageViewModels[PageIndex] : null; }

        #endregion

        #region Commands

        public ICommand PlayCommand { get; set; }

        public ICommand PauseCommand { get; set; }

        public ICommand StopCommand { get; set; }


        #endregion


        #region Default Constructor

        public BookViewModel()
        {
            PlayCommand = new RelayCommand(async ()=> await Play());

            PauseCommand = new RelayCommand(Pause);

            StopCommand = new RelayCommand(Stop);

            this.PageViewModels = new ObservableCollection<PageViewModel>();

            AddPage(new PageViewModel()
            {
                Focused = true,
                Index=0,
                ParagraphIndex=0,
            });

            OnPropertyChanged(nameof(CurrentPage));
        }

        private void Stop()
        {
            throw new NotImplementedException();
        }

        private void Pause()
        {
            throw new NotImplementedException();
        }

        private async Task Play()
        {
            await CurrentPage.ReadCurrent();
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

            this.PageViewModels.Add(pageViewModel);
        }

        public void NextPage(int currentPage)
        {

        }

        #endregion

        #region Private Methods

        private void Finnished()
        {
            if (OnFinnished != null)
                OnFinnished();
        }

        #endregion
    }
}
