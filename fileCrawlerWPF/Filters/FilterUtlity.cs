﻿using fileCrawlerWPF.Media;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace fileCrawlerWPF.Filters
{
    static class FilterUtlity
    {


        private static bool IsVideoAlias(string val, out string normalised)
        {
            normalised = string.Empty;
            if (Util.FileAliases.x265Aliases.Contains(val))
            {
                normalised = "hevc";
                return true;
            }
            else if (Util.FileAliases.x264Aliases.Contains(val))
            {
                normalised = "h264";
                return true;
            }
            else
                return false;
        }

        private static bool IsNameMatch(string src, string target)
        {
            for(int i = 0; i < src.Length; ++i)
            {
                var substr = src.Substring(0, i);

                if (target.ToLower().Contains(substr.ToLower()))
                    return true;
            }
            return false;
        }

        public static Func<ProbeFile, object, bool> ResFunc = (file, x) =>
        {
            return (file.Width * file.Height) >= (int)x;
        };

        public static Func<ProbeFile, object, bool> FramerateFunc = (file, x) =>
        {
            return file.FrameRate >= (int)x;
        };

        public static Func<ProbeFile, object, bool> VCodecFunc = (file, x) =>
        {
            if (IsVideoAlias((string)x, out string normalised))
            {
                return file.videoCodec.codec == normalised;
            }
            return false;
        };

        public static Func<ProbeFile, object, bool> ACodecFunc = (file, x) =>
        {
            return file.audioCodec.codec == (string)x;
        };

        public static Func<ProbeFile, object, bool> NameFunc = (file, x) =>
        {
            return IsNameMatch((string)x, file.Name);
        };

        public static Func<ProbeFile, object, bool> RegexFunc = (file, x) =>
        {
            var str = x as string;
            return Regex.IsMatch(file.Name, str);
        };
    }
}
