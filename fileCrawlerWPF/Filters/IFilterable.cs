using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileCrawlerWPF.Filters
{
    public interface IFilterable
    {


        bool PassFilter(Predicate<bool> predicate);
    }
}
