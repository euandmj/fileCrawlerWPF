using fileCrawlerWPF.Controls;
using fileCrawlerWPF.Events;
using fileCrawlerWPF.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace fileCrawlerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string lastCheckedDirectory = string.Empty;


        //private static readonly Lazy<MediaCollection> _media = 
        //    new Lazy<MediaCollection>(() => new MediaCollection());
        //private static MediaCollection MediaInstance { get => _media.Value; }

        private readonly MediaCollection media;

        // Illegal path characters
        readonly char[] illegal_chars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
        const char path_seperator_token = '>';

        public MainWindow()
        {
            InitializeComponent();

            media = new MediaCollection();


            ctlFilter.RequestFilter += this.CtlFilter_RequestFilter;
            ctlFilter.FileSelected += this.CtlFilter_FileSelected;
        }

        private ProbeFile SelectedItem
        {
            get
            {
                try 
                {
                    if (AllFilesListBox.SelectedIndex == -1) return null;
                    var dir = (FileDirectory)AllFilesListBox.SelectedItem;
                    return media.GetFileFromCache(dir);                 
                }
                catch(InvalidCastException)
                {
                    return null;
                }                
            }
        }

        public ProbeFile SelectedFilterFile { get; set; }


        #region Events
        private void ScanFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = lastCheckedDirectory;
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string path = dialog.SelectedPath;
                    media.ProcessDirectory(path);
                }
            }
        }

        private void ScanFileBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.InitialDirectory = lastCheckedDirectory;
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    string path = dialog.FileName;
                    media.ProcessDirectory(path);
                }
            }
        }

        private void CtlFilter_FileSelected(object sender, FileSelectedEventArgs e)
        {
            if(sender is FilterControl)
            {
                Filter_FileInfo.ProbeFile = media.GetFileFromCache(e.FileID);
            }
        }

        private void CtlFilter_RequestFilter(object sender, EventArgs e)
        {
            media.CacheAll();
            ctlFilter.OnFilter(media.CachedFiles, e);
        }

        private void AllFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllFilesListBox.SelectedIndex == -1) return;

            var lv = SelectedItem;


            All_FileInfo.ProbeFile = lv;
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
            All_FileInfo.DataContext =
                SelectedItem;
            AllFilesListBox.DataContext = media.Directories;
        }
        #endregion


    }

}
