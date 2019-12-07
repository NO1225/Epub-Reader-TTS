using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epub_Reader_TTS.Core;

namespace Epub_Reader_TTS.Relational
{
    /// <summary>
    /// Stores and retrieves information about the client application 
    /// such as login credentials, messages, settings and so on
    /// in an SQLite database
    /// </summary>
    public class BaseClientDataStore : IClientDataStore
    {
        #region Protected Members

        /// <summary>
        /// The database context for the client data store
        /// </summary>
        protected ClientDataStoreDbContext mDbContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dbContext">The database to use</param>
        public BaseClientDataStore(ClientDataStoreDbContext dbContext)
        {
            // Set local member
            mDbContext = dbContext;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Makes sure the client data store is correctly set up
        /// </summary>
        /// <returns>Returns a task that will finish once setup is complete</returns>
        public async Task EnsureDataStoreAsync()    
        {
            // Make sure the database exists and is created
            await mDbContext.Database.EnsureCreatedAsync();
        }

        public Task<List<Book>> GetBooks()
        {
            return Task.FromResult(mDbContext.Books.OrderBy(b=>b.LastOpenDate).ToList());
        }

        public async Task AddBook(Book book)
        {
            // Clear all entries
            //var exists = mDbContext.Books.FirstOrDefault(b=>b.Id == book.Id)!=null;

            if (string.IsNullOrEmpty(book.Id.ToString()))
                // Add new one
                mDbContext.Books.Add(book);
            else
                // Update the existing one
                mDbContext.Books.Update(book);

            // Save changes
            await mDbContext.SaveChangesAsync();
        }

        public Task<bool> CheckBookPaths(Book book)
        {
            return Task.FromResult(CoreDI.FileManager.PathExists(book.BookFilePath));
        }

        public Task<bool> CheckAllBooksPaths()
        {
            throw new System.NotImplementedException();
        }


        #endregion
    }
}
