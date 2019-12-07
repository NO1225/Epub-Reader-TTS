namespace Epub_Reader_TTS
{
    public interface ISettingsManager
    {
        int GetFontSize();
        int GetReadingSpeed();
        string GetSelectedVoice();
        bool IsDarkMode();
        void SetDarkMode(bool value);
        void SetFontSize(int value);
        void SetReadingSpeed(int value);
        void SetSelectedVoice(string value);
    }
}