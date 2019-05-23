using System;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Generic;
using Młynek.Model;


namespace Młynek.View.Converters
{
    class FieldStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FieldState currentState = (FieldState)value;
            string state = "Not assigned";
            switch (currentState)
            {
                case FieldState.White:
                    state = "White";
                    break;
                case FieldState.Black:
                    state = "Black";
                    break;
                default:
                    state = "Empty";
                    break;
            }

            return state;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
