using fileCrawlerWPF.Events;
using fileCrawlerWPF.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace fileCrawlerWPF.Controls
{
    abstract class FilterOptionBase
        : UserControl, IFilterOption
    {
        public abstract bool IsNumeric { get; set; }
        public abstract FilterContext FilterContext { get; set; }
        public abstract string FilterName { get; set; }
        public abstract string Value { get; }

        public abstract event EventHandler<FilterToggledEventArgs> FilterToggled;

        public virtual T GetValue<T>()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                return default;
            }
            else
            {
                return (T)Convert.ChangeType(Value, typeof(T));
            }
        }

        public abstract void Reset();
    }
}
