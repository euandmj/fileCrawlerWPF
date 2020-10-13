using fileCrawlerWPF.Controls.model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace fileCrawlerWPF.Controls
{
    /// <summary>
    /// Interaction logic for ctlFileInformation.xaml
    /// </summary>
    public partial class FileInformation : UserControl
    {
        public FileInformation()
        {
            InitializeComponent();
            Model = new FileInformation_ViewModel();
            DataContextChanged += this.FileInformation_DataContextChanged;
        }

        public readonly FileInformation_ViewModel Model;

        //public FileInformation_ViewModel ViewModel
        //{
        //    get { return (FileInformation_ViewModel)GetValue(ViewModelProperty); }
        //    set { SetValue(ViewModelProperty, value); }
        //}

        //public static readonly DependencyProperty ViewModelProperty =
        //    DependencyProperty.Register(
        //        nameof(ViewModel),
        //        typeof(FileInformation_ViewModel),
        //        typeof(FileInformation),
        //        new PropertyMetadata(null));

        private async void imgHashCalculate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                progresBar.Visibility = Visibility.Visible;
                imgHashCalculate.IsEnabled = false;

                await Model.CalculateHash();
            }
            catch (Exception)
            {
                MessageBox.Show($"Unable to calculate hash for file {Model.FileName}");
            }
            finally
            {
                progresBar.Visibility = Visibility.Collapsed;
                imgHashCalculate.IsEnabled = true;
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Model.ProbeFile.OpenFile();
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Model.ProbeFile.OpenFolder();
        }

        private void FileInformation_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            progresBar.Visibility = Visibility.Collapsed;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Model;
        }
    }
}