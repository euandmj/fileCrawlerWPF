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
        private ProbeFile _probeFile;

        public FileInformation()
        {
            InitializeComponent();

            DataContext = this;
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
                UpdateView();
            }
        }

        private void UpdateView()
        {
            txtFileName.Text    = _probeFile.Name;
            txtPath.Text        = _probeFile.Path;
            txtResolution.Text  = _probeFile.Resolution;
            txtFrameRate.Text   = _probeFile.FrameRate.ToString();
            txtVCodec.Text      = _probeFile.VideoCodec;
            txtACodec.Text      = _probeFile.AudioCodec;
            txtFileSize.Text    = _probeFile.FileSize;
            txtHash.Text        = _probeFile.HashAsHex;
            imgThumbnail.Source = _probeFile.Thumbnail;
        }

        private void imgHashCalculate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // @TODO make async
            if (_probeFile is null) return;

            using (new WaitCursor())
            {
                _probeFile.ComputeHash();
                txtHash.Text = _probeFile.HashAsHex;
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
    }
}