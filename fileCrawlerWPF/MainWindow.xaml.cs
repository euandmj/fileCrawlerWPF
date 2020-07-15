using fileCrawlerWPF.Events;
using fileCrawlerWPF.Media;
using System;
using System.Diagnostics;
using System.Windows;

namespace fileCrawlerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly MediaCollection _mediaCollection;

        public MainWindow()
        {
            InitializeComponent();

            _mediaCollection = new MediaCollection();

            ctlFileImport.FileSelected      += this.CtlFileImport_FileSelected;
            ctlFileImport.PathSelected      += this.CtlFileImport_DirectoryScanned;
            ctlFileImport.Clear             += this.CtlFileImport_Clear;
            ctlFileImport.RemoveFile        += this.CtlFileImport_RemoveFile;
            ctlFilter.RequestFilter         += this.CtlFilter_RequestFilter;
            ctlFilter.FileSelected          += this.CtlFilter_FileSelected;
            ctlFilter.Clear                 += this.CtlFilter_Clear;
        }

       

        #region Events

        private void CtlFileImport_DirectoryScanned(object sender, PathSelectedEventArgs e)
        {
            try
            {
                _mediaCollection.ProcessDirectory(e.Path);
            }
            catch(DirectoryAlreadyExistsException ex)
            {
                MessageBox.Show($"The following directory already exists\n{ex.Directory}", 
                    "Error scanning folder", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, 
                    "Error scanning folder", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CtlFileImport_Clear(object sender, EventArgs e)
        {
            _mediaCollection.Reset();
            All_FileInfo.SetFile(null);
        }

        private void CtlFileImport_FileSelected(object sender, FileSelectedEventArgs e)
        {
            var f = _mediaCollection.GetFileFromCache(e.Directory);
            if (f is null) throw new ArgumentNullException(nameof(e));
            All_FileInfo.SetFile(f);
        }
        private void CtlFileImport_RemoveFile(object sender, FileSelectedEventArgs e)
        {
            _mediaCollection.RemoveFile(e.ID);
            All_FileInfo.SetFile(null);
        }

        private void CtlFilter_FileSelected(object sender, FileSelectedEventArgs e)
        {
            Filter_FileInfo.SetFile(_mediaCollection.GetFileFromCache(e.ID));
        }       

        private void CtlFilter_Clear(object sender, EventArgs e)
        {
            Filter_FileInfo.SetFile(null);
        }

        private void CtlFilter_RequestFilter(object sender, EventArgs e)
        {
            try
            {
                _mediaCollection.CacheAll();
                ctlFilter.OnFilter(_mediaCollection.CachedFiles, e);
            }
            catch(OverflowException)
            {
                ctlFilter.ResetView();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error scanning folder",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItemServerStatus_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://github.com/euandmj/fileCrawlerWPF");
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ctlFileImport.dgFiles.DataContext = _mediaCollection.Directories;
        }
        #endregion


    }

}
