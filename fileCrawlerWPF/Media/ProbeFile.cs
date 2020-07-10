using Microsoft.WindowsAPICodePack.Shell;
using NReco.VideoInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace fileCrawlerWPF.Media
{
    public struct CodecInfo
    {
        public enum CodecType
        {
            Video,
            Audio
        }

        public string codec;
        public CodecType codecType;
    }

    public class ProbeFile : IEquatable<ProbeFile>
    {
        protected long _size;
        protected BitmapSource _thumbnail;
        protected readonly DirectoryInfo _directory;

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
            => Hash != null
            ? $"#{BitConverter.ToString(Hash).Replace("-", "").ToLowerInvariant()}"
            : null;

        public byte[] Hash { get; set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public float FrameRate { get; protected set; }
        public TimeSpan Duration { get; protected set; }
        public IList<string> FileTypes { get; protected set; }

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
                FileTypes = info.FormatName.Split(',');

                foreach (var stream in info.Streams)
                {
                    if (stream.CodecType == "video")
                    {
                        videoCodec.codec = stream.CodecName;
                        videoCodec.codecType = CodecInfo.CodecType.Video;
                        Width = stream.Width;
                        Height = stream.Height;
                        FrameRate = stream.FrameRate;
                    }
                    else if (stream.CodecType == "audio")
                    {
                        audioCodec.codec = stream.CodecName;
                        audioCodec.codecType = CodecInfo.CodecType.Audio;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Unable to locate the specific file. path is supplied as " + Path);
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

        public async Task<byte[]> ComputeHashAsync()
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(Path))
            {
                return await Task.Run(() =>
                {
                    return md5.ComputeHash(stream);
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