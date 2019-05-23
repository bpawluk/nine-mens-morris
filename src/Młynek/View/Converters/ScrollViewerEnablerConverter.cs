using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Młynek.View.Converters
{
    class ScrollViewerEnablerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double minSize = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            double actualSize = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
            return actualSize <= minSize ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
