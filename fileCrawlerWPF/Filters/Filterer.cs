using fileCrawlerWPF.Media;
using System;
using System.Collections.Generic;

namespace fileCrawlerWPF.Filters
{
    sealed class Filterer
    {
        // @todo refactor Value into a struct. Then maybe into its own Wrapper?
        private readonly Dictionary<FilterContext, (bool, Func<ProbeFile, object, bool>)> _filterMap;

        public Filterer()
        {
            _filterMap = new Dictionary<FilterContext, (bool, Func<ProbeFile, object, bool>)>();

            InitFilterMap();
        }

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

            _filterMap.Add(
                FilterContext.Regex,
                (false,
                FilterUtlity.RegexFunc));
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
