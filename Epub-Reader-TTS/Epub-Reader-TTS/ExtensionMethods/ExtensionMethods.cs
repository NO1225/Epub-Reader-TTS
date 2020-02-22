using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Extension methods to hel in the application
    /// </summary>
    public static class ExtensionMethods
    {
        #region ExtensionMethods

        /// <summary>
        /// Convert image to byte array
        /// </summary>
        /// <param name="bmp">The image to be converted</param>
        /// <returns></returns>
        public static byte[] ToByteArray(this System.Drawing.Image bmp, bool transperant = false)
        {
            if (transperant)
            {
                var converter = new System.Drawing.ImageConverter();
                var imageBytes = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
                return imageBytes;
            }
            //initialize quality array
            Int64[] qual = { 100L, 90L, 80L, 70L, 60L, 50L, 40L, 30L, 20L, 10L };
            ImageCodecInfo jpgEncoder = GetEncoder("image/jpeg");
            var myEncoder = System.Drawing.Imaging.Encoder.Quality;
            var encParameters = new EncoderParameters(1);
            int qualindex = 0;

            byte[] ret;
            do
            {
                //generate for selected quality
                var param = new EncoderParameter(myEncoder, qual[qualindex]);
                qualindex++;
                encParameters.Param[0] = param;

                var str = new MemoryStream();
                bmp.Save(str, jpgEncoder, encParameters);
                ret = str.ToArray();

            } while (ret.Length > 7900 && qualindex < 8); //lower quality and check if the size doesn't exceeded SQL Server CE limitations

            return ret;
        }

        /// <summary>
        /// Convert byte array to image 
        /// </summary>
        /// <param name="byteArrayIn">The byte array to be converted</param>
        /// <returns></returns>
        public static System.Drawing.Image ToImage(this byte[] byteArrayIn)
        {
            // Using the system converter ...
            System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
            System.Drawing.Image img = byteArrayIn != null ? (System.Drawing.Image)converter.ConvertFrom(byteArrayIn) : null;
            return img;
        }

        /// <summary>
        /// Change the brightness of the color with a given amount
        /// </summary>
        /// <param name="color"></param>
        /// <param name="factor"></param>
        /// <returns>New color with the added / removed brightness</returns>
        public static System.Windows.Media.Color ChangeBrightness(this System.Windows.Media.Color color, int factor)
        {
            int R = (color.R + factor > 255) ? 255 : (color.R + factor < 0) ? 0 : color.R + factor;
            int G = (color.G + factor > 255) ? 255 : (color.G + factor < 0) ? 0 : color.G + factor;
            int B = (color.B + factor > 255) ? 255 : (color.B + factor < 0) ? 0 : color.B + factor;

            return System.Windows.Media.Color.FromRgb((byte)R, (byte)G, (byte)B);
        }
        
        /// <summary>
        /// Change the brightness of the color with a given amount
        /// </summary>
        /// <param name="color"></param>
        /// <param name="factor"></param>
        /// <returns>New color with the added / removed brightness</returns>
        public static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromRgb((byte)color.R, (byte)color.G, (byte)color.B);
        }

        public static T FindDescendant<T>(this DependencyObject d) where T : DependencyObject
        {
            if (d == null)
                return null;

            var childCount = VisualTreeHelper.GetChildrenCount(d);

            for (var i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);

                var result = child as T ?? FindDescendant<T>(child);

                if (result != null)
                    return result;
            }

            return null;
        }

        public static double GetParagraphHeight(this string paragraph, double width, double fontSize, string fontFamily = "OpenSansRegular")
        {
            var formattedText = new FormattedText(paragraph, 
                CultureInfo.CurrentCulture, 
                FlowDirection.LeftToRight, 
                new Typeface((FontFamily)Application.Current.Resources[fontFamily], new FontStyle(),new FontWeight(), new FontStretch()), 
                fontSize,
                Brushes.Black, 
                new NumberSubstitution(), 1);

            formattedText.MaxTextWidth = width;

            return formattedText.Height;

        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// The encoder to be used in image converting
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetEncoder(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        #endregion
    }
}
