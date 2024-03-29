﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace KnittingAssistant.View.Converters
{
    class StringToPropertyValue : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            double source;
            if (int.TryParse((string)parameter, out int decimalPartLength))
                source = Math.Round((double)value[0], decimalPartLength);
            else
                source = Math.Round((double)value[0], 2);
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

            if (int.TryParse((string)parameter, out int decimalPartLength))
                result = Math.Round((double)result, decimalPartLength);
            else
                result = Math.Round((double)result, 2);

            return new object[] { result };
        }
    }
}
