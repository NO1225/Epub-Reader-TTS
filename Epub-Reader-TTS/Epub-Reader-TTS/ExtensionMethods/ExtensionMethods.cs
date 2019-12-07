using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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
        public static byte[] ToByteArray(this Image bmp, bool transperant = false)
        {
            if (transperant)
            {
                var converter = new ImageConverter();
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
        public static Image ToImage(this byte[] byteArrayIn)
        {
            // Using the system converter ...
            System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
            Image img = byteArrayIn != null ? (Image)converter.ConvertFrom(byteArrayIn) : null;
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
        /// Easier way to use the color directory
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Color ToColor(this Dictionary<string, string> dictionary, string key)
        {
            return ColorTranslator.FromHtml(dictionary[key]);
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
