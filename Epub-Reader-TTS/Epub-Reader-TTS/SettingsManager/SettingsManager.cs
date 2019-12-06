namespace Epub_Reader_TTS
{
    public class SettingsManager
    {
        #region Font Size

        public int GetFontSize()
        {
            return (int)Properties.Settings.Default["FontSize"];
        }

        public void SetFontSize(int value)
        {
            Properties.Settings.Default["FontSize"] = value;
        }

        #endregion

        #region Selected Voice

        public string GetSelectedVoice()
        {
            return (string)Properties.Settings.Default["SelectedVoice"];
        }

        public void SetSelectedVoice(string value)
        {
            Properties.Settings.Default["SelectedVoice"] = value;
        }

        #endregion

        #region Reading Speed

        public int GetReadingSpeed()
        {
            return (int)Properties.Settings.Default["ReadingSpeed"];
        }

        public void SetReadingSpeed(int value)
        {
            Properties.Settings.Default["ReadingSpeed"] = value;
        }

        #endregion

    }
}
