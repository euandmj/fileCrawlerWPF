using fileCrawlerWPF.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace fileCrawlerWPF.Controls
{
    /// <summary>
    /// Interaction logic for FilterControl.xaml
    /// </summary>
    public partial class FilterControl : UserControl
    {
        private Filterer Filterer;

        public FilterControl()
        {
            InitializeComponent();

            Filterer = new Filterer();
            FilteredItems = new List<ListViewItem>();
        }

        public List<ListViewItem> FilteredItems { get; set; }
    }
}
