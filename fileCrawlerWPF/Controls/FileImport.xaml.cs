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

        private TreeViewItem SelectedItem
        {
            get
            {
                try
                {
                    if (tvFiles.SelectedItem == null) return null;
                    return (TreeViewItem)tvFiles.SelectedItem;
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
        }

        public void ReplaceTree(TreeViewItem newItem)
        {
            this.tvFiles.Items.Clear();
            this.tvFiles.Items.Add(newItem);
        }

        private void Files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (SelectedItem is null)
            //    return;

            //FileSelected?.Invoke(this, new FileSelectedEventArgs((Guid)SelectedItem.Tag));
        }
        private void TreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            if (SelectedItem is null) return;

            var x = (Guid)SelectedItem.Tag;

            FileSelected?.Invoke(this, new FileSelectedEventArgs(x));
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
