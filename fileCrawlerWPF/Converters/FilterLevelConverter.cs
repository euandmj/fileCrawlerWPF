using fileCrawlerWPF.Filters;
using System;
using System.Globalization;
using System.Windows.Data;

namespace fileCrawlerWPF.Converters
{
    [ValueConversion(typeof(FilterLevel), typeof(string))]
    class FilterLevelConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (FilterLevel)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (FilterLevel)value;
        }
    }
}
