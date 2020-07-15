using fileCrawlerWPF.Media;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace fileCrawlerWPF.Filters
{
    sealed class Filterer
    {
        // @todo maybe refactor into its own Wrapper?
        // experiment with [] operators
        private readonly Dictionary<FilterContext, IFilter> _filterMap;

        public Filterer()
        {
            _filterMap = new Dictionary<FilterContext, IFilter>();

            InitFilterMap();
        }

        public FilterLevel Level { get; set; } = FilterLevel.All;

        private void InitFilterMap()
        {
            _filterMap.Add(
                FilterContext.Resolution,
                new Filter(FilterUtlity.ResFunc));

            _filterMap.Add(
                FilterContext.Framerate,
                new Filter(FilterUtlity.FramerateFunc));

            _filterMap.Add(
                FilterContext.VideoCodec,
                new Filter(FilterUtlity.VCodecFunc)); 
            
            _filterMap.Add(
                 FilterContext.AudioCodec,
                 new Filter(FilterUtlity.ACodecFunc));

            _filterMap.Add(
                FilterContext.Name,
                new Filter(FilterUtlity.NameFunc));

            _filterMap.Add(
                FilterContext.Regex,
                new Filter(FilterUtlity.RegexFunc));

            _filterMap.Add(
                FilterContext.Extension,
                new Filter(FilterUtlity.ExtFunc));
        }       


        private bool FilterFile(
            IReadOnlyCollection<(FilterContext, object)> filterContexts,
            ProbeFile file)
        {
            bool isMatch = Level == FilterLevel.All;

            foreach ((FilterContext context, object value) in filterContexts)
            {

                if (_filterMap.TryGetValue(context, out IFilter mapValue) &&
                    mapValue.Enabled)
                {
                    var match = mapValue.Func(file, value);

                    isMatch =
                        Level == FilterLevel.All
                        ? isMatch &= match
                        : isMatch |= match;

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
            if (_filterMap.TryGetValue(ctx, out IFilter value))
            {
                value.Enabled = isenabled;
            }
        }
    }
}
