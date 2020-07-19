using fileCrawlerWPF.Controls;
using fileCrawlerWPF.Controls.model;
using fileCrawlerWPF.Events;
using fileCrawlerWPF.Exceptions;
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
        public MainWindow()
        {
            InitializeComponent();

            ctlFileImport.FileSelected  += this.CtlFileImport_FileSelected;
            ctlFileImport.PathSelected  += this.CtlFileImport_PathSelected;
            ctlFileImport.Clear         += this.CtlFileImport_Clear;
            ctlFileImport.RemoveFile    += this.CtlFileImport_RemoveFile;

            ctlFilter.FileSelected      += this.CtlFilter_FileSelected;
            ctlFilter.Clear             += this.CtlFilter_Clear;
        }


        public FileInfoModel All_FileInfo_Model
        {
            get { return (FileInfoModel)GetValue(All_FileInfo_Property); }
            set { SetValue(All_FileInfo_Property, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty All_FileInfo_Property =
            DependencyProperty.Register(nameof(All_FileInfo_Model), 
                typeof(FileInfoModel), 
                typeof(MainWindow), 
                new PropertyMetadata(null));




        #region Child Control Events

        private void CtlFileImport_PathSelected(object sender, PathSelectedEventArgs e)
        {
            try
            {
                MediaManager.MediaCollectionInstance.ProcessDirectory(e.Path);
            }
            catch (DirectoryAlreadyExistsException ex)
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
            MediaManager.MediaCollectionInstance.Reset();
            All_FileInfo.SetFile(null);
        }

        private void CtlFileImport_FileSelected(object sender, FileSelectedEventArgs e)
        {
            var f = MediaManager.MediaCollectionInstance.GetFile(e.Directory);
            if (f is null) throw new ArgumentNullException(nameof(e));
            
            All_FileInfo.SetFile(f);
            All_FileInfo_Model = new FileInfoModel(f);

        }

        private void CtlFileImport_RemoveFile(object sender, FileSelectedEventArgs e)
        {
            MediaManager.MediaCollectionInstance.RemoveFile(e.ID);
            All_FileInfo.SetFile(null);
        }

        private void CtlFilter_FileSelected(object sender, FileSelectedEventArgs e)
        {
            Filter_FileInfo.SetFile(MediaManager.MediaCollectionInstance.GetFile(e.ID));
        }

        private void CtlFilter_Clear(object sender, EventArgs e)
        {
            Filter_FileInfo.SetFile(null);
        }
        #endregion

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
            All_FileInfo.DataContext = All_FileInfo_Property;
            ctlFileImport.dgFiles.DataContext = MediaManager.MediaCollectionInstance.Directories;
        }


    }

}
