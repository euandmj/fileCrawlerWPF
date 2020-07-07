using fileCrawlerWPF.Media;
using fileCrawlerWPF.Util;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace fileCrawlerWPF.Converters
{
    [ValueConversion(typeof(ObservableCollection<FileDirectory>), typeof(TreeViewItem))]
    public class MediaTreeViewConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ObservableCollection<FileDirectory> m)) return null;
            if (m.Count() == 0) return new TreeViewItem();
            return DirectoryProcessor.BuildFileTree(m);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
