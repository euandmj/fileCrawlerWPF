using fileCrawlerWPF.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileCrawlerWPF.Filters
{
    interface IFilter
    {


        bool Enabled { get; set; }
        Func<ProbeFile, object, bool> Func { get; }
    }
}
