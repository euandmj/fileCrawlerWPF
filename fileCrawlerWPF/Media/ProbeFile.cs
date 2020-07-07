using Microsoft.WindowsAPICodePack.Shell;
using NReco.VideoInfo;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace fileCrawlerWPF.Media
{
    public struct CodecInfo
    {
        public string codec;
        public string codecType;
    }

    public class ProbeFile : IEquatable<ProbeFile>
    {
        protected long _size;
        protected byte[] _hash;
        protected BitmapSource _thumbnail;
        public readonly DirectoryInfo _directory;

        public string VideoCodec => videoCodec.codec;
        public string AudioCodec => audioCodec.codec;

        public readonly Guid ID;

        public CodecInfo audioCodec;
        public CodecInfo videoCodec;

        public string Name          => _directory.Name;
        public string Directory     => _directory.Parent.FullName;
        public string Path          => _directory.FullName;
        public string FileSize      => _size / 1000000 + " MB";
        public string Resolution    => $"{Width}x{Height}";
        public string HashAsHex
            => _hash != null
            ? $"#{BitConverter.ToString(_hash).Replace("-", "").ToLowerInvariant()}"
            : null;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public float FrameRate { get; private set; }
        public TimeSpan Duration { get; private set; }        

        public ProbeFile() { }

        public ProbeFile(string path, Guid id)
        {            
            ID = id;
            _directory = new DirectoryInfo(path);

            ReadFile();
        }

        public ProbeFile(string path) 
            : this(path, Guid.NewGuid()) { }
        
        private void ReadFile()
        {
            try
            {
                FFProbe probe = new FFProbe();
                MediaInfo info = probe.GetMediaInfo(Path);
                _size = new FileInfo(Path).Length;

                Duration = info.Duration;

                foreach (var stream in info.Streams)
                {
                    if (stream.CodecType == "video")
                    {
                        videoCodec.codec = stream.CodecName;
                        videoCodec.codecType = stream.CodecType;
                        Width = stream.Width;
                        Height = stream.Height;
                        FrameRate = stream.FrameRate;
                    }
                    else if (stream.CodecType == "audio")
                    {
                        audioCodec.codec = stream.CodecName;
                        audioCodec.codecType = stream.CodecType;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Unable to locate the specific file. path is supplied as " + Path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private BitmapSource BitmapToBitmapImage(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           System.Windows.Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
        }

        private void LoadThumbnail()
        {
            ShellFile imgfile = ShellFile.FromFilePath(Path);
            var bmp = imgfile.Thumbnail.ExtraLargeBitmap;
            _thumbnail = BitmapToBitmapImage(bmp);
        }

        public async Task<string> ComputeHashAsync()
        {
            if (!(_hash is null)) return HashAsHex;

            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(Path))
            {
                return await Task.Run(() =>
                {
                   _hash = md5.ComputeHash(stream);
                   return HashAsHex;
                });
            }
        }

        public BitmapSource GetThumbnail()
        {
            if (_thumbnail == null)
                LoadThumbnail();

            return _thumbnail;
        }

        public void OpenFile()
        {
            Process.Start(Path);
        }

        public void OpenFolder()
        {
            Process.Start(Directory);
        }

        public bool Equals(ProbeFile other) => Path == other.Path;

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (ProbeFile a, ProbeFile b) 
            => a?.Path == b?.Path;

        public static bool operator != (ProbeFile a, ProbeFile b) 
            => a?.Path != b?.Path;
    }
}