using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileCrawlerWPF.Filters
{
    internal class FilterCollection
    {

        private readonly Dictionary<FilterContext, IFilter> _filterMap;




        public FilterCollection()
        {
            _filterMap = new Dictionary<FilterContext, IFilter>();

        }


    }
}
