﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace MindWeaveClient.Helpers
{
    public class DifficultyIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id && id > 0) return id - 1;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index && index >= 0) return index + 1;
            return 1;
        }
    }
}