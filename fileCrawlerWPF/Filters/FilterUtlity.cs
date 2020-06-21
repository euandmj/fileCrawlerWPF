using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileCrawlerWPF.Filters
{
    class FilterUtlity
    {


        public static bool IsVideoAlias(string val, out string normalised)
        {
            normalised = string.Empty;
            if (FileAliases.x265Aliases.Contains(val))
            {
                normalised = "hevc";
                return true;
            }
            else if (FileAliases.x264Aliases.Contains(val))
            {
                normalised = "h264";
                return true;
            }
            else
                return false;
        }
    }
}
