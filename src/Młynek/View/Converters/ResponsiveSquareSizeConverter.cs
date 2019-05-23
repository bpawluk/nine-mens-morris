using System;
using System.Globalization;
using System.Windows.Data;

namespace Młynek.View.Converters
{
    public class ResponsiveSquareSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string[] parameters = ((string)parameter).Split(';');
            double width = System.Convert.ToDouble(values[0], CultureInfo.InvariantCulture);
            double height = System.Convert.ToDouble(values[1], CultureInfo.InvariantCulture);
            double value = height < width ? height : width;
            double toAdd = parameters.Length > 1 ? System.Convert.ToDouble(parameters[1], CultureInfo.InvariantCulture) : 0;
            return value * System.Convert.ToDouble(parameters[0], CultureInfo.InvariantCulture) + toAdd;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}