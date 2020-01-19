using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileCrawlerWPF
{
    class FileAliases
    {
        public static ReadOnlyCollection<string> x264Aliases =
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

        public static  ReadOnlyCollection<string> x265Aliases =
            new ReadOnlyCollection<string>(new[]
            {
                "h265",
                "hevc",
                "hevc/h.265",
                "x265"
            });
    }
}
