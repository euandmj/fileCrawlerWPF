using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

        public static readonly DependencyProperty ProbeFileDependencyProperty =
            DependencyProperty.Register("ProbeFile", typeof(ProbeFile), typeof(FileInformation), new PropertyMetadata());

        
        
        
        public FileInformation()
        {
            InitializeComponent();
        }

        ~FileInformation()
        {
        }

        private void CtlFileInformation_CalculateHash(object sender, EventArgs e)
        {
            txtHash.Text = _probeFile.Hash;
        }

        public string Title { get; set; }

        public ProbeFile ProbeFile
        {
            get => _probeFile;
            set
            {
                if(!(_probeFile is null))
                    _probeFile.HashCalculated -= this.CtlFileInformation_CalculateHash;

                _probeFile = value;
                UpdateView();
            }
        }

        private void UpdateView()
        {
            _probeFile.HashCalculated += this.CtlFileInformation_CalculateHash;

            txtFileName.Text    = _probeFile.Name;
            txtPath.Text        = _probeFile.Path;
            txtResolution.Text  = _probeFile.Resolution;
            txtFrameRate.Text   = _probeFile.FrameRate.ToString();
            txtVCodec.Text      = _probeFile.VideoCodec;
            txtACodec.Text      = _probeFile.AudioCodec;
            txtFileSize.Text    = _probeFile.FileSize;
            txtHash.Text        = _probeFile.Hash;
            imgThumbnail.Source = _probeFile.Thumbnail;
        }

        private void imgHashCalculate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_probeFile is null ||
                _probeFile.Hash is null) return;

            using (new WaitCursor())
            {
                _probeFile.ComputeHash();
                
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