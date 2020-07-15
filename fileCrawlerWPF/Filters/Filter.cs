using fileCrawlerWPF.Media;
using System;

namespace fileCrawlerWPF.Filters
{
    class Filter
        : IFilter
    {
        public Filter(Func<ProbeFile, object, bool> func)
            => Func = func;

        public bool Enabled { get; set; }
        public Func<ProbeFile, object, bool> Func { get; }
    }
}
