using fileCrawlerWPF.Controls.model;
using fileCrawlerWPF.Media;
using System;
using System.Globalization;
using System.Windows.Data;

namespace fileCrawlerWPF.Converters
{
    [ValueConversion(typeof(ProbeFile), typeof(bool))]
    public class FileNullConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var file = value as FileInfoModel;

            return !(file == null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
