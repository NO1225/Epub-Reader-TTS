using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Epub_Reader_TTS
{
    public class MainWindowViewModel:BaseViewModel
    {

        private Window _window;

        public MainWindowViewModel(Window window)
        {
            this._window = window;

            Load();
        }

        public BookViewModel BookViewModel { get; set; }

        private void Load()
        {
            //BookViewModel = new BookViewModel();
        }
    }
}
