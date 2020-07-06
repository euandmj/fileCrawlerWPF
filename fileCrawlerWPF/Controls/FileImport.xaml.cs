using fileCrawlerWPF.Events;
using fileCrawlerWPF.Media;
using System;
using System.Windows.Controls;

namespace fileCrawlerWPF.Controls
{
    /// <summary>
    /// Interaction logic for FileImport.xaml
    /// </summary>
    public partial class FileImport : UserControl
    {
        private readonly string lastCheckedDirectory = null;

        public event EventHandler Clear;
        public event EventHandler<DirectorySelectedEventArgs> DirectoryScanned;
        public event EventHandler<FileSelectedEventArgs> FileSelected;


        public FileImport()
        {
            InitializeComponent();
        }

        private FileDirectory? SelectedItem
        {
            get
            {
                try
                {
                    if (dgFiles.SelectedIndex == -1) return null;
                    return (FileDirectory)dgFiles.SelectedItem;
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
        }

        private void Files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgFiles.SelectedIndex == -1 ||
                !SelectedItem.HasValue)
                return;

            FileSelected?.Invoke(this, new FileSelectedEventArgs(SelectedItem.Value));
        }

        private void btnSelectFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = lastCheckedDirectory;
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string path = dialog.SelectedPath;
                    DirectoryScanned?.Invoke(this, new DirectorySelectedEventArgs(path));
                }
            }
        }

        private void btnSelectFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.InitialDirectory = lastCheckedDirectory;
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    string path = dialog.FileName;
                    DirectoryScanned?.Invoke(this, new DirectorySelectedEventArgs(path));
                }
            }
        }

        private void btnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Clear?.Invoke(this, EventArgs.Empty);
        }
    }
}
