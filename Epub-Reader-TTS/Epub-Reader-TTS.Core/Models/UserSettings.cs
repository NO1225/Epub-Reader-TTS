using System;
using System.Collections.Generic;
using System.Text;

namespace Epub_Reader_TTS.Core
{
    public class UserSettings
    {
        public int FontSize { get; set; } = 14;

        public string SelectedVoice { get; set; }

        public double ReadingSpeed { get; set; } = 3;

        public double VoicePitch { get; set; } = 1;

        public bool DarkMode { get; set; } = false;
    }
}
