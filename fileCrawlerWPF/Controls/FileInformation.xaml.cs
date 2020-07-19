using fileCrawlerWPF.Controls.model;
using fileCrawlerWPF.Media;
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
        private readonly FileInfoModel _viewModel;

        public FileInformation()
        {
            InitializeComponent();
            _viewModel = new FileInfoModel();
            DataContextChanged += this.FileInformation_DataContextChanged;
        }       
           

        public void SetFile(ProbeFile f)
        {
            //return;
            _viewModel.ProbeFile = f;
        }

        private async void imgHashCalculate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            byte[] hash = null;
            var file = _viewModel.ProbeFile;

            try
            {
                progresBar.Visibility = Visibility.Visible;
                imgHashCalculate.IsEnabled = false;

                hash = await file.ComputeHashAsync();
            }
            catch (Exception)
            {
                MessageBox.Show($"Unable to calculate hash for file {_viewModel.FileName}");
            }
            finally
            {
                //file.Hash ??= hash;
                if (_viewModel.ID == file.ID)
                    _viewModel.SetHash(hash);
                else
                    file.Hash = hash;

                progresBar.Visibility = Visibility.Collapsed;
                imgHashCalculate.IsEnabled = true;
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ProbeFile.OpenFile();
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ProbeFile.OpenFolder();
        }

        private void FileInformation_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            progresBar.Visibility = Visibility.Collapsed;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //DataContext = this;
            DataContext = _viewModel;
        }
    }
}