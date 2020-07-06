using fileCrawlerWPF.Controls;
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

        private readonly MediaCollection media;

        public MainWindow()
        {
            InitializeComponent();

            media = new MediaCollection();

            ctlFileImport.FileSelected      += this.CtlFileImport_FileSelected;
            ctlFileImport.DirectoryScanned  += this.CtlFileImport_DirectoryScanned;
            ctlFileImport.Clear             += this.CtlFileImport_Clear;
            ctlFilter.RequestFilter         += this.CtlFilter_RequestFilter;
            ctlFilter.FileSelected          += this.CtlFilter_FileSelected;
            ctlFilter.Clear                 += this.CtlFilter_Clear;
        }

        

        public ProbeFile SelectedFilterFile { get; set; }


        #region Events

        private void CtlFileImport_FileSelected(object sender, FileSelectedEventArgs e)
        {
            var f = media.GetFileFromCache(e.Directory);

            if (f is null) throw new ArgumentNullException(nameof(e));

            All_FileInfo.ProbeFile = f;
        }

        private void CtlFileImport_Clear(object sender, EventArgs e)
        {
            media.Reset();
            All_FileInfo.ProbeFile = null;
        }

        private void CtlFilter_Clear(object sender, EventArgs e)
        {
            Filter_FileInfo.ProbeFile = null;
        }

        private void CtlFileImport_DirectoryScanned(object sender, DirectorySelectedEventArgs e)
        {
            try
            {
                media.ProcessDirectory(e.Path);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error scanning folder", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CtlFilter_FileSelected(object sender, FileSelectedEventArgs e)
        {
            if(sender is FilterControl)
            {
                Filter_FileInfo.ProbeFile = media.GetFileFromCache(e.ID);
            }
        }

        private void CtlFilter_RequestFilter(object sender, EventArgs e)
        {
            media.CacheAll();
            ctlFilter.OnFilter(media.CachedFiles, e);
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
            ctlFileImport.dgFiles.DataContext = media.Directories;
        }
        #endregion


    }

}
