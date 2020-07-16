using System;
using System.Collections.Generic;

namespace fileCrawlerWPF.Media
{
    public sealed class MediaManager
    {
        private static readonly object _lock = new object();
        private static readonly Lazy<MediaCollection> _media
            = new Lazy<MediaCollection>(() => new MediaCollection());
        public static MediaCollection MediaCollectionInstance => _media.Value;



        public static IReadOnlyCollection<ProbeFile> RequestFilter()
        {
            lock (_lock)
            {
                MediaCollectionInstance.CacheAll();
                return MediaCollectionInstance.CachedFiles;
            }
        }
    }
}
