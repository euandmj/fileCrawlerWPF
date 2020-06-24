using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace fileCrawlerWPF.Filters
{
    class Filterer
    {
        private readonly Dictionary<FilterContext, (bool, Func<ProbeFile, object, bool>)> _filterMap;
        //private readonly Dictionary<FilterContext, bool> _filterEnabled;

        public Filterer()
        {
            _filterMap = new Dictionary<FilterContext, (bool, Func<ProbeFile, object, bool>)>();
            //_filterEnabled = new Dictionary<FilterContext, bool>();




            InitFilterMap();
        }

        //private IReadOnlyCollection<FilterContext> _EnabledFilters
        //{
        //    get => _filterMap
        //        .Where(x => x.Value)
        //        .Select(y => y.Key.Context)
        //        .ToList();
        //}

        private void InitFilterMap()
        {
            _filterMap.Add(
                FilterContext.Resolution,
                (false,
                FilterUtlity.ResFunc));

            _filterMap.Add(
                FilterContext.Framerate,
                (false,
                FilterUtlity.FramerateFunc));

            _filterMap.Add(
                FilterContext.VideoCodec,
                (false,
                FilterUtlity.VCodecFunc)); 
            
            _filterMap.Add(
                 FilterContext.AudioCodec,
                 (false,
                 FilterUtlity.ACodecFunc));

            _filterMap.Add(
                FilterContext.Name,
                (false,
                FilterUtlity.NameFunc));    
        }       


        private bool FilterFile(
            IReadOnlyCollection<(FilterContext, object)> filterContexts,
            ProbeFile file)
        {
            bool isMatch = false;

            foreach ((FilterContext context, object value) in filterContexts)
            {

                if (_filterMap.TryGetValue(context, out (bool enabled, Func<ProbeFile, object, bool> func) mapValue))
                {
                    if(mapValue.enabled)
                    {
                        // @TODO loose (any) and hard (all) matching
                        isMatch |= mapValue.func(file, value);
                    }
                }
            }

            return isMatch;
        }

        public IEnumerable<ProbeFile> Filter(
            IReadOnlyCollection<(FilterContext, object)> filterContexts,
            IReadOnlyCollection<ProbeFile> files)
        {
            // @todo validate that filterContexts is distinct

            foreach (var file in files)
            {
                if (FilterFile(filterContexts, file))
                    yield return file;
            }
        }

        public void ToggleFilter(FilterContext ctx, bool isenabled)
        {
            if (_filterMap.TryGetValue(ctx, out (bool, Func<ProbeFile, object, bool> func) value))
            {
                _filterMap[ctx] = (isenabled, value.func);
            }
        }
    }
}
