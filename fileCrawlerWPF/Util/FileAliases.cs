using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace fileCrawlerWPF.Util
{
    class FileAliases
    {
        public static IReadOnlyCollection<string> x264Aliases =
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

        public static  IReadOnlyCollection<string> x265Aliases =
            new ReadOnlyCollection<string>(new[]
            {
                "h265",
                "hevc",
                "hevc/h.265",
                "x265"
            });
    }
}
