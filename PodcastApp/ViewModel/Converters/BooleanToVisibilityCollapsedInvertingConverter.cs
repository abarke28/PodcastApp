using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PodcastApp.ViewModel.Converters
{
    public class BooleanToVisibilityCollapsedInvertingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case true:
                    return "Collapsed";

                case false:
                    return "Visible";
            }

            return "Visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "Collapsed":
                    return true;

                case "Hidden":
                    return true;

                case "Visible":
                    return false;
            }

            return true;
        }
    }
}
