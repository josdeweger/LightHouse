using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Lighthouse.UI.Converters
{
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is string parameterString))
                return null;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return null;

            var parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is string parameterString))
                return null;

            return Enum.Parse(targetType, parameterString);
        }
    }
}