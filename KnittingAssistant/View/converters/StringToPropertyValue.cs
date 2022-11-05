using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace KnittingAssistant.View.Converters
{
    class StringToPropertyValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)(double)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
