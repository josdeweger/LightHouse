using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Lighthouse.UI.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public static EnumToBooleanConverter Instance = new EnumToBooleanConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return value.Equals(parameter);
            }
            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return (bool)value ? parameter : AvaloniaProperty.UnsetValue;
            }
            return AvaloniaProperty.UnsetValue;
        }
    }
}