using Epub_Reader_TTS.Core;
using EpubSharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Viewmodel to store all the details to be displayed on the dashboard page
    /// </summary>
    public class DashboardViewModel : BaseViewModel
    {

        #region Public Properties

        /// <summary>
        /// Collection of tiles - Stored books - to be displayed
        /// </summary>
        public ObservableCollection<TileViewModel> Tiles { get; set; }

        #endregion

        #region Private Fields

        /// <summary>
        /// List of the books stored in the database
        /// </summary>
        private List<Book> bookList;

        #endregion

        #region Public Commands

        /// <summary>
        /// Command to add a new file to the list of books
        /// </summary>
        public ICommand OpenBookCommand { get; set; }

        /// <summary>
        /// Command to refresh and retreive all the information about the stored books
        /// </summary>
        public ICommand RefreshAllCommand { get; set; }

        #endregion

        #region Default Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DashboardViewModel()
        {
            OpenBookCommand = new RelayCommand(async () => await OpenBookFile());

            RefreshAllCommand = new RelayCommand(() => DI.TaskManager.Run(async () => await RefreshAll()));

            Initiate().GetAwaiter().GetResult();
        }

        #endregion

        #region Initiator

        /// <summary>
        /// The initiator for this page to get the books from the database and store them to be displayed
        /// </summary>
        /// <returns></returns>
        private async Task Initiate()
        {
            this.bookList = await DI.ClientDataStore.GetBooks();

            Tiles = new ObservableCollection<TileViewModel>();

            foreach (Book book in bookList)
                Tiles.Add(new TileViewModel(book, this));
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Refresh all the books and retreive their information
        /// </summary>
        /// <returns></returns>
        private async Task RefreshAll()
        {
            foreach (Book book in bookList)
            {
                await RefreshBook(book);
            }
        }

        /// <summary>
        /// Open the file and assosiate it with a book model
        /// </summary>
        /// <returns></returns>
        private async Task OpenBookFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "epub books (*.epub)|*.epub";

            var result = openFileDialog.ShowDialog();

            if ((result != null) && (bool)result)
            {
                var book = bookList.FirstOrDefault(b => b.BookFilePath == openFileDialog.FileName);


                if (book != null)
                {
                    book.LastOpenDate = DateTime.Now;

                    await RefreshBook(book);
                }
                else
                {
                    book = new Book()
                    {
                        LastOpenDate = DateTime.Now,
                        BookFilePath = openFileDialog.FileName,
                    };

                    await RefreshBook(book);

                }
                await Initiate();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh the selected book
        /// </summary>
        /// <param name="file">the selected book</param>
        /// <returns></returns>
        public async Task RefreshBook(Book file)
        {
            if (!DI.FileManager.PathExists(file.BookFilePath))
            {
                // TODO: 
                MessageBox.Show("Not Found");
            }

            // Read an epub file
            EpubBook book = EpubReader.Read(file.BookFilePath);

            file.BookName = book.Title;

            await DI.ClientDataStore.AddBook(file);

            if (!DI.FileManager.PathExists(file.BookCoverPath))
            {
                if (string.IsNullOrEmpty(file.BookCoverPath))
                    file.BookCoverPath = DI.FileManager.ResolvePath($"Covers/{file.Id}.png");

                DI.FileManager.EnsurePathExist(DI.FileManager.ResolvePath("Covers/"));

                var image = book.CoverImage.ToImage();

                image.Save(file.BookCoverPath, System.Drawing.Imaging.ImageFormat.Png);
            }

            await DI.ClientDataStore.AddBook(file);
        }

        /// <summary>
        /// Open the selected book
        /// </summary>
        /// <param name="file">the selected book</param>
        /// <returns></returns>
        public async Task OpenBook(Book file)
        {
            //await RefreshBook(file);

            if (!DI.FileManager.PathExists(file.BookFilePath))
            {
                // TODO: 
                MessageBox.Show("Not Found");
            }

            // Read an epub file
            EpubBook book = EpubReader.Read(file.BookFilePath);

            ICollection<EpubTextFile> html = book.Resources.Html;

            var bookVM = new BookViewModel()
            {
                Title = file.BookName,
            };

            var i = 0;
            foreach (EpubTextFile text in html)
            {
                var pageVM = new PageViewModel()
                {
                    Index = i,
                    Title = book.TableOfContents[i].Title
                };

                var j = 0;
                foreach (string paragraph in text.ToParagraphs())
                {
                    var paragraphVM = new ParagraphViewModel()
                    {
                        Index = j++,
                        ParagraphText = paragraph,
                    };

                    pageVM.AddParagraph(paragraphVM);
                }
                if (pageVM.ParagraphViewModels.Count > 0)
                {
                    bookVM.AddPage(pageVM);
                    i++;
                }
            }

            file.LastOpenDate = DateTime.Now;

            DI.ClientDataStore.AddBook(file);

            ViewModelLocator.ApplicationViewModel.CurrentBook = file;

            ViewModelLocator.ApplicationViewModel.CurrentBookViewModel = bookVM;

            ViewModelLocator.ApplicationViewModel.CurrentBookViewModel.Initialize(file);

            ViewModelLocator.ApplicationViewModel.GoToPage(ApplicationPage.Book);

        }

        /// <summary>
        /// Remove the selected book
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task RemoveBook(Book file)
        {
            // TODO:  
        }

        #endregion

    }
}
