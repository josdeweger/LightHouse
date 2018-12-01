using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using LightHouse.Lib;

namespace LightHouse.UwpApp
{
    public class ResultToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var build = (Build)value;

            switch (build.Result)
            {
                case BuildResult.Failed:
                    return Colors.Red;
                case BuildResult.PartiallySucceeded:
                case BuildResult.None:
                    return Colors.Orange;
                case BuildResult.Canceled:
                    return Colors.White;
                case BuildResult.Succeeded:
                    return Colors.Green;
                default:
                    return Colors.Orange;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}