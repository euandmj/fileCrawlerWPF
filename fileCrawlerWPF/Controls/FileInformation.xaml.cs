using fileCrawlerWPF.Controls.model;
using fileCrawlerWPF.Media;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
//xmlns:fileinfo="clr-namespace:fileCrawlerWPF.Controls.model;assembly=FileInfoModel"

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
            //viewModel = new FileInfoModel()
            //_probeFile = pf;
            _viewModel = new FileInfoModel(ProbeFile);
            DataContext = _viewModel;

            InitializeComponent();
            DataContextChanged += this.FileInformation_DataContextChanged;
            //_modelView = new FileInfoModel(ProbeFile);
        }



        private void FileInformation_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        ~FileInformation()
        {
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

            progresBar.Visibility = Visibility.Visible ;
            var hash = await _probeFile.ComputeHashAsync();
            txtHash.Text = hash;
            progresBar.Visibility = Visibility.Collapsed;
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            _probeFile?.OpenFile();
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            _probeFile?.OpenFolder();
        }
    }
}