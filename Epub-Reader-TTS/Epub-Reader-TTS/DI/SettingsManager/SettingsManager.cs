namespace Epub_Reader_TTS
{
    public class SettingsManager : ISettingsManager
    {
        private void SaveChanges() => Properties.Settings.Default.Save();


        #region Font Size

        public int GetFontSize()
        {
            return (int)Properties.Settings.Default.FontSize;
        }

        public void SetFontSize(int value)
        {
            Properties.Settings.Default.FontSize = value;
            SaveChanges();
        }

        #endregion

        #region Selected Voice

        public string GetSelectedVoice()
        {
            return (string)Properties.Settings.Default.SelectedVoice;
        }

        public void SetSelectedVoice(string value)
        {
            Properties.Settings.Default.SelectedVoice = value;
            SaveChanges();
        }

        #endregion

        #region Reading Speed

        public int GetReadingSpeed()
        {
            return (int)Properties.Settings.Default.ReadingSpeed;
        }

        public void SetReadingSpeed(int value)
        {
            Properties.Settings.Default.ReadingSpeed = value;
            SaveChanges();
        }

        #endregion

        #region Dark Mode

        public bool IsDarkMode()
        {
            return (bool)Properties.Settings.Default.DarkMode;
        }

        public void SetDarkMode(bool value)
        {
            Properties.Settings.Default.DarkMode = value;
            SaveChanges();
        }

        #endregion

    }
}
