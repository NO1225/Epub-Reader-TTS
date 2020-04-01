using Dna;
using Epub_Reader_TTS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Epub_Reader_TTS.Relational
{
    /// <summary>
    /// Extension methods for the <see cref="FrameworkConstruction"/>
    /// </summary>
    public static class FrameworkConstructionExtensions
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public static FrameworkConstruction AddClientDataStore(this FrameworkConstruction construction)
        {
            // Getting the path to the document folder
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Epub Reader TTS");
            
            // Getting the current connection string
            var tmpConnectionString = construction.Configuration.GetConnectionString("ClientDataStoreConnection");

            // Check for preserved keys and replace them
            var connectionString = tmpConnectionString.Replace("|Documents|", path);

            // If there was any replacement, make sure the folder exist
            if(tmpConnectionString != connectionString)
                Directory.CreateDirectory(path);

            // Inject our SQLite EF data store
            construction.Services.AddDbContext<ClientDataStoreDbContext>(options =>
            {
                // Setup connection string
                options.UseSqlite(connectionString);
            }, contextLifetime: ServiceLifetime.Transient);

            // Add client data store for easy access/use of the backing data store
            // Make it scoped so we can inject the scoped DbContext
            construction.Services.AddTransient<IClientDataStore>(
                provider => new BaseClientDataStore(provider.GetService<ClientDataStoreDbContext>()));

            // Return framework for chaining
            return construction;
        }
    }
}
