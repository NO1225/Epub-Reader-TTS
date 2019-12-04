using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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

            Test();//.GetAwaiter().GetResult();

            //Current.MainWindow = new MainWindow();

            //Current.MainWindow.Show();

        }

        private async Task Test()
        {

            var text = @"The experienced publisher misdirects the downhill dragon. When will a suite object? Can the credible ideal nose? When will a dustbin collapse underneath a trained politician?";

            var sc = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");



            var ss = new SpeechSynthesizer(sc);


            using (var result = await ss.SpeakTextAsync(text))
            {
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    Debug.WriteLine($"Speech synthesized to speaker for text [{text}]");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    Debug.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Debug.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Debug.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        Debug.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }

            //ss.SpeakTextAsync(text).GetAwaiter().GetResult();

            //a.Speek().GetAwaiter().GetResult();

        }
    }
}
