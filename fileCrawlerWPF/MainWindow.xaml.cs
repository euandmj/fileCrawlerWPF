using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace fileCrawlerWPF
{
    struct ListViewItem
    {
        public Guid ID { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string lastCheckedDirectory = string.Empty;
        List<(Guid Key, string Path, string Name)> fileDirectories;
        List<(Guid Key, string Name)> filteredFiles;
        Dictionary<Guid, ProbeFile> fileCache;


        // Illegal path characters
        readonly char[] illegal_chars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
        const char path_seperator_token = '>';

        public MainWindow()
        {
            InitializeComponent();
            fileCache = new Dictionary<Guid, ProbeFile>();
            filteredFiles = new List<(Guid Key, string Name)>();
            fileDirectories = new List<(Guid, string, string)>();
        }        

        public ProbeFile SelectedFile { get; set; }
        public ProbeFile SelectedFilterFile { get; set; }
        public int TotalFilesCount { get { return fileDirectories.Count(); } }

        private void ProcessDirectory(string path)
        {
            if (fileDirectories.Any(x => x.Path == path))
                return;

            try
            {
                using (new WaitCursor())
                {
                    if (File.Exists(path))
                    {
                        if (!fileDirectories.Any(x => x.Path == path))
                            fileDirectories.Add((Guid.NewGuid(), path, Path.GetFileName(path)));
                    }                        
                    else if (Directory.Exists(path))
                    {
                        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                        files.ToList().ForEach(x =>
                        {
                            if (!fileDirectories.Any(y => y.Path == x))
                                fileDirectories.Add((Guid.NewGuid(), x, Path.GetFileName(x)));
                        });
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show(e.Message);
            }

            lastCheckedDirectory = path;
            totalFilesCount.Text = fileDirectories.Count.ToString();
            UpdateAllFilesListBox();
        }

        private void CullNonVideoFiles()
        {
            // work out which keys belong to non video files
            var keys_to_remove = fileCache.Where(entry => entry.Value.videoCodec.codecType != "video")
                                                               .Select(entry => entry.Key);

            // remove the non-video entries from the dictionary. 
            foreach (var k in keys_to_remove)
            {
                fileCache.Remove(k);
                fileDirectories.RemoveAt(fileDirectories.FindIndex(x => x.Key == k));
            }
        }

        private void UpdateAllFilesListBox()
        {
            AllFilesListBox.Items.Clear();

            foreach (var dir in fileDirectories)
            {
                AllFilesListBox.Items.Add(new ListViewItem { ID = dir.Key, Path = dir.Path, Name = dir.Name });
            }
        }

        private void CacheAll()
        {
            foreach (ListViewItem item in AllFilesListBox.Items)
            {
                if (!fileCache.ContainsKey(item.ID))
                    fileCache.Add(item.ID, new ProbeFile(item.Path, item.ID));
            }
        }

        private void Filter(int w, int h)
        {
            // first needs to complete the cache
            CacheAll();

            foreach (var file in fileCache.Values)
            {
                bool ismatch = true;
                if ((bool)fResChecked.IsChecked)
                {
                    ismatch &= file.Width >= w && file.Height >= h;
                }
                if ((bool)fVCodecChecked.IsChecked)
                {
                    // is the entered codec an alias?
                    if (IsTargetAnAlias(fVidCodec.Text.ToLower(), out string t))
                        ismatch &= file.videoCodec.codec == t;
                    else
                        ismatch &= file.videoCodec.codec == fVidCodec.Text.ToLower();
                }
                if ((bool)fACodecChecked.IsChecked)
                {
                    ismatch &= file.AudioCodec == fAudCodec.Text.ToLower();
                }
                if ((bool)fDurChecked.IsChecked)
                {
                    TimeSpan ts = TimeSpan.Parse(fDur.Text);
                    ismatch &= ts == file.Duration;
                }
                if ((bool)fFramesChecked.IsChecked)
                {
                    ismatch &= FrameratesFilter(framesCombox.SelectedIndex, file.FrameRate);
                }
                if ((bool)fNameChecked.IsChecked)
                {
                    // ismatch &= file.Name.ToLower().Contains(fName.Text.ToLower());
                    ismatch &= IsSearchNameMatch(fName.Text.ToLower(), file.Name.ToLower());
                }
                if (ismatch)
                {
                    filteredFiles.Add((file.ID, file.Name));
                }
            }

            foreach (var match in filteredFiles)
            {
                FilesListBox_Preview.Items.Add(match.Name);

            }

            filterMatches.Content = "Total Matches: " + FilesListBox_Preview.Items.Count;
        }

        private void ClearSelectedFileInformation()
        {
            previewName.Clear();
            previewPath.Clear();
            previewResol.Clear();
            previewFPS.Clear();
            previewVidCodec.Clear();
            previewAudioCodec.Clear();
            previewFileSize.Clear();
            // Clear hash info. 
            previewHash.Clear();
            previewHash.IsEnabled = false;
            thumbnail.Source = null;
        }

        private void ClearPreviewFileInformation()
        {
            FilesListBox_Preview.Items.Clear();
            // text fields
            fWidth.Text = "1920";
            fHeight.Text = "1080";
            fVidCodec.Clear();
            fAudCodec.Clear();
            fName.Clear();

            // check boxes
            fResChecked.IsChecked = false;
            fVCodecChecked.IsChecked = false;
            fACodecChecked.IsChecked = false;
            fFramesChecked.IsChecked = false;
            fNameChecked.IsChecked = false;

            // file info fields
            previewName_Copy.Clear();
            previewPath_Copy.Clear();
            previewResol_Copy.Clear();
            previewFPS_Copy.Clear();
            previewVidCodec_Copy.Clear();
            previewAudioCodec_Copy.Clear();
            previewFileSize_Copy.Clear();
            thumbnail1.Source = null;
        }

        private bool FrameratesFilter(int index, float file_fps)
        {
            // index0  = 0 - 30
            // index1  = 30-60
            // index2  = 60+
            file_fps = (float)Math.Round(file_fps, 0);
            switch (index)
            {
                case 0:
                    return file_fps <= 30;
                case 1:
                    return file_fps > 30 && file_fps <= 60;
                case 2:
                    return file_fps > 60;
                default:
                    return false;
            }
        }

        private bool IsSearchNameMatch(string source, string target)
        {
            var split = source.Split(' ');

            foreach (string s in split)
            {
                if (target.Contains(s))
                    return true;
            }
            return false;
        }

        private bool IsTargetAnAlias(string val, out string normalised)
        {
            normalised = string.Empty;
            if (FileAliases.x265Aliases.Contains(val))
            {
                normalised = "hevc";
                return true;
            }
            else if (FileAliases.x264Aliases.Contains(val))
            {
                normalised = "h264";
                return true;
            }
            else
                return false;
        }

        #region Events
        private void ScanFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            string path = null;

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = lastCheckedDirectory;
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    path = dialog.SelectedPath;
                }
            }

            ProcessDirectory(path);
        }

        private void ScanFileBtn_Click(object sender, RoutedEventArgs e)
        {
            string path = null;

            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.InitialDirectory = lastCheckedDirectory;
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    path = dialog.FileName;
                }
            }

            ProcessDirectory(path);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (previewPath.Text == "")
                return;

            Process.Start(previewPath.Text);
        }

        private void openFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (previewPath.Text == "")
                return;
            string txt = previewPath.Text;
            txt = txt.Substring(0, txt.Length - previewName.Text.Length);


            Process.Start(txt);
        }

        private void AllFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Cannot find a subtly working method for getting cell data from a data grid or listview. 
            // SelectedItems[0].SubItems does not exist? 
            // This method only works without user sorting which is a minus. 

            if (AllFilesListBox.SelectedIndex == -1) return;

            var lv = (ListViewItem)AllFilesListBox.SelectedItem;

            if (!fileCache.ContainsKey(lv.ID))
            {
                var pf = new ProbeFile(lv.Path, lv.ID);
                fileCache.Add(pf.ID, pf);
            }

            if (fileCache.TryGetValue(lv.ID, out ProbeFile file))
                SelectedFile = file;


            previewHash.Clear();
            previewHash.IsEnabled = false;

            var item = SelectedFile;

            previewName.Text = item.Name;
            previewPath.Text = item.Path;
            previewResol.Text = item.Resolution;
            previewFPS.Text = item.FrameRate.ToString();
            previewVidCodec.Text = item.VideoCodec;
            previewAudioCodec.Text = item.AudioCodec;
            previewFileSize.Text = item.FileSize;
            thumbnail.Source = item.Thumbnail;
        }

        private void ScanBttn_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (FilesListBox_Preview.Items.Count < 1) return;

            string writeToPath = @"results.txt";
            exportTextLabel.Content = $"Results exported to \"{writeToPath}\"";
            exportTextLabel.Visibility = Visibility.Visible;

            using (StreamWriter sw = File.CreateText(writeToPath))
            {
                foreach (var i in FilesListBox_Preview.Items)
                {
                    sw.WriteLine(i.ToString());
                }
            }
        }

        private void FilterApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(fWidth.Text, out int w))
            {
                MessageBox.Show("Please enter a valid numerical value for Width");
                return;
            }
            if (!int.TryParse(fHeight.Text, out int h))
            {
                MessageBox.Show("Please enter a valid numerical value for Height");
                return;
            }

            FilesListBox_Preview.Items.Clear();
            filteredFiles.Clear();

            using (new WaitCursor())
            {
                Filter(w, h);
            }            
        }

        private void FilesListBox_Preview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilesListBox_Preview.SelectedIndex == -1)
                return;

            int index = FilesListBox_Preview.SelectedIndex;

            var selectedFilterFile = filteredFiles.ElementAt(index);
            fileCache.TryGetValue(selectedFilterFile.Key, out ProbeFile file);

            if(file is null)
            {
                MessageBox.Show($"Unable to find {selectedFilterFile.Key} in {nameof(fileCache)}");
                return;
            }

            previewName_Copy.Text = file.Name;
            previewPath_Copy.Text = file.Path;
            previewResol_Copy.Text = file.Resolution;
            previewFPS_Copy.Text = file.FrameRate.ToString();
            previewVidCodec_Copy.Text = file.VideoCodec;
            previewAudioCodec_Copy.Text = file.AudioCodec;
            previewFileSize_Copy.Text = file.FileSize;
            thumbnail1.Source = file.Thumbnail;
        }

        private void openFolderBtn_Copy_Click(object sender, RoutedEventArgs e)
        {

            if (previewPath_Copy.Text == "")
                return;
            string txt = previewPath_Copy.Text;
            txt = txt.Substring(0, txt.Length - previewName_Copy.Text.Length);
            Process.Start(txt);
        }

        private void openFileBtn_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(previewPath_Copy.Text))
                return;

            Process.Start(previewPath_Copy.Text);
        }

        private void ComputeHashBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFile is null) return;

            using (new WaitCursor())
            {
                if (SelectedFile.Hash is null)
                    SelectedFile.ComputeHash();
            }
            previewHash.IsEnabled = true;
            previewHash.Text = "#" + SelectedFile.HashAsHex;
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

        private void MenuItemRefresh_Click(object sender, RoutedEventArgs e)
        {
            ClearPreviewFileInformation();
        }

        private void MenuItemClearTopResults_Click(object sender, RoutedEventArgs e)
        {
            fileCache.Clear();
            fileDirectories.Clear();
            ClearSelectedFileInformation();
            UpdateAllFilesListBox();
        }

        #endregion
    }

}
