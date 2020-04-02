using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Epub_Reader_TTS
{
    public class UIManager : IUIManager
    {
        #region Private Fields

        System.Windows.Media.Color primaryFontColor;
        System.Windows.Media.Color secondaryFontColor;
        System.Windows.Media.Color primaryBackGroundColor;
        System.Windows.Media.Color secondaryBackGroundColor;

        System.Windows.Media.Color accentColor1;

        SolidColorBrush primaryFontColorBrush;
        SolidColorBrush secondaryFontColorBrush;
        SolidColorBrush primaryBackGroundColorBrush;
        SolidColorBrush secondaryBackGroundColorBrush;

        SolidColorBrush accentColor1Brush;

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
                primaryFontColor = ColorTranslator.FromHtml("#ffffff").ToMediaColor();
                secondaryFontColor = ColorTranslator.FromHtml("#f0f0f0").ToMediaColor();
                primaryBackGroundColor = ColorTranslator.FromHtml("#000000").ToMediaColor();
                secondaryBackGroundColor = ColorTranslator.FromHtml("#2b2b2b").ToMediaColor();

                accentColor1 = ColorTranslator.FromHtml("#3baaf5").ToMediaColor();

                primaryFontColorBrush = new SolidColorBrush(primaryFontColor);
                secondaryFontColorBrush = new SolidColorBrush(secondaryFontColor);
                primaryBackGroundColorBrush = new SolidColorBrush(primaryBackGroundColor);
                secondaryBackGroundColorBrush = new SolidColorBrush(secondaryBackGroundColor);

                accentColor1Brush = new SolidColorBrush(accentColor1);
            }
            else
            {
                primaryFontColor = ColorTranslator.FromHtml("#000000").ToMediaColor();
                secondaryFontColor = ColorTranslator.FromHtml("#2b2b2b").ToMediaColor();
                primaryBackGroundColor = ColorTranslator.FromHtml("#ffffff").ToMediaColor();
                secondaryBackGroundColor = ColorTranslator.FromHtml("#f0f0f0").ToMediaColor();

                accentColor1 = ColorTranslator.FromHtml("#00569f").ToMediaColor();

                primaryFontColorBrush = new SolidColorBrush(primaryFontColor);
                secondaryFontColorBrush = new SolidColorBrush(secondaryFontColor);
                primaryBackGroundColorBrush = new SolidColorBrush(primaryBackGroundColor);
                secondaryBackGroundColorBrush = new SolidColorBrush(secondaryBackGroundColor);

                accentColor1Brush = new SolidColorBrush(accentColor1);
            }

            Application.Current.Resources["PrimaryFontColor"] = primaryFontColor;
            Application.Current.Resources["SecondaryFontColor"] = secondaryFontColor;
            Application.Current.Resources["PrimaryBackGroundColor"] = primaryBackGroundColor;
            Application.Current.Resources["SecondaryBackGroundColor"] = secondaryBackGroundColor;

            Application.Current.Resources["AccentColor1"] = accentColor1;

            Application.Current.Resources["PrimaryFontColorBrush"] = primaryFontColorBrush;
            Application.Current.Resources["SecondaryFontColorBrush"] = secondaryFontColorBrush;
            Application.Current.Resources["PrimaryBackGroundColorBrush"] = primaryBackGroundColorBrush;
            Application.Current.Resources["SecondaryBackGroundColorBrush"] = secondaryBackGroundColorBrush;

            Application.Current.Resources["AccentColor1Brush"] = accentColor1Brush;
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
