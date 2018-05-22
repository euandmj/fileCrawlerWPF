using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Drawing;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Navigation;
using Microsoft.WindowsAPICodePack;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace fileCrawlerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> fileDirectories;
        Dictionary<string, ProbeFile> fileDictionary;
        List<ProbeFile> files;
        

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


        public MainWindow()
        {
            InitializeComponent();
            fileDictionary = new Dictionary<string, ProbeFile>();
            fileDirectories = new List<string>();
        }

        private void ScanBttn_Click(object sender, RoutedEventArgs e)
        {
            var path = dirText.Text;
           // var path = @"I:\Movies\TV\Game of Thrones\";

            if (path == "")
                return;

            fileDirectories.Clear();
            AllFilesListBox.Items.Clear();



            if (File.Exists(path))
            {
                fileDirectories.Add(path);
            }
            else if (Directory.Exists(path))
            {
                ProcessDirectory(path);
            }

            files = new List<ProbeFile>();

            foreach(string s in fileDirectories)
            {
                files.Add(new ProbeFile(s));
            }

            CullNonVideoFiles();
            UpdateAllFilesListBox();

            totalFilesCount.Text = files.Count.ToString();
        }

        private void ProcessDirectory(string path)
        {
            // Recurvise function to retrieve all files in a supplied dirctory. 
            // From https://msdn.microsoft.com/en-us/library/07wt70x2(v=vs.110).aspx
            string[] foundfiles = Directory.GetFiles(path);

            foreach (string s in foundfiles)
                fileDirectories.Add(s);

            string[] subDirectories = Directory.GetDirectories(path);

            foreach (string s in subDirectories)
            {
                ProcessDirectory(s);
            }
        }

        private void CullNonVideoFiles()
        {
            // removes all files which do not have a video file. 
            files.RemoveAll(s => s.videoCodec.codecType != "video");
            
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
            foreach(var f in files)
            {
                AllFilesListBox.Items.Add(f.Name);
            }
        }

        private void AllFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllFilesListBox.SelectedIndex == -1) return;

            var current = AllFilesListBox.SelectedItem.ToString();

            foreach(var file in files)
            {
                if(file.Name == AllFilesListBox.SelectedItem.ToString())
                {
                    UpdateSelectedFile(file);
                }
            }
        }

        private void UpdateSelectedFile(ProbeFile file)
        {            
            previewName.Text = file.Name;
            previewPath.Text = file.Path;
            previewResol.Text = file.Width + "x" + file.Height;
            previewFPS.Text = file.FrameRate.ToString();
            previewVidCodec.Text = file.VideoCodec;
            previewAudioCodec.Text = file.AudioCodec;
            previewFileSize.Text = file.FileSize;

            Microsoft.WindowsAPICodePack.Shell.ShellFile imgfile = Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(file.Path);
            Bitmap bmp = imgfile.Thumbnail.ExtraLargeBitmap;
            thumbnail.Source = BitmapToBitmapImage(bmp);
        }

        private void UpdateFilteredPreview(ProbeFile file)
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
            var matches = new List<ProbeFile>();
            // loop through the files and if they meet the filter, add to list
            foreach(var file in files)
            {
                bool ismatch = true;
                if ((bool)fResChecked.IsChecked)
                {
                    if (int.TryParse(fWidth.Text, out int w) && int.TryParse(fHeight.Text, out int h))
                    {
                        ismatch &= file.Width >= w && file.Height >= h;
                    }
                    else
                        MessageBox.Show("Please enter a valid integer for width and height");
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
                    ismatch &= frameratesFilter(fpsCombox.SelectedIndex, file.FrameRate, fFPS.Text);
                   // ismatch &= Math.Round(file.FrameRate, 2).ToString() == fFPS.Text; 
                }
                if((bool)fNameChecked.IsChecked)
                {
                    // ismatch &= file.Name.ToLower().Contains(fName.Text.ToLower());
                    ismatch &= isSearchNameMatch(fName.Text.ToLower(), file.Name.ToLower());
                }

                if (ismatch)
                    matches.Add(file);
            }

            //FilesListBox_Preview.Document.Blocks.Clear();
            FilesListBox_Preview.Items.Clear();
            // Update the listbox. 
            foreach (var item in matches)
            {
                //FilesListBox_Preview.AppendText(item.name);
                FilesListBox_Preview.Items.Add(item.Name);
            }

            filterMatches.Content = "Total Matches: \n" + matches.Count;
        }

        // index is the selected item of the combo box. 0 - > ; 1 - < ; 2 - =
        bool frameratesFilter(int index, float file_fps, string input)
        {
            float.TryParse(input, out float filter_fps);
            
            switch (index)
            {
                case 0:
                    return file_fps > filter_fps;
                case 1:
                    return file_fps < filter_fps;
                case 2:
                    return file_fps == filter_fps;
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

        private void FilterApplyButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            FilesListBox_Preview.Items.Clear();
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
            string writeToPath = @"results.txt";
            exportTextLabel.Content = $"\"{writeToPath}\"";
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
            foreach(var file in files)
            {
                if(file.Name == FilesListBox_Preview.SelectedItem.ToString())
                {
                    UpdateFilteredPreview(file);
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
    }
    
}
