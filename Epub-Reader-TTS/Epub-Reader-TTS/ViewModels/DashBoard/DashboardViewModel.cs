using Epub_Reader_TTS.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Epub_Reader_TTS
{ 
    public class DashboardViewModel : BaseViewModel
    {

        public ObservableCollection<Book> Books { get; set; }

        public int Columns { get; set; } = 4;

        private List<Book> bookList;



        public ICommand OpenBookCommand { get; set; }

        public DashboardViewModel()
        {
            OpenBookCommand = new RelayCommand(async () => await OpenBook());

            Initiate().GetAwaiter().GetResult();
        }

        private async Task Initiate()
        {
            bookList = await DI.ClientDataStore.GetBooks();

            Books = new ObservableCollection<Book>(bookList);
        }

        private async Task OpenBook()
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
                    await DI.ClientDataStore.AddBook(book);
                }
                else
                {
                    book = new Book()
                    {
                        LastOpenDate = DateTime.Now,
                        BookFilePath = openFileDialog.FileName,
                    };

                    await DI.ClientDataStore.AddBook(book);
                }
                //var bookStream = DI.FileManager.OpenFile(openFileDialog.FileName);



                await Initiate();

            }





        }
    }
}
