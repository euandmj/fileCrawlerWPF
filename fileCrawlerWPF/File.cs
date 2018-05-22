using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NReco.VideoInfo;
using System.IO;

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

        public TimeSpan duration;
        public CodecInfo videoCodec;

        public string Name {  get { return name; } set { name = value; } }
        public int Duration { get { return duration.Minutes; }
            set
            {
                duration = TimeSpan.Parse(value.ToString());
            }
        }
        public int Width { get { return width; } set { width = value; } }
        public int Height { get { return height; } set { height = value; } }
        public float FrameRate { get { return framerate; }set { framerate = value; } }
        public string VideoCodec { get { return videoCodec.codec; } }
        public string AudioCodec { get { return audioCodec.codec; } }
        public string Path { get { return path; } set { path = value; } }
        public string FileSize
        {
            get
            {
                return size / 1000000 + " MB";
                //return size >= 1000000000 ? (size / 1024) / 1024 / 1024 + " GB" : size / 1024 + " MB";
            }
        }
       
        // encapsulate
        // file size variable return

        public ProbeFile(string path, bool logToggle = false)
        {
            this.path = path;
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
                    if(isLoggingEnabled) Console.WriteLine("Stream {0} ({1})", stream.CodecName, stream.CodecType);
                   

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
                    else if(stream.CodecType == "audio")
                    {
                        audioCodec.codec = stream.CodecName;
                        audioCodec.codecType = stream.CodecType;
                    }
                }
                if (isLoggingEnabled) Console.WriteLine("\n");
            }catch(FileNotFoundException)
            {
                Console.WriteLine("Unable to locate the specific file. path is supplied as " + path);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
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