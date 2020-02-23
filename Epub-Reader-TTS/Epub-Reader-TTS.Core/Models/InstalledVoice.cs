using System;

namespace Epub_Reader_TTS.Core
{
    public class InstalledVoice
    {

        public string Description { get; set; }

        public string DisplayName { get; set; }

        public VoiceGender Gender { get; set; }

        public string Id { get; set; }

        public string Language { get; set; }
    }

}
