using fileCrawlerWPF.Events;
using System;

namespace fileCrawlerWPF.Filters
{
    interface IFilterOption
    {
        event EventHandler<FilterToggledEventArgs> FilterToggled;

        bool IsNumeric { get; set; }
        FilterContext FilterContext { get; set; }
        string FilterName { get; set; }
        string Value { get; }

        void Reset();
        T GetValue<T>();
    }
}
