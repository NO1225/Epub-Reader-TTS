using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Convert the <see cref="ApplicationPage"/> to an actual view/Page
    /// </summary>
    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            switch ((ApplicationPage)value)
            {
                case ApplicationPage.Dashboard:
                    return new DashboardPage();
                case ApplicationPage.Book:
                    return new BookPage(parameter as BookViewModel);
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
