using Epub_Reader_TTS.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Epub_Reader_TTS
{
    public class SettingsViewModel:BaseViewModel
    {
        #region Private Feilds

        /// <summary>
        /// The selected voice
        /// </summary>
        private InstalledVoice selectedVoice;

        /// <summary>
        /// The reading speed
        /// </summary>
        private double readingSpeed;

        /// <summary>
        /// The voice pitch
        /// </summary>
        private double voicePitch;

        /// <summary>
        /// Determine if the them is dark mode
        /// </summary>
        private bool isDarkMode;

        /// <summary>
        /// The font size
        /// </summary>
        private int fontSize;

        #endregion

        #region Public Properties

        /// <summary>
        /// List of all the installed voice on this pc
        /// </summary>
        public ReadOnlyCollection<InstalledVoice> InstalledVoices { get; set; }

        /// <summary>
        /// The selected voice to be used in the reading 
        /// </summary>
        public InstalledVoice SelectedVoice
        {
            get => selectedVoice;
            set
            {
                selectedVoice = value;
                UpdateSelectedVoice(selectedVoice);
            }
        }

        /// <summary>
        /// The reading speed 
        /// </summary>
        public double ReadingSpeed
        {
            get => readingSpeed;
            set
            {
                if (value == 0)
                    return;
                readingSpeed = value;
                UpdateReadingSpeed(readingSpeed);
            }
        }

        /// <summary>
        /// The voice pitch
        /// </summary>
        public double VoicePitch
        {
            get => voicePitch;
            set
            {
                if (value == 0)
                    return;
                voicePitch = value;
                UpdateVoicePitch(voicePitch);
            }
        }

        /// <summary>
        /// The trigger to enable darkmode
        /// </summary>
        public bool IsDarkMode
        {
            get { return isDarkMode; }
            set
            {
                isDarkMode = value;
                DI.UIManager.SetDarkMode(IsDarkMode);
            }
        }

        /// <summary>
        /// The setting to change teh font size
        /// </summary>
        public int FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
                DI.UIManager.SetFontSize(FontSize);
                DI.ViewModelApplication.CurrentBookViewModel?.SortCurrent();
            }
        }

        #region About

        /// <summary>
        /// The current version of the application
        /// </summary>
        public string PatchNotes { get; set; } = $"Epub Reader TTS v{Assembly.GetEntryAssembly().GetName().Version}";

        /// <summary>
        /// The text to click to go to the discord server
        /// </summary>
        public string DiscordText { get; set; } = $"Discord Server";

        /// <summary>
        /// The Uri to the discord server
        /// </summary>
        public Uri DiscordUri { get; set; } = new Uri("https://discord.gg/2Vqy8V");

        /// <summary>
        /// The text to click to send the auther an email
        /// </summary>
        public string EmailText { get; set; } = $"Send me an E-Mail";

        /// <summary>
        /// The Uri to send an E-mail
        /// </summary>
        public Uri EmailUri { get; set; } = new Uri("mailto:Epub-Reader-TTS@outlook.com?subject=FeedBack");

        /// <summary>
        /// The text to click to go to the app website
        /// </summary>
        public string WebsiteText { get; set; } = $"Go to website: epubreadertts.tech";

        /// <summary>
        /// The Uri to the website
        /// </summary>
        public Uri WebsiteUri { get; set; } = new Uri("https://epubreadertts.tech");

        #endregion

        #endregion

        #region Default Constructor

        public SettingsViewModel()
        {
            this.isDarkMode = DI.SettingsManager.IsDarkMode();

            this.fontSize = DI.SettingsManager.GetFontSize();

            InstalledVoices = new ReadOnlyCollection<InstalledVoice>(DI.SpeechSynthesizer.GetInstalledVoices().ToList());

            var loadedVoice = DI.SettingsManager.GetSelectedVoice();

            if (string.IsNullOrEmpty(loadedVoice))
                loadedVoice = InstalledVoices.First().DisplayName;

            this.selectedVoice = InstalledVoices.First(v => v.DisplayName == loadedVoice);

            this.readingSpeed = DI.SettingsManager.GetReadingSpeed();

            this.voicePitch = DI.SettingsManager.GetVoicePitch();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Select voice based on the installed voice
        /// </summary>
        /// <param name="selectedVoice">The selected voice</param>
        private void UpdateSelectedVoice(InstalledVoice selectedVoice)
        {
            DI.SpeechSynthesizer.SelectVoice(selectedVoice.DisplayName);

            DI.SettingsManager.SetSelectedVoice(selectedVoice.DisplayName);
        }

        /// <summary>
        /// Update the speed of reading
        /// </summary>
        /// <param name="readingSpeed">the speed of reading</param>
        private void UpdateReadingSpeed(double readingSpeed)
        {
            DI.SpeechSynthesizer.Rate = readingSpeed;

            DI.SettingsManager.SetReadingSpeed(readingSpeed);
        }

        /// <summary>
        /// Update the pitch of the voice
        /// </summary>
        /// <param name="voicePitch">the pithc of the voice</param>
        private void UpdateVoicePitch(double voicePitch)
        {
            DI.SettingsManager.SetVoicePitch(voicePitch);

            DI.SpeechSynthesizer.Pitch = voicePitch;
        }

        #endregion
    }
}
