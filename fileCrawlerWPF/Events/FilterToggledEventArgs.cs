using fileCrawlerWPF.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace fileCrawlerWPF.Events
{
    public class FilterToggledEventArgs
        : RoutedEventArgs
    {
        public FilterContext Context { get; }
        public bool IsEnabled { get; }


        public FilterToggledEventArgs(FilterContext ctx, bool val)
        {
            Context = ctx;
            IsEnabled = val;
        }

    }
}
