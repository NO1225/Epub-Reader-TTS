namespace Epub_Reader_TTS
{
    /// <summary>
    /// Settings manager to manage the storing and retreaving of the settings valuess
    /// </summary>
    public class SettingsManager : ISettingsManager
    {
        #region Private Methods

        /// <summary>
        /// Save the settings
        /// </summary>
        private void SaveChanges() => Properties.Settings.Default.Save();

        #endregion

        #region Font Size

        /// <summary>
        /// Get the saved font size
        /// </summary>
        /// <returns>font size</returns>
        public int GetFontSize()
        {
            return (int)Properties.Settings.Default.FontSize;
        }

        /// <summary>
        /// Store font size to be saved
        /// </summary>
        /// <param name="value">font size</param>
        public void SetFontSize(int value)
        {
            Properties.Settings.Default.FontSize = value;
            SaveChanges();
        }

        #endregion

        #region Selected Voice

        /// <summary>
        /// Get the saved voice
        /// </summary>
        /// <returns>the saved voice</returns>
        public string GetSelectedVoice()
        {
            return (string)Properties.Settings.Default.SelectedVoice;
        }

        /// <summary>
        /// Store a voice to be saved
        /// </summary>
        /// <param name="value">voice name</param>
        public void SetSelectedVoice(string value)
        {
            Properties.Settings.Default.SelectedVoice = value;
            SaveChanges();
        }

        #endregion

        #region Reading Speed

        /// <summary>
        /// Get the saved reading speed
        /// </summary>
        /// <returns>reeding speed</returns>
        public int GetReadingSpeed()
        {
            return (int)Properties.Settings.Default.ReadingSpeed;
        }

        /// <summary>
        /// Store reading speed to be saved 
        /// </summary>
        /// <param name="value">reading speed</param>
        public void SetReadingSpeed(int value)
        {
            Properties.Settings.Default.ReadingSpeed = value;
            SaveChanges();
        }

        #endregion

        #region Dark Mode

        /// <summary>
        /// To check if the saved settings is a dark mode
        /// </summary>
        /// <returns>dark mode is true, false is the light mode</returns>
        public bool IsDarkMode()
        {
            return (bool)Properties.Settings.Default.DarkMode;
        }

        /// <summary>
        /// Store the settings for the dark mode
        /// </summary>
        /// <param name="value">dark mode is true, false is the light mode</param>
        public void SetDarkMode(bool value)
        {
            Properties.Settings.Default.DarkMode = value;
            SaveChanges();
        }

        #endregion

    }
}
