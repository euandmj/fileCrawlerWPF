using NReco.VideoInfo;
using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace fileCrawlerWPF
{
    public struct CodecInfo
    {
        public string codec;
        public string codecType;
    }

    public class ProbeFile
    {
        bool isLoggingEnabled;

        private string name;
        private int width, height;
        private float framerate;
        private CodecInfo audioCodec;
        private string path;
        private long size;
        private int index;
        private byte[] hash; 

        private TimeSpan duration;
        public CodecInfo videoCodec;

        public int Index { get { return index; } set { index = value; } }
        public string Name {  get { return name; } set { name = value; } }       
        public int Width { get { return width; } set { width = value; } }
        public int Height { get { return height; } set { height = value; } }
        public float FrameRate { get { return framerate; }set { framerate = value; } }
        public string VideoCodec { get { return videoCodec.codec; } }
        public string AudioCodec { get { return audioCodec.codec; } }
        public string Path { get { return path; } set { path = value; } }
        public TimeSpan Duration {  get { return duration; } }

        public string FileSize  
        {
            get
            {
                return size / 1000000 + " MB";
                //return size >= 1000000000 ? (size / 1024) / 1024 / 1024 + " GB" : size / 1024 + " MB";
            }
        }
        public string Hash { get { return hash?.ToString(); } }
        public string HashAsHex {
            get
            {
                return hash != null ? BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant() : null;
            }
        }

        

        public ProbeFile(string path, int index, bool logToggle = false)
        {
            this.path = path;
            this.index = index;


            //using (var shell = ShellObject.FromParsingName(Path))
            //{
            //    IShellProperty p = shell.Properties.System.Media.Duration;
            //    duration = p.FormatForDisplay(PropertyDescriptionFormatOptions.None).ToString();
            //}


            isLoggingEnabled = logToggle;

            name = this.path.Substring(this.path.LastIndexOf('\\') + 1);

            ReadFile();
        }

       
        
        void ReadFile()
        {
            try
            {
                FFProbe probe = new FFProbe();
                MediaInfo info = probe.GetMediaInfo(path);
                size = new FileInfo(path).Length;

                duration = info.Duration;

                foreach (var stream in info.Streams)
                {
                    if (isLoggingEnabled) Console.WriteLine("Stream {0} ({1})", stream.CodecName, stream.CodecType);


                    if (stream.CodecType == "video")
                    {
                        if (isLoggingEnabled) Console.WriteLine("\tFrame size: {0}x{1}", stream.Width, stream.Height);
                        if (isLoggingEnabled) Console.WriteLine("\tFrame rate: {0:0.##}", stream.FrameRate);
                        videoCodec.codec = stream.CodecName;
                        videoCodec.codecType = stream.CodecType;
                        width = stream.Width;
                        height = stream.Height;
                        framerate = stream.FrameRate;
                    }
                    else if (stream.CodecType == "audio")
                    {
                        audioCodec.codec = stream.CodecName;
                        audioCodec.codecType = stream.CodecType;
                    }
                }
                if (isLoggingEnabled) Console.WriteLine("\n");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Unable to locate the specific file. path is supplied as " + path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // this increases load times by way too much. apply for an option in file information. 
               // SetHash(); 
            }
        }

        public void ComputeHash()
        {
            using (new WaitCursor())
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        hash = md5.ComputeHash(stream);
                    }
                }
            }
            
        }

        public void PrintInfo()
        {
            Console.WriteLine("\n");
            Console.WriteLine($"Info for {this.name}" +
                $"\nDuration: {this.duration.ToString()}" +
                $"\nWxH: {this.width}x{this.height}" +
                $"\nFramrate: {this.framerate}");
        }
    }
}