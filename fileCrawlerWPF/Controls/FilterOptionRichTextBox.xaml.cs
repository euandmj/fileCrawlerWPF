using fileCrawlerWPF.Events;
using fileCrawlerWPF.Extensions;
using fileCrawlerWPF.Filters;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace fileCrawlerWPF.Controls
{
    /// <summary>
    /// Interaction logic for FilterOptionRichTextBox.xaml
    /// </summary>
    public partial class FilterOptionRichTextBox 
        : UserControl, IFilterOption
    {
        public event EventHandler<FilterToggledEventArgs> FilterToggled;        
        
        public FilterOptionRichTextBox()
        {
            InitializeComponent();

            DataContext = this;
        }
        public bool IsNumeric { get; set; }
        public FilterContext FilterContext { get; set; }
        public string FilterName { get; set; }
        public string Value => txtValue.GetText();

        public void Reset()
        {
            txtValue.Document.Blocks.Clear();
            chkCheck.IsChecked = false;
            CheckChanged(null, null);
        }

        public T GetValue<T>() 
        {
            return string.IsNullOrWhiteSpace(Value)
                ? (T)Convert.ChangeType(string.Empty, typeof(T))
                : (T)Convert.ChangeType(Value, typeof(T));
        }

        private void ValidateInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void CheckChanged(object sender, RoutedEventArgs e)
        {
            FilterToggled?.Invoke(
                null,
                new FilterToggledEventArgs(
                    FilterContext,
                    chkCheck.IsChecked.Value));
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsNumeric)
                txtValue.PreviewTextInput += ValidateInput;
        }
    }
}
