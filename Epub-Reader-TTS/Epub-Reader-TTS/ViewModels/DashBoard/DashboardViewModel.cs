using Epub_Reader_TTS.Core;
using EpubSharp;
using EpubSharp.Format;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Epub_Reader_TTS
{ 
    public class DashboardViewModel : BaseViewModel
    {

        public ObservableCollection<TileViewModel> Tiles { get; set; }

        public int Columns { get; set; } = 4;

        private List<Book> bookList;



        public ICommand OpenBookCommand { get; set; }
        public ICommand RefreshAllCommand { get; set; }

        public DashboardViewModel()
        {
            OpenBookCommand = new RelayCommand(async () => await OpenBookFile());

            RefreshAllCommand = new RelayCommand(() => DI.TaskManager.Run(async()=>await RefreshAll()));

            Initiate().GetAwaiter().GetResult();
        }

        private async Task RefreshAll()
        {


            foreach(Book book in bookList)
            {
                await RefreshBook(book);
            }
        }

        private async Task Initiate()
        {
            bookList = await DI.ClientDataStore.GetBooks();

            Tiles = new ObservableCollection<TileViewModel>();

            foreach (Book book in bookList)
                Tiles.Add(new TileViewModel(book, this));
        }

        private async Task OpenBookFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "epub books (*.epub)|*.epub";

            var result =  openFileDialog.ShowDialog();

            if((result!=null) && (bool)result)
            {
                var book = bookList.FirstOrDefault(b => b.BookFilePath == openFileDialog.FileName);


                if (book !=null)
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

            if(!DI.FileManager.PathExists(file.BookCoverPath))
            {
                if (string.IsNullOrEmpty(file.BookCoverPath))
                    file.BookCoverPath = DI.FileManager.ResolvePath($"Covers/{file.Id}.png");

                DI.FileManager.EnsurePathExist(DI.FileManager.ResolvePath("Covers/"));

                var image = book.CoverImage.ToImage();

                image.Save(file.BookCoverPath, System.Drawing.Imaging.ImageFormat.Png);
            }

            await DI.ClientDataStore.AddBook(file);

            //await Initiate();
        }

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
            foreach(EpubTextFile text in html)
            {
                var pageVM = new PageViewModel()
                {
                    Index = i,
                    Title = book.TableOfContents[i].Title
                };

                var j = 0;
                foreach(string paragraph in text.ToParagraphs())
                {
                    var paragraphVM = new ParagraphViewModel()
                    {
                        Index = j++,
                        ParagraphText = paragraph,
                    };

                    pageVM.AddParagraph(paragraphVM);
                }
                if(pageVM.ParagraphViewModels.Count>0)
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
            //// Read metadata
            //string title = book.Title;
            //List<string> authors = book.Authors.ToList();
            //Image cover = book.CoverImage.ToImage();

            //// Get table of contents
            //ICollection<EpubChapter> chapters = book.TableOfContents;

            //// Get contained files
            //ICollection<EpubTextFile> css = book.Resources.Css;
            //ICollection<EpubByteFile> images = book.Resources.Images;
            //ICollection<EpubByteFile> fonts = book.Resources.Fonts;

            //// Convert to plain text
            //string text = book.ToPlainText();

            //var text1 = html.Skip(10).First().ToParagraphs();
        }

        public async Task RemoveBook(Book file)
        {

        }

        private void LoadBook(Book file)
        {
            // Read an epub file
            EpubBook book = EpubReader.Read(file.BookFilePath);

            // Read metadata
            string title = book.Title;
            List<string> authors = book.Authors.ToList();
            Image cover = book.CoverImage.ToImage();

            // Get table of contents
            ICollection<EpubChapter> chapters = book.TableOfContents;

            // Get contained files
            ICollection<EpubTextFile> html = book.Resources.Html;
            ICollection<EpubTextFile> css = book.Resources.Css;
            ICollection<EpubByteFile> images = book.Resources.Images;
            ICollection<EpubByteFile> fonts = book.Resources.Fonts;

            // Convert to plain text
            string text = book.ToPlainText();

            var text1 = html.Skip(10).First().ToParagraphs();
            //Debug.WriteLine(text1);
            //html.First().TextContent

            // Access internal EPUB format specific data structures.
            EpubFormat format = book.Format;
            OcfDocument ocf = format.Ocf;
            OpfDocument opf = format.Opf;
            NcxDocument ncx = format.Ncx;
            NavDocument nav = format.Nav;

            // Create an EPUB
            EpubWriter.Write(book, "new.epub");
        }
    }
}
