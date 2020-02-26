using Epub_Reader_TTS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epub_Reader_TTS
{
    public class SpeechSynthesizer : ISpeechSynthesizer
    {
        //private WindowsXamlHost windowsXamlHost = new WindowsXamlHost();

        private Speaker.SpeakerWithSMTC speaker;

        public double Rate
        {
            get
            {
                return speaker.Rate;
            }
            set
            {
                speaker.Rate = value;
            }
        }

        public double Pitch
        {
            get
            {
                return speaker.Pitch;
            }
            set
            {
                speaker.Pitch = value;
            }
        }

        public Action<int, int> SpeakProgress
        {
            set
            {
                speaker.SpeakProgress = value;
            }
        }

        public Action<CompletionReason> SpeakComplete
        {
            set
            {
                speaker.SpeakComplete = value;
            }
        }

        public Action PlayPressed
        {
            set
            {
                speaker.PlayPressed = value;
            }
        }

        public Action PausePressed
        {
            set
            {
                speaker.PausePressed = value;
            }
        }

        public Action NextPressed
        {
            set
            {
                speaker.NextPressed = value;
            }
        }

        public Action PreviousPressed
        {
            set
            {
                speaker.PreviousPressed = value;
            }
        }

        public SpeechSynthesizer(global::Speaker.SpeakerWithSMTC speaker)
        {
            this.speaker = speaker;
        }

        public async Task SpeakAsync(string text)
        {
            await speaker.SpeakAsync(text);
        }

        public IEnumerable<InstalledVoice> GetInstalledVoices()
        {
            return speaker.GetInstalledVoices();

        }

        public bool SelectVoice(string voiceName)
        {
            var voice = speaker.GetInstalledVoices().FirstOrDefault(v => v.DisplayName == voiceName);

            if (voice != null)
            {
                return speaker.SelectVoice(voice);
            }
            return false;
        }

        public InstalledVoice GetSelectedVoice()
        {
            return speaker.GetSelectedVoice();
        }

        public async Task SpeakAsyncCancelAll()
        {
            await speaker.SpeakAsyncCancelAll();
        }

        public void UpdateSystemMediaTrasportControls(
            string title,
            string chapter,
            string coverPath = "",
            MediaPlaybackStatus mediaPlaybackStatus = MediaPlaybackStatus.Playing,
            bool isPLayEnabled = true,
            bool isNextEnabled = true,
            bool isPreviousEnabled = true,
            bool isPauseEnabled = true
            )
        {
            speaker.UpdateSystemMediaTrasportControls(
                title,
                chapter,
                coverPath,
                mediaPlaybackStatus,
                isPLayEnabled,
                isNextEnabled,
                isPreviousEnabled,
                isPauseEnabled);
        }

    }
}
