using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileCrawlerWPF.Filters
{
    public interface IFilterable
    {
        FilterContext Context { get; set; }

    }
}
