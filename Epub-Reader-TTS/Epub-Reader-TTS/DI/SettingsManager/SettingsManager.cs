using Epub_Reader_TTS.Core;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Settings manager to manage the storing and retreaving of the settings valuess
    /// </summary>
    public class SettingsManager : ISettingsManager
    {

        #region Private Fields

        private UserSettings userSettings;

        private string fileLocation = "usersettings.json";

        bool saving = false;

        private object _lockObject = new object();

        #endregion


        #region Private Methods

        /// <summary>
        /// Save the settings
        /// </summary>
        private async Task SaveChanges() 
        {
            while (saving) { }
            Debug.WriteLine("Initilizing Saving");
            saving = true;
            lock(_lockObject)
            {
                using (FileStream fs = File.Create(fileLocation))
                {
                    JsonSerializer.SerializeAsync(fs, userSettings);
                }
                Debug.WriteLine("Saving DOne");
            }            
            saving = false;
        }

        #endregion

        #region Font Size

        /// <summary>
        /// Get the saved font size
        /// </summary>
        /// <returns>font size</returns>
        public int GetFontSize() => userSettings.FontSize;

        /// <summary>
        /// Store font size to be saved
        /// </summary>
        /// <param name="value">font size</param>
        public async Task SetFontSize(int value)
        {
            //Properties.Settings.Default.FontSize = value;
            userSettings.FontSize = value;
            await SaveChanges();
        }

        #endregion

        #region Selected Voice

        /// <summary>
        /// Get the saved voice
        /// </summary>
        /// <returns>the saved voice</returns>
        public string GetSelectedVoice() => userSettings.SelectedVoice;

        /// <summary>
        /// Store a voice to be saved
        /// </summary>
        /// <param name="value">voice name</param>
        public async Task SetSelectedVoice(string value)
        {
            userSettings.SelectedVoice = value;
            await SaveChanges();
        }

        #endregion

        #region Reading Speed

        /// <summary>
        /// Get the saved reading speed
        /// </summary>
        /// <returns>reeding speed</returns>
        public double GetReadingSpeed() => userSettings.ReadingSpeed;

        /// <summary>
        /// Store reading speed to be saved 
        /// </summary>
        /// <param name="value">reading speed</param>
        public async Task SetReadingSpeed(double value)
        {
            userSettings.ReadingSpeed = value;
            await SaveChanges();
        }

        #endregion
        
        #region Voice Pitch

        /// <summary>
        /// Get the saved voice pitch
        /// </summary>
        /// <returns>reeding speed</returns>
        public double GetVoicePitch() => userSettings.VoicePitch;

        /// <summary>
        /// Store voice pitch to be saved 
        /// </summary>
        /// <param name="value">reading speed</param>
        public async Task SetVoicePitch(double value)
        {
            userSettings.VoicePitch = value;
            await SaveChanges();
        }

        #endregion

        #region Dark Mode

        /// <summary>
        /// To check if the saved settings is a dark mode
        /// </summary>
        /// <returns>dark mode is true, false is the light mode</returns>
        public bool IsDarkMode() => userSettings.DarkMode;

        /// <summary>
        /// Store the settings for the dark mode
        /// </summary>
        /// <param name="value">dark mode is true, false is the light mode</param>
        public async Task SetDarkMode(bool value)
        {
            userSettings.DarkMode = value;
            await SaveChanges();
        }

        #endregion

        #region Initiation

        public async Task Initiate()
        {
            Debug.WriteLine("Initilizing Settings");

            if(File.Exists(fileLocation))
            {
                using (FileStream fs = File.OpenRead(fileLocation))
                {
                    if(fs.Length>1)
                        userSettings = await JsonSerializer.DeserializeAsync<UserSettings>(fs);
                    else
                        userSettings = new UserSettings();
                }
            }
            else
            {
                userSettings = new UserSettings();
                await SaveChanges();
            }


            Debug.WriteLine("Initilizing Done");

        }

        #endregion
    }
}
