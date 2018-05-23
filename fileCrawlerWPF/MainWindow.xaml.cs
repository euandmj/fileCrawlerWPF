using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace fileCrawlerWPF
{
    struct ListViewItem
    {
        public int Index { get; set; }
        public string Name { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> fileDirectories;
        Dictionary<int, ProbeFile> fileDictionary;
        List<ProbeFile> filterFiles;
        

        // Alias for codec types. to be more flexible on the user
        readonly ReadOnlyCollection<string> x264Aliases = 
            new ReadOnlyCollection<string>(new[]
            {
                "x264",
                "h264",
                "h.264",
                "h.264/mpeg-4",
                "h.264/mpeg-4 avc",
                "h.264/mp4",
                "h.264/mp4 avc"
            });

        readonly ReadOnlyCollection<string> x265Aliases =
            new ReadOnlyCollection<string>(new[]
            {
                "h265",
                "hevc",
                "hevc/h.265",
                "x265"
            });

        // Illegal path characters
        readonly char[] illegal_chars = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
        const char path_seperator_token = '>';

        public MainWindow()
        {
            InitializeComponent();
            fileDictionary = new Dictionary<int, ProbeFile>();
            fileDirectories = new List<string>();
            
            
          

 
        }

        private void ScanBttn_Click(object sender, RoutedEventArgs e)
        {
            
            var path = dirText.Text.Trim();
            // var path = @"I:\Movies\TV\Rick and Morty";

            if (path == "")
                return;

            // splits up mutliple directories
            string[] dirinpt = path.Split(path_seperator_token);

            fileDirectories.Clear();
            fileDictionary.Clear();
            AllFilesListBox.Items.Clear();
            previewHash.Clear(); 
            previewHash.IsEnabled = false;
            ClearSelectedFileInformation();

            using (new WaitCursor())
            {
                foreach(string p in dirinpt)
                {
                    if (File.Exists(p))
                    {
                        fileDirectories.Add(p);
                    }
                    else if (Directory.Exists(p))
                    {
                        ProcessDirectory(p);
                    }
                }               

                //files = new List<ProbeFile>();

                foreach (string s in fileDirectories)
                {
                    // files.Add(new ProbeFile(s, fileDictionary.Count + 1));
                    ProbeFile pf = new ProbeFile(s, fileDictionary.Count);

                    if (pf.videoCodec.codecType == "video") fileDictionary.Add(pf.Index, pf);
                }
            }

            CullNonVideoFiles();
            UpdateAllFilesListBox();

            totalFilesCount.Text = fileDictionary.Count.ToString();
        }

        private void ProcessDirectory(string path)
        {
            // Recurvise function to retrieve all files in a supplied dirctory. 
            // From https://msdn.microsoft.com/en-us/library/07wt70x2(v=vs.110).aspx
            string[] foundfiles = null;

            try
            {
                foundfiles = Directory.GetFiles(path);

                foreach (string s in foundfiles)
                    fileDirectories.Add(s);

                string[] subDirectories = Directory.GetDirectories(path);

                foreach (string s in subDirectories)
                {
                    ProcessDirectory(s);
                }
            }
            catch(UnauthorizedAccessException e)
            {
                MessageBox.Show(e.Message);
            }           
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void CullNonVideoFiles()
        {
            // removes all files which do not have a video file. 
            //files.RemoveAll(s => s.videoCodec.codecType != "video");

            // work out which keys belong to non video files
            var keys_to_remove = fileDictionary.Where(entry => entry.Value.videoCodec.codecType != "video")
                                                               .Select(entry => entry.Key)
                                                               .ToArray(); 

            // remove the non-video entries from the dictionary. 
            foreach(var k in keys_to_remove)
            {
                fileDictionary.Remove(k); 
            }
            
            // "update" the keys 
        }

        private void RemoveAll()
        {
            foreach (var entry in fileDictionary)
            {
                if (entry.Value.videoCodec.codecType != "video")
                {
                    fileDictionary.Remove(entry.Key);
                    RemoveAll();
                }
            }
        }

        private void UpdateAllFilesListBox()
        {
            foreach(var dict in fileDictionary)
            {
                AllFilesListBox.Items.Add(new ListViewItem { Index = dict.Key, Name = dict.Value.Name });
            }
        }

        private void AllFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Cannot find a subtly working method for getting cell data from a data grid or listview. 
            // SelectedItems[0].SubItems does not exist? 
            // This method only works without user sorting which is a minus. 

            if (AllFilesListBox.SelectedIndex == -1) return;
            

            // Clear hash info. 
            previewHash.Clear();
            previewHash.IsEnabled = false;

            int index = AllFilesListBox.SelectedIndex;

            previewName.Text = fileDictionary[index].Name;
            previewPath.Text = fileDictionary[index].Path;
            previewResol.Text = fileDictionary[index].Width + "x" + fileDictionary[index].Height;
            previewFPS.Text = fileDictionary[index].FrameRate.ToString();
            previewVidCodec.Text = fileDictionary[index].VideoCodec;
            previewAudioCodec.Text = fileDictionary[index].AudioCodec;
            previewFileSize.Text = fileDictionary[index].FileSize;

            Microsoft.WindowsAPICodePack.Shell.ShellFile imgfile = Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(fileDictionary[index].Path);
            Bitmap bmp = imgfile.Thumbnail.ExtraLargeBitmap;
            thumbnail.Source = BitmapToBitmapImage(bmp);
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

        private BitmapSource BitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapSource i = Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
            return i;
        }


        private void FilterApplyButton_Click(object sender, RoutedEventArgs e)
        {
            filterFiles = new List<ProbeFile>();

            int w = 0, h = 0;
            if (!int.TryParse(fWidth.Text, out w)) {
                MessageBox.Show("Please enter a valid numerical value for Width");
                return;
            }
            if(!int.TryParse(fHeight.Text, out h))
            {
                MessageBox.Show("Please enter a valid numerical value for Height");
                return;
            }

            // loop through the files and if they meet the filter, add to list
            foreach (var file in fileDictionary.Values)
            {
                bool ismatch = true;
                if ((bool)fResChecked.IsChecked)
                {
                    ismatch &= file.Width >= w && file.Height >= h;
                }
                if ((bool)fVCodecChecked.IsChecked)
                {
                    // is the entered codec an alias?
                    if (isTargetAnAlias(fVidCodec.Text.ToLower(), out string t))
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
                    ismatch &= ts == file.duration;
                }
                if ((bool)fFramesChecked.IsChecked)
                {
                    ismatch &= frameratesFilter(framesCombox.SelectedIndex, file.FrameRate);
                }
                if((bool)fNameChecked.IsChecked)
                {
                    // ismatch &= file.Name.ToLower().Contains(fName.Text.ToLower());
                    ismatch &= isSearchNameMatch(fName.Text.ToLower(), file.Name.ToLower());
                }

                if (ismatch)
                    filterFiles.Add(file);
            }

            //FilesListBox_Preview.Document.Blocks.Clear();
            FilesListBox_Preview.Items.Clear();
            // Update the listbox. 
            foreach (var item in filterFiles)
            {
                //FilesListBox_Preview.AppendText(item.name);
                FilesListBox_Preview.Items.Add(item.Path);
            }

            filterMatches.Content = "Total Matches: " + filterFiles.Count;
        }
        
        bool frameratesFilter(int index, float file_fps)
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

        bool isSearchNameMatch(string source, string target)
        {
            string[] split = source.Split(' ');
            
            foreach(string s in split)
            {
                if (target.Contains(s))
                    return true;
            }
            return false;
        }

        bool isTargetAnAlias(string val, out string normalised)
        {
            normalised = "";
            if (x265Aliases.Contains(val))
            {
                normalised = "hevc";
                return true;
            }
            else if (x264Aliases.Contains(val))
            {
                normalised = "h264";
                return true;
            }
            else
                return false;
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

        private void ScanBttn_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (filterFiles.Count < 1) return;

            string writeToPath = @"results.txt";
            exportTextLabel.Content = $"Results exported to \"{writeToPath}\"";
            exportTextLabel.Visibility = Visibility.Visible;

            using (StreamWriter sw = File.CreateText(writeToPath))
            {
               foreach(var i in FilesListBox_Preview.Items)
                {
                    sw.WriteLine(i.ToString());
                }
            }
        }

        private void FilesListBox_Preview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilesListBox_Preview.SelectedIndex == -1)
                return;

            foreach(var file in filterFiles)
            {
                if(file.Path == FilesListBox_Preview.SelectedItem.ToString())
                {
                    previewName_Copy.Text = file.Name;
                    previewPath_Copy.Text = file.Path;
                    previewResol_Copy.Text = file.Width + "x" + file.Height;
                    previewFPS_Copy.Text = file.FrameRate.ToString();
                    previewVidCodec_Copy.Text = file.VideoCodec;
                    previewAudioCodec_Copy.Text = file.AudioCodec;
                    previewFileSize_Copy.Text = file.FileSize;

                    Microsoft.WindowsAPICodePack.Shell.ShellFile imgfile = Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(file.Path);
                    Bitmap bmp = imgfile.Thumbnail.ExtraLargeBitmap;
                    thumbnail1.Source = BitmapToBitmapImage(bmp);

                    return;
                }
            }
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
            if (previewPath_Copy.Text == "")
                return;

            Process.Start(previewPath_Copy.Text);
        }

        private void ComputeHashBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllFilesListBox.SelectedIndex == -1) return;

            var currentFile = fileDictionary[AllFilesListBox.SelectedIndex];
            
            using(new WaitCursor())
            {
                if (currentFile.Hash == null)
                    currentFile.ComputeHash();
            }


            previewHash.IsEnabled = true;
            previewHash.Text = "#" + currentFile.HashAsHex;
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

        }
    }
    
}
