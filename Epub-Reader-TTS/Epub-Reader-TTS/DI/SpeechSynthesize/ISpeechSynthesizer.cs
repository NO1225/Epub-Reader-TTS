using Epub_Reader_TTS.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epub_Reader_TTS
{
    public interface ISpeechSynthesizer
    {
        Action NextPressed { set; }
        Action PausePressed { set; }
        double Pitch { get; set; }
        Action PlayPressed { set; }
        Action PreviousPressed { set; }
        double Rate { get; set; }
        Action<CompletionReason> SpeakComplete { set; }
        Action<int, int> SpeakProgress { set; }

        IEnumerable<InstalledVoice> GetInstalledVoices();
        InstalledVoice GetSelectedVoice();
        bool SelectVoice(string voiceName);
        Task SpeakAsync(string text);
        Task SpeakAsyncCancelAll();
        void UpdateSystemMediaTrasportControls(string title, string chapter, string coverPath = "", MediaPlaybackStatus mediaPlaybackStatus = MediaPlaybackStatus.Playing, bool isPLayEnabled = true, bool isNextEnabled = true, bool isPreviousEnabled = true, bool isPauseEnabled = true);
    }
}