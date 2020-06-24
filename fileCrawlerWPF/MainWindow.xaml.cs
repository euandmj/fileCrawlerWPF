using fileCrawlerWPF.Controls;
using fileCrawlerWPF.Events;
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

        

        // ItemsSource="{Binding Source=ListViewDirectories}"
        public List<ListViewItem> ListViewDirectories
        {  
            get
            {
                return media.Directories.
                    Select(x => new ListViewItem { ID = x.Key, Name = x.Name, Path = x.Path }).
                    ToList();
            }
        }

        private ListViewItem? SelectedItem
        {
            get
            {
                try { return (ListViewItem)AllFilesListBox.SelectedItem; }
                catch(InvalidCastException)
                {
                    return null;
                }                
            }
        }

        public ProbeFile SelectedFilterFile { get; set; }

        // @todo replace with control
        private void UpdateAllFilesListBox()
        {
            AllFilesListBox.Items.Clear();

            foreach (var dir in media.Directories)
            {
                AllFilesListBox.Items.Add(
                    new ListViewItem 
                    { 
                        ID = dir.Key, 
                        Path = dir.Path, 
                        Name = dir.Name 
                    });
            }
        }

        private void Filter(int w, int h)
        {
            //// first needs to complete the cache
            //media.CacheAll();

            //foreach (var file in media.CachedFiles)
            //{
            //    bool ismatch = true;
            //    if ((bool)fResChecked.IsChecked)
            //    {
            //        ismatch &= file.Width >= w && file.Height >= h;
            //    }
            //    if ((bool)fVCodecChecked.IsChecked)
            //    {
            //        // is the entered codec an alias?
            //        if (IsTargetAnAlias(fVidCodec.Text.ToLower(), out string t))
            //            ismatch &= file.videoCodec.codec == t;
            //        else
            //            ismatch &= file.videoCodec.codec == fVidCodec.Text.ToLower();
            //    }
            //    if ((bool)fACodecChecked.IsChecked)
            //    {
            //        ismatch &= file.AudioCodec == fAudCodec.Text.ToLower();
            //    }
            //    if ((bool)fDurChecked.IsChecked)
            //    {
            //        TimeSpan ts = TimeSpan.Parse(fDur.Text);
            //        ismatch &= ts == file.Duration;
            //    }
            //    if ((bool)fFramesChecked.IsChecked)
            //    {
            //        ismatch &= FrameratesFilter(framesCombox.SelectedIndex, file.FrameRate);
            //    }
            //    if ((bool)fNameChecked.IsChecked)
            //    {
            //        // ismatch &= file.Name.ToLower().Contains(fName.Text.ToLower());
            //        ismatch &= IsSearchNameMatch(fName.Text.ToLower(), file.Name.ToLower());
            //    }
            //    if (ismatch)
            //    {
            //        media.FilteredFiles.Add((file.ID, file.Name));
            //    }
            //}

            //foreach (var match in media.FilteredFiles)
            //{
            //    FilesListBox_Preview.Items.Add(match.Name);

            //}

            //filterMatches.Content = "Total Matches: " + FilesListBox_Preview.Items.Count;
        }

        private void ClearPreviewFileInformation()
        {
        }

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

            UpdateAllFilesListBox();
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
            UpdateAllFilesListBox();
        }

        private void CtlFilter_FileSelected(object sender, FileSelectedEventArgs e)
        {
            if(sender is FilterControl)
            {
                Filter_FileInfo.ProbeFile = media.GetFileFromCache(e.FileID);
            }
            //else if(sender is AllFilesListBox)
            //{
            //    All_FileInfo.ProbeFile = media.GetFileFromCache(e.FileID);
            //}
        }

        private void CtlFilter_RequestFilter(object sender, EventArgs e)
        {
            media.CacheAll();
            ctlFilter.OnFilter(media.CachedFiles, e);
        }

        private void openFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (previewPath.Text == "")
                return;
            string txt = previewPath.Text;
            txt = txt.Substring(0, txt.Length - previewName.Text.Length);


            Process.Start(txt);
            */
        }

        private void AllFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllFilesListBox.SelectedIndex == -1) return;

            var lv = SelectedItem;

            if (!lv.HasValue)
                return;

            All_FileInfo.ProbeFile = media.GetFileFromCache(lv.Value);
        }

        private void openFolderBtn_Copy_Click(object sender, RoutedEventArgs e)
        {

            //if (previewPath_Copy.Text == "")
            //    return;
            //string txt = previewPath_Copy.Text;
            //txt = txt.Substring(0, txt.Length - previewName_Copy.Text.Length);
            //Process.Start(txt);
        }

        private void openFileBtn_Copy_Click(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrEmpty(previewPath_Copy.Text))
            //    return;

            //Process.Start(previewPath_Copy.Text);
        }

        private void filterReset_Click(object sender, RoutedEventArgs e)
        {
            ClearPreviewFileInformation();
        }

        private void MenuItemServerStatus_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://github.com/euandmj/fileCrawlerWPF");
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }

}
