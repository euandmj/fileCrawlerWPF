using fileCrawlerWPF.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace fileCrawlerWPF.Controls
{
    class FilterContextCheckBox
        : CheckBox
    {

        public FilterContext FilterContext { get; set; }

        public FilterContextCheckBox()
        {
            
        }
    }
}
