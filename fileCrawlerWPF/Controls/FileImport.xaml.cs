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
        public event EventHandler<PathSelectedEventArgs> PathSelected;
        public event EventHandler<FileSelectedEventArgs> RemoveFile;
        public event EventHandler<FileSelectedEventArgs> FileSelected;


        public FileImport()
        {
            InitializeComponent();
        }

        private FileDirectory? SelectedItem
            => dgFiles.SelectedItem as FileDirectory?;

        private void Files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!SelectedItem.HasValue)
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
                    PathSelected?.Invoke(this, new PathSelectedEventArgs(path));
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
                    PathSelected?.Invoke(this, new PathSelectedEventArgs(path));
                }
            }
        }

        private void btnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Clear?.Invoke(this, EventArgs.Empty);
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelectedItem is null) return;
            RemoveFile?.Invoke(this, new FileSelectedEventArgs(SelectedItem.Value.ID));
        }

        private void Cull_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
