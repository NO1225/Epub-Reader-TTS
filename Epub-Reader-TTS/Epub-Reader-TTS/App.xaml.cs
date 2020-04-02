using Dna;
using Epub_Reader_TTS.Core;
using Epub_Reader_TTS.Relational;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using static Epub_Reader_TTS.DI;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// Custom startup so we load our IoC immediately before anything else
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            // Let the base application do what it needs
            base.OnStartup(e);

            var win = new Epub_Reader_TTS.SplashScreen();

            win.Show();

            // Setup the main application 
            await ApplicationSetupAsync();

            // Log it
            Logger.LogDebugSource("Application starting...");

            Book book = null;

            foreach (string file in e.Args)
            {
                book = DI.ViewModelDashboard.OpenBookFile(file).GetAwaiter().GetResult();
            }

            ViewModelApplication.GoToPage(ApplicationPage.Dashboard);

            Current.MainWindow = new MainWindow();

            win.Close();

            Current.MainWindow.Show();


            // To be used with open with command 
            if (e.Args.Length == 1)
            {
                TaskManager.Run(() => DI.ViewModelDashboard.OpenBook(book));//.GetAwaiter().GetResult();
            }


        }


        /// <summary>
        /// Configures our application ready for use
        /// </summary>
        private async Task ApplicationSetupAsync()
        {
            // Set the default working directory
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? Directory.GetCurrentDirectory());

            // Setup the Dna Framework
            Framework.Construct<DefaultFrameworkConstruction>()
                .AddFileLogger()
                .AddClientDataStore()
                .AddApplicationViewModels()
                .AddClientServices()
                .Build();

            await DI.SettingsManager.Initiate();

            FileAssociations.EnsureAssociationsSet();

            // Load settings
            DI.UIManager.UpdateDarkMode(DI.SettingsManager.IsDarkMode());

            DI.UIManager.UpdateFontSize(DI.SettingsManager.GetFontSize());

            // Ensure the client data store 
            await ClientDataStore.EnsureDataStoreAsync();

        }
    }
}
