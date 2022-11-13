using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace KnittingAssistant.View.Converters
{
    class StringToPropertyValue : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            double source = Math.Round((double)value[0], 2);
            string text = (string)value[1];

            object result;
            if (double.TryParse(text, NumberStyles.AllowDecimalPoint, culture, out double target) && target == source)
                result = Binding.DoNothing;
            else
                result = source.ToString(culture);

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            object result = null;
            string text = (string)value;

            if (string.IsNullOrWhiteSpace(text))
                result = 0.0;
            else if (double.TryParse(text, NumberStyles.AllowDecimalPoint, culture, out double target))
                result = target;

            if (result == null)
                return null;

            result = Math.Round((double)result, 2);
            return new object[] { result };
        }
    }
}
