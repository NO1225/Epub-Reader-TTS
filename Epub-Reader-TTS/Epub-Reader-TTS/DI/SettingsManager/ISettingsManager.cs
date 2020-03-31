using System.Threading.Tasks;

namespace Epub_Reader_TTS
{
    public interface ISettingsManager
    {
        Task Initiate();

        int GetFontSize();
        double GetReadingSpeed();
        double GetVoicePitch();
        string GetSelectedVoice();
        bool IsDarkMode();
        bool AskToAssosiate();

        Task SetDarkMode(bool value);
        Task SetFontSize(int value);
        Task SetReadingSpeed(double value);
        Task SetVoicePitch(double value);
        Task SetSelectedVoice(string value);
        Task SetAskToAssosiate(bool value);
    }
}