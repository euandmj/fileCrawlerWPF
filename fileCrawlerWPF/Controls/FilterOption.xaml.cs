using fileCrawlerWPF.Events;
using fileCrawlerWPF.Filters;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace fileCrawlerWPF.Controls
{
    /// <summary>
    /// Interaction logic for FilterOption.xaml
    /// </summary>
    public partial class FilterOption : UserControl
    {
        public event EventHandler<FilterToggledEventArgs> FilterToggled;
       

        public FilterOption()
        {
            InitializeComponent();

            DataContext = this;

        }

        public bool IsNumeric { get; set; }
        public FilterContext FilterContext { get; set; }
        public string FilterName { get; set; }
        public string Value => txtValue.Text;

        public void Reset()
        {
            txtValue.Clear();
            chkCheck.IsChecked = false;
            CheckChanged(null, null);
        }

        public T GetValue<T>()
        {
            if(string.IsNullOrWhiteSpace(Value))
            {
                return default;
            }
            else
            {
                return (T)Convert.ChangeType(Value, typeof(T));
            }
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
