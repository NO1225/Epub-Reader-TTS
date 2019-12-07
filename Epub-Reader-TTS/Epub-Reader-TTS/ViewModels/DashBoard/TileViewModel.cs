using Epub_Reader_TTS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Epub_Reader_TTS
{
    public class TileViewModel : BaseViewModel
    {
        public Book Book { get; private set; }
        public DashboardViewModel Parent { get; private set; }

        public string CoverPath => Book.BookCoverPath;


        public ICommand RefreshCommand { get; set; }

        public ICommand RemoveCommand { get; set; }

        public ICommand OpenCommand { get; set; }

        public TileViewModel(Book book, DashboardViewModel parent)
        {
            this.Book = book;
            this.Parent = parent;

            RefreshCommand = new RelayCommand(async () => await Refresh());
            RemoveCommand = new RelayCommand(async () => await Remove());
            OpenCommand = new RelayCommand(async () => await Open());
        }

        private async Task Open()
        {
            await Parent.OpenBook(this.Book);
        }

        private async Task Remove()
        {
            await Parent.RemoveBook(this.Book);
        }

        private async Task Refresh()
        {
            DI.TaskManager.Run(async() => await Parent.RefreshBook(this.Book));
            OnPropertyChanged(nameof(CoverPath));
        }
    }
}
