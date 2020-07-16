using fileCrawlerWPF.Events;
using fileCrawlerWPF.Filters;
using fileCrawlerWPF.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace fileCrawlerWPF.Controls
{
    public partial class FilterControl : UserControl
    {
        private readonly Filterer _filterer;

        public event EventHandler Clear;
        public event EventHandler<FileSelectedEventArgs> FileSelected;

        public FilterControl()
        {
            InitializeComponent();


            _filterer = new Filterer();
            FilteredItems = new ObservableCollection<ProbeFile>();
        } 


        public ObservableCollection<ProbeFile> FilteredItems { get; set; }


        public ProbeFile SelectedItem
        {
            get
            {
                try
                {
                    return lvFilter.SelectedItem is null
                        ? null
                        : (ProbeFile)lvFilter.SelectedItem;
                }
                catch (InvalidCastException) { return null; }
            }
        }

        private void ResetView()
        {
            FilteredItems.Clear();
            foreach(var opt in Grid.Children.OfType<IFilterOption>())
            {
                opt.Reset();
            }
            Clear?.Invoke(this, EventArgs.Empty);
        }

        private IReadOnlyCollection<(FilterContext, object)> GetFilterContexts()
        {
            return new List<(FilterContext, object)>(5)
            {
                // @todo i dont like this
                (Filter_Res.FilterContext, Filter_Res.GetValue<int>()),
                (Filter_Frames.FilterContext, Filter_Frames.GetValue<int>()),
                (Filter_VCodec.FilterContext, Filter_VCodec.GetValue<string>()),
                (Filter_ACodec.FilterContext, Filter_ACodec.GetValue<string>()),
                (Filter_Name.FilterContext, Filter_Name.GetValue()),
                (Filter_Extension.FilterContext, Filter_Extension.GetValue<string>())
            };
        }

        private void OnFilter(IReadOnlyCollection<ProbeFile> files)
        {
            try
            {
                FilteredItems.Clear();

                var contexts = GetFilterContexts();

                var matches = _filterer.Filter(contexts, files);

                foreach (var match in matches)
                {
                    FilteredItems.Add(match);
                }
            }
            catch (OverflowException)
            {
                ResetView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error scanning folder",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lvFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(SelectedItem is null))
                FileSelected?.Invoke(this, new FileSelectedEventArgs(SelectedItem.ID));
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ResetView();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            OnFilter(MediaManager.RequestFilter());
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // export results

        }        

        private void FilterOption_FilterToggled(object sender, FilterToggledEventArgs e)
        {
            _filterer.ToggleFilter(e.Context, e.IsEnabled);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
            comboFilterLevel.ItemsSource = Enum.GetValues(typeof(FilterLevel));
            comboFilterLevel.DataContext = _filterer;
        }
    }
}
