using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
        protected override void OnStartup(StartupEventArgs e)
        {
            // Let the base application do what it needs
            base.OnStartup(e);

            // Setup IoC 
            //IoC.Setup();

            //Test();

            Current.MainWindow = new MainWindow();

            Current.MainWindow.Show();

        }

        //private void Test()
        //{

        //    var a = new SpeechManager();

        //    a.Speek().GetAwaiter().GetResult();

        //}
    }
}
