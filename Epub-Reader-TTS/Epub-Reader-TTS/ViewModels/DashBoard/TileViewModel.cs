using Epub_Reader_TTS.Core;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Viewmodel to store the details of the tile
    /// </summary>
    public class TileViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The book related to this tile
        /// </summary>
        public Book Book { get; private set; }

        /// <summary>
        /// The parent - container - of this tile
        /// </summary>
        public DashboardViewModel Parent { get; private set; }

        /// <summary>
        /// The path to the cover image
        /// </summary>
        public string CoverPath => Book.BookCoverPath;

        #endregion

        #region Public Commands

        /// <summary>
        /// Command to refresh this tile
        /// </summary>
        public ICommand RefreshCommand { get; set; }

        /// <summary>
        /// Command to initiate the removing of the book related to this tile 
        /// </summary>
        public ICommand RemoveCommand { get; set; }

        /// <summary>
        /// Command to open the book related to this tile
        /// </summary>
        public ICommand OpenCommand { get; set; }

        #endregion

        #region Default Constructor

        /// <summary>
        /// The default constuctor 
        /// </summary>
        /// <param name="book">The book related ti this tile</param>
        /// <param name="parent">The container of this tile</param>
        public TileViewModel(Book book, DashboardViewModel parent)
        {
            this.Book = book;
            this.Parent = parent;

            RefreshCommand = new RelayCommand(async () => await Refresh());
            RemoveCommand = new RelayCommand(async () => await Remove());
            OpenCommand = new RelayCommand(async () => await Open());
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Open the book related to this tile
        /// </summary>
        /// <returns></returns>
        private async Task Open()
        {
            await Parent.OpenBook(this.Book);
        }

        /// <summary>
        /// Remove the book related to this tile
        /// </summary>
        /// <returns></returns>
        private async Task Remove()
        {
            await Parent.RemoveBook(this.Book);
        }

        /// <summary>
        /// Refresh the book related to this tile
        /// </summary>
        /// <returns></returns>
        private async Task Refresh()
        {
            DI.TaskManager.Run(async () => await Parent.RefreshBook(this.Book));
            OnPropertyChanged(nameof(CoverPath));
        }

        #endregion
    }
}
