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
        private FileInfoModel _viewModel;
        private ProbeFile _probeFile = null;

        public FileInformation()
        {
            _viewModel = new FileInfoModel(ProbeFile);
            DataContext = _viewModel;

            InitializeComponent();

            DataContextChanged += this.FileInformation_DataContextChanged;
        }       
           

        public string Title { get; set; }

        public ProbeFile ProbeFile
        {
            get => _probeFile;
            set
            {
                _probeFile = value;
                _viewModel = new FileInfoModel(value);
                DataContext = _viewModel;
            }
        }

        private async void imgHashCalculate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_probeFile is null) return;
            var currid = _probeFile.ID;
            string hash = null;

            try
            {
                progresBar.Visibility = Visibility.Visible;
                hash = await _probeFile.ComputeHashAsync();
            }
            catch(Exception)
            {
                MessageBox.Show($"Unable to calculate hash for file {_probeFile.Name}");
            }
            finally
            {
                if (_probeFile.ID == currid)
                    txtHash.Text = hash;

                progresBar.Visibility = Visibility.Collapsed;
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            _probeFile?.OpenFile();
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            _probeFile?.OpenFolder();
        }

        private void FileInformation_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            progresBar.Visibility = Visibility.Collapsed;
        }
    }
}