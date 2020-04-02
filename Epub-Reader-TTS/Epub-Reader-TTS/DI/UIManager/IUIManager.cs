namespace Epub_Reader_TTS
{
    public interface IUIManager
    {
        void SetDarkMode(bool isDarkMode);
        void SetFontSize(int fontSize);
        void UpdateDarkMode(bool isDarkMode);
        void UpdateFontSize(int fontSize);
    }
}