using System;
using Windows.UI.Xaml.Data;

namespace SmartShade.Converters
{
    class BoolToButtonStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "Pressed" : "Released";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
