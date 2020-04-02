using Epub_Reader_TTS.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

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
        private string tmpLocation = "tmp.json";

        bool saving = false;

        bool waiting = false;

        #endregion

        #region Private Methods

        /// <summary>
        /// Save the settings
        /// </summary>
        private async Task SaveChanges() 
        {
            if(saving)
            {
                waiting = true;
            }
            else
            {
                saving = true;
                await Save();
                saving = false;
                waiting = false;
            }
            
            Debug.WriteLine("Initilizing Saving");
            
        }

        private async Task Save()
        {
            var tmp = userSettings;
            using (FileStream fs = File.Create(tmpLocation))
            {
                await JsonSerializer.SerializeAsync(fs, userSettings);
            }
            File.Copy(tmpLocation, fileLocation, true);
            Debug.WriteLine("Saving Done");

            if(waiting & !equalSettings(tmp))
            {
                await Save();
            }
        }

        private bool equalSettings(UserSettings userSettings)
        {
            if (userSettings.DarkMode != this.userSettings.DarkMode)
                return false;
            if (userSettings.DontAskToAssosiate != this.userSettings.DontAskToAssosiate)
                return false;
            if (userSettings.FontSize != this.userSettings.FontSize)
                return false;
            if (userSettings.ReadingSpeed != this.userSettings.ReadingSpeed)
                return false;
            if (userSettings.SelectedVoice != this.userSettings.SelectedVoice)
                return false;
            if (userSettings.VoicePitch != this.userSettings.VoicePitch)
                return false;

            return true;
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


        #region Assosiate File

        /// <summary>
        /// To check if the saved settings is a dark mode
        /// </summary>
        /// <returns>dark mode is true, false is the light mode</returns>
        public bool AskToAssosiate() => !userSettings.DontAskToAssosiate;

        /// <summary>
        /// Store the settings for the dark mode
        /// </summary>
        /// <param name="value">dark mode is true, false is the light mode</param>
        public async Task SetAskToAssosiate(bool value)
        {
            userSettings.DontAskToAssosiate = !value;
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

            //MessageBox.Show($"Initialized {userSettings}");
            Debug.WriteLine("Initilizing Done");

        }

        #endregion
    }
}
