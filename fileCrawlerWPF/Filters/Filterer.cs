using System;
using System.Collections.Generic;
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





        public Filterer()
        {
            _filterMap = new Dictionary<FilterContext, (bool enabled, Func<ProbeFile, object, bool> func)>();



            InitFilterMap();
        }

        private void InitFilterMap()
        {
            //@ replace string key with enum
            //@ define val as string/int. shouldnt need two dictionaries
            _filterMap.Add(
                FilterContext.Resolution,
                (false, 
                (file, x) =>
                {
                    if (x is int val)
                        return (file.Width * file.Height) >= val;
                    else
                        return false;
                }));

            _filterMap.Add(
                FilterContext.Framerate,
                (false,
                (file, x) =>
                {
                    if (x is int val)
                        return file.FrameRate >= val;
                    else
                        return false;
                }));


            _filterMap.Add(
                FilterContext.VideoCodec,
                (false,
                (file, x) =>
                {
                    if(x is string val)
                    {
                        if (FilterUtlity.IsVideoAlias(val, out string normalised))
                        {
                            return file.videoCodec.codec == normalised;
                        }

                    }
                    return false;
                }
            ));

            _filterMap.Add(
                FilterContext.AudioCodec,
                (false,
                (file, x) =>
                {
                    if (x is string val)
                        return file.audioCodec.codec == val;
                    else 
                        return false;
                }));

            _filterMap.Add(
                FilterContext.Name,
                (false,
                (file, x) =>
                {                    
                    if (x is string val)
                        return IsNameMatch(val, file.Name);
                    else
                        return false;
                }
            ));
        }


        private bool IsNameMatch(string src, string target)
        {            
            foreach(string s in src.Split(' '))
            {
                if (string.Compare(s, target, ignoreCase: true) == 0)
                    return true;
            }
            return false;
        }

        public bool FilterFile(IEnumerable<(FilterContext, object)> filterContexts, ProbeFile file)
        {
            bool isMatch = false;

            foreach ((FilterContext context, object value) in filterContexts)
            {
                if (_filterMap.TryGetValue(context, out (bool enabled, Func<ProbeFile, object, bool> func) mapValue))
                {
                    if (!mapValue.enabled) continue;

                    isMatch &= mapValue.func(file, value);
                }
            }

            return isMatch;
        }












    }
}
