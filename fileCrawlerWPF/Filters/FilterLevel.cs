using System;

namespace fileCrawlerWPF.Filters
{
    [Flags]
    enum FilterLevel
    {
        OneOrMany   = 0,
        All         = 1
    }
}
