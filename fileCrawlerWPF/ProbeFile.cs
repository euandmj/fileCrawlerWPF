using NReco.VideoInfo;
using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace fileCrawlerWPF
{
    public struct CodecInfo
    {
        public string codec;
        public string codecType;
    }

    public class ProbeFile : IEquatable<ProbeFile>
    {
        protected long size;
        protected byte[] hash;
        protected BitmapSource thumbnail;

        public string VideoCodec => videoCodec.codec;
        public string AudioCodec => audioCodec.codec;

        public readonly Guid ID;

        public CodecInfo audioCodec;
        public CodecInfo videoCodec;

        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float FrameRate { get; set; }
        public string Path { get; set; }
        public TimeSpan Duration { get; private set; }
        public string FileSize => size / 1000000 + " MB";
        public string HashAsHex => hash != null ? $"#{BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant()}" : null;
        public string Resolution => $"{Width}x{Height}";
        public string Directory => Path.Substring(0, Path.Length - Name.Length);

        public BitmapSource Thumbnail
        {
            get
            {
                if (thumbnail == null)
                    LoadThumbnail();

                return thumbnail;
            }
            set { thumbnail = value; }
        }

        public ProbeFile() { }

        public ProbeFile(string path, Guid id)
        {
            Path = path ?? throw new ArgumentNullException($"{nameof(path)}");
            ID = id;

            Name = Path.Substring(Path.LastIndexOf('\\') + 1);

            ReadFile();
        }

        public ProbeFile(string path) : this(path, Guid.NewGuid()) { }
        
        private void ReadFile()
        {
            try
            {
                FFProbe probe = new FFProbe();
                MediaInfo info = probe.GetMediaInfo(Path);
                size = new FileInfo(Path).Length;

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
            Thumbnail = BitmapToBitmapImage(bmp);
        }

        public async Task<string> ComputeHashAsync()
        {
            if (!(hash is null)) return HashAsHex;

            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(Path))
            {
                return await Task.Run(() =>
                {
                   hash = md5.ComputeHash(stream);
                   return HashAsHex;
                });
            }
        }

        public void OpenFile()
        {
            Process.Start(Path);
        }

        public void OpenFolder()
        {
            Process.Start(Directory);
        }

        public void PrintInfo()
        {
            Console.WriteLine("\n");
            Console.WriteLine($"Info for {Name}" +
                $"\nDuration: {Duration}" +
                $"\nWxH: {Width}x{Height}" +
                $"\nFramrate: {FrameRate}");
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

        public static bool operator == (ProbeFile a, ProbeFile b) => a.Path == b.Path;

        public static bool operator != (ProbeFile a, ProbeFile b) => a.Path != b.Path;
    }
}