using Epub_Reader_TTS.Core;
using EpubSharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

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

        /// <summary>
        /// When the books are loading to be opened
        /// </summary>
        public bool Loading { get; set; }

        /// <summary>
        /// Constant to holde where should the cover images be saved to 
        /// </summary>
        public readonly string CoverPath;


        /// <summary>
        /// The type of the popup to be displayed
        /// </summary>
        public AdditionalContent CurrentAdditionalContent { get; set; }

        /// <summary>
        /// Show the pop up
        /// </summary>
        public bool AdditionalContentVisible { get; set; }

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

        /// <summary>
        /// Command to show the settings popup
        /// </summary>
        public ICommand ToggleSettingsCommand { get; set; }

        /// <summary>
        /// Command to hide any popup
        /// </summary>
        public ICommand HidePopUpCommand { get; set; }

        #endregion

        #region Default Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DashboardViewModel()
        {
            CoverPath = $"{Path.GetTempPath()}Epub Reader TTS\\Covers\\";

            OpenBookCommand = new RelayCommand(async () => await OpenBookFile());

            RefreshAllCommand = new RelayCommand(() => DI.TaskManager.Run(async () => await RefreshAll()));

            ToggleSettingsCommand = new RelayCommand(ToggleSettings);

            HidePopUpCommand = new RelayCommand(() => AdditionalContentVisible = false);

            Initiate().GetAwaiter().GetResult();
        }

        #endregion

        #region Initiator

        /// <summary>
        /// The initiator for this page to get the books from the database and store them to be displayed
        /// </summary>
        /// <returns></returns>
        public async Task Initiate()
        {
            this.bookList = (await DI.ClientDataStore.GetBooks()).Where(b => b.IsDisabled != true).ToList() ;

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
                await OpenBookFile(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// Show/Hide the settings popup
        /// </summary>
        private void ToggleSettings()
        {
            if (CurrentAdditionalContent == AdditionalContent.Settings)
            {
                AdditionalContentVisible = !AdditionalContentVisible;
            }
            else
            {
                CurrentAdditionalContent = AdditionalContent.Settings;
                AdditionalContentVisible = true;
            }
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// Open the file and assosiate it with a book model
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <returns></returns>
        public async Task<Book> OpenBookFile(string path)
        {
            if (!(Path.GetExtension(path).Substring(1).ToLower() == "epub"))
            {
                MessageBox.Show("Wrong file");
                return null;
            }

            var book = (await DI.ClientDataStore.GetBooks()).FirstOrDefault(b => b.BookFilePath == path);


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
                    BookFilePath = path,
                };

                await RefreshBook(book);

            }
            await Initiate();

            return book;
        }

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

            file.IsDisabled = false;

            await DI.ClientDataStore.AddBook(file);

            if (!DI.FileManager.PathExists(file.BookCoverPath))
            {
                file.BookCoverPath = DI.FileManager.ResolvePath($"{CoverPath}{file.Id}.png");

                DI.FileManager.EnsurePathExist(DI.FileManager.ResolvePath(CoverPath));
                
                if (book.CoverImage != null)
                {
                    File.WriteAllBytes(file.BookCoverPath, book.CoverImage);
                }
                else
                {
                    file.BookCoverPath = string.Empty;
                }
            }

            await DI.ClientDataStore.AddBook(file);
        }

        /// <summary>
        /// Open the selected book
        /// </summary>
        /// <param name="file">the selected book</param>
        /// <returns></returns>
        public void OpenBook(Book file)
        {
            //await RefreshBook(file);

            Loading = true;

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
                var chapter = book.TableOfContents.FirstOrDefault(tof => tof.FileName == text.FileName);

                if (chapter == null)
                {
                    continue;
                }

                // TODO: out of index
                var pageVM = new PageViewModel()
                {
                    Index = i,
                    Title = chapter.Title
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

            Loading = false;

        }

        /// <summary>
        /// Remove the selected book
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task RemoveBook(Book file)
        {
            // TODO:  

            var result = MessageBox.Show("Are you sure you want to delete this book", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            await DI.ClientDataStore.RemoveBook(file);

            await Initiate();
        }

        #endregion

    }
}
