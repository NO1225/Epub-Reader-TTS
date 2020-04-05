using System.Windows;
using System.Windows.Media;

namespace Epub_Reader_TTS
{
    public class UIManager : IUIManager
    {
        #region Private Fields

       Color primaryFontColor;
       Color secondaryFontColor;
       Color primaryBackGroundColor;
       Color secondaryBackGroundColor;

       Color accentColor1;
       Color dimOverlayColor;

        SolidColorBrush primaryFontColorBrush;
        SolidColorBrush secondaryFontColorBrush;
        SolidColorBrush primaryBackGroundColorBrush;
        SolidColorBrush secondaryBackGroundColorBrush;

        SolidColorBrush accentColor1Brush;
        SolidColorBrush dimOverlayColorBrush;

        #endregion

        /// <summary>
        /// Change the selected theme and save it to the settings file 
        /// </summary>
        /// <param name="isDarkMode"></param>
        public void SetDarkMode(bool isDarkMode)
        {
            UpdateDarkMode(isDarkMode);

            DI.TaskManager.Run(() => DI.SettingsManager.SetDarkMode(isDarkMode));
        }

        /// <summary>
        /// Update the UI according to the selected theme
        /// </summary>
        /// <param name="isDarkMode"></param>
        public void UpdateDarkMode(bool isDarkMode)
        {
            if (isDarkMode)
            {
                primaryFontColor = (Color)ColorConverter.ConvertFromString("#ffffff");
                secondaryFontColor = (Color)ColorConverter.ConvertFromString("#f0f0f0");
                primaryBackGroundColor = (Color)ColorConverter.ConvertFromString("#000000");
                secondaryBackGroundColor = (Color)ColorConverter.ConvertFromString("#2b2b2b");

                accentColor1 = (Color)ColorConverter.ConvertFromString("#3baaf5");
                dimOverlayColor = (Color)ColorConverter.ConvertFromString("#33A4B0FF");
            }
            else
            {
                primaryFontColor = (Color)ColorConverter.ConvertFromString("#000000");
                secondaryFontColor = (Color)ColorConverter.ConvertFromString("#2b2b2b");
                primaryBackGroundColor = (Color)ColorConverter.ConvertFromString("#ffffff");
                secondaryBackGroundColor = (Color)ColorConverter.ConvertFromString("#f0f0f0");

                accentColor1 = (Color)ColorConverter.ConvertFromString("#00569f");
                dimOverlayColor = (Color)ColorConverter.ConvertFromString("#33292D44");
            }

            primaryFontColorBrush = new SolidColorBrush(primaryFontColor);
            secondaryFontColorBrush = new SolidColorBrush(secondaryFontColor);
            primaryBackGroundColorBrush = new SolidColorBrush(primaryBackGroundColor);
            secondaryBackGroundColorBrush = new SolidColorBrush(secondaryBackGroundColor);

            accentColor1Brush = new SolidColorBrush(accentColor1);
            dimOverlayColorBrush = new SolidColorBrush(dimOverlayColor);

            Application.Current.Resources["PrimaryFontColor"] = primaryFontColor;
            Application.Current.Resources["SecondaryFontColor"] = secondaryFontColor;
            Application.Current.Resources["PrimaryBackGroundColor"] = primaryBackGroundColor;
            Application.Current.Resources["SecondaryBackGroundColor"] = secondaryBackGroundColor;

            Application.Current.Resources["AccentColor1"] = accentColor1;
            Application.Current.Resources["DimOverlayColor"] = dimOverlayColor;

            Application.Current.Resources["PrimaryFontColorBrush"] = primaryFontColorBrush;
            Application.Current.Resources["SecondaryFontColorBrush"] = secondaryFontColorBrush;
            Application.Current.Resources["PrimaryBackGroundColorBrush"] = primaryBackGroundColorBrush;
            Application.Current.Resources["SecondaryBackGroundColorBrush"] = secondaryBackGroundColorBrush;

            Application.Current.Resources["AccentColor1Brush"] = accentColor1Brush;
            Application.Current.Resources["DimOverlayColorBrush"] = dimOverlayColorBrush;
        }

        /// <summary>
        /// Change the font size and save it to the settings file
        /// </summary>
        /// <param name="fontSize"></param>
        public void SetFontSize(int fontSize)
        {
            UpdateFontSize(fontSize);

            DI.TaskManager.Run(() => DI.SettingsManager.SetFontSize(fontSize));
        }

        /// <summary>
        /// Update the font in the UI 
        /// </summary>
        /// <param name="fontSize"></param>
        public void UpdateFontSize(int fontSize)
        {
            Application.Current.Resources["MainFontSize"] = (double)fontSize;
        }
    }
}
