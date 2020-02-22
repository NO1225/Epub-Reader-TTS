﻿using Dna;
using Epub_Reader_TTS.Relational;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

            // Setup the main application 
            await ApplicationSetupAsync();

            // Log it
            Logger.LogDebugSource("Application starting...");


            // To be used with open with command 
            if(e.Args.Length == 1)
            {
                foreach (string file in e.Args)
                {
                    var book = DI.ViewModelDashboard.OpenBookFile(file).GetAwaiter().GetResult();

                    DI.ViewModelDashboard.OpenBook(book).GetAwaiter().GetResult();
                }                
            }
            else
            {
                foreach (string file in e.Args)
                {
                    DI.ViewModelDashboard.OpenBookFile(file).GetAwaiter().GetResult();
                }

                ViewModelApplication.GoToPage(ApplicationPage.Dashboard);
            }
                       
            Current.MainWindow = new MainWindow();

            Current.MainWindow.Show();
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

            // Load settings
            ViewModelApplication.SetDarkMode(DI.SettingsManager.IsDarkMode());

            ViewModelApplication.SetFontSize(DI.SettingsManager.GetFontSize());

            // Ensure the client data store 
            await ClientDataStore.EnsureDataStoreAsync();

        }
    }
}
