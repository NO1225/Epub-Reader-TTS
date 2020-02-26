using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Interaction logic for UWPControlsHost.xaml
    /// </summary>
    public partial class UWPControlsHost : UserControl
    {
        public UWPControlsHost()
        {
            InitializeComponent();
        }

        private void WindowsXamlHost_ChildChanged(object sender, EventArgs e)
        {
            // Hook up x:Bind source.
            global::Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost windowsXamlHost =
                sender as global::Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost;
            global::Speaker.SpeakerWithSMTC userControl =
                windowsXamlHost.GetUwpInternalObject() as global::Speaker.SpeakerWithSMTC;

            if (userControl != null)
            {
                DI.SpeechSynthesizer = new SpeechSynthesizer(userControl);
            }
        }
    }
}
