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
        private static DashboardPage dashboardPage;

        public static BookPage bookPage = new BookPage(null,true);

        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            switch ((ApplicationPage)value)
            {
                case ApplicationPage.Dashboard:
                    ViewModelLocator.DashboardViewModel.Initiate().GetAwaiter().GetResult();

                    if(dashboardPage!=null)
                    {
                        dashboardPage.DataContext = null;
                        dashboardPage.DataContext = ViewModelLocator.DashboardViewModel;
                    }
                    else
                    {
                        dashboardPage = new DashboardPage(ViewModelLocator.DashboardViewModel);
                    }

                    return dashboardPage;

                case ApplicationPage.Book:
                    bookPage.DataContext = null;
                    bookPage.DataContext = ViewModelLocator.ApplicationViewModel.CurrentBookViewModel;
                    return bookPage;

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
