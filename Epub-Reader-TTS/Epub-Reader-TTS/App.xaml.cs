using Dna;
using Epub_Reader_TTS.Relational;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static Dna.FrameworkDI;
using static Epub_Reader_TTS.Core.CoreDI;
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

            //MessageBox.Show(e.Args.Length.ToString());
            //MessageBox.Show(e.Args.FirstOrDefault().ToString());
            // Setup the main application 
            await ApplicationSetupAsync();

            // Log it
            Logger.LogDebugSource("Application starting...");

            ViewModelApplication.SetDarkMode(DI.SettingsManager.IsDarkMode());

            ViewModelApplication.SetFontSize(DI.SettingsManager.GetFontSize());

            ViewModelApplication.GoToPage(ApplicationPage.Dashboard);

            // To be used with open with command 
            if(e.Args.Length>0)
            {
                var dashboardViewModel = new DashboardViewModel();

                foreach (string file in e.Args)
                {
                    dashboardViewModel.RefreshBook(new Core.Book() { BookFilePath = file });
                }
            }

            


            Current.MainWindow = new MainWindow();

            Current.MainWindow.Show();
        }


        /// <summary>
        /// Configures our application ready for use
        /// </summary>
        private async Task ApplicationSetupAsync()
        {
            // Setup the Dna Framework
            Framework.Construct<DefaultFrameworkConstruction>()
                .AddFileLogger()
                .AddClientDataStore()
                .AddApplicationViewModels()
                .AddClientServices()
                .Build();

            // Ensure the client data store 
            await ClientDataStore.EnsureDataStoreAsync();

        }
    }
}
