using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Convert the <see cref="ApplicationPage"/> to an actual view/Page
    /// </summary>
    public class AdditionalContentConverter : BaseValueConverter<AdditionalContentConverter>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {

            var addContent = (AdditionalContent)value;


            switch (addContent)
            {
                case AdditionalContent.Bookmarks:
                    return new BookmarkControl();
                case AdditionalContent.Settings:
                    return new SettingsControl();
                default:
                    break;
            }

            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
