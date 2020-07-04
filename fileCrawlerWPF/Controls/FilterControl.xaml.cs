using fileCrawlerWPF.Events;
using fileCrawlerWPF.Extensions;
using fileCrawlerWPF.Filters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace fileCrawlerWPF.Controls
{
    /// <summary>
    /// Interaction logic for FilterControl.xaml
    /// </summary>
    public partial class FilterControl : UserControl
    {
        private readonly Filterer _filterer;

        public event EventHandler<EventArgs> RequestFilter;
        public event EventHandler<FileSelectedEventArgs> FileSelected;

        public FilterControl()
        {
            InitializeComponent();

            DataContext = this;
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
            FilteredItems          .Clear();
            txtResolution          .Clear();
            txtFramerate           .Clear();
            txtVCodec              .Clear();
            txtACodec              .Clear();
            txtName.Document.Blocks.Clear();

            chkResolution.IsChecked = false;
            chkFrameRate.IsChecked  = false;
            chkVCodec.IsChecked     = false;
            chkACodec.IsChecked     = false;
            chkName.IsChecked       = false;

        }

        private IReadOnlyCollection<(FilterContext, object)> GetFilterContexts()
        {
            return new List<(FilterContext, object)>(5)
            {
                (FilterContext.Framerate, int.Parse(txtFramerate.Text == string.Empty ? "0" : txtFramerate.Text)),
                (FilterContext.Resolution, int.Parse(txtResolution.Text == string.Empty ? "0" : txtResolution.Text)),
                (FilterContext.Name, txtName.GetText()),
                (FilterContext.AudioCodec, txtACodec.Text),
                (FilterContext.VideoCodec, txtVCodec.Text)
            };
        }

        public void OnFilter(IReadOnlyCollection<ProbeFile> files, EventArgs e)
        {
            FilteredItems.Clear();

            var contexts = GetFilterContexts();

            var matches = _filterer.Filter(contexts, files);

            foreach(var match in matches)
            {
                FilteredItems.Add(match);
            }
        }

        private void lvFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // this should raise an event to the parent control to update the filter select info
            if (!(SelectedItem is null))
                FileSelected?.Invoke(this, new FileSelectedEventArgs(SelectedItem.ID));
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            // reset ui to default
            ResetView();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            // raise an event to request fresh media collection
            RequestFilter?.Invoke(this, e);

        }
        private void FilterCheckChanged(object sender, RoutedEventArgs e)
        {
            var cb = sender as FilterContextCheckBox;
            _filterer.ToggleFilter(cb.FilterContext, cb.IsChecked.Value);
        }

        private void Numerical_PreviewText(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}
