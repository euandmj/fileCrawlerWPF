using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileCrawlerWPF
{
    public class MediaCollection
    {
        // Illegal path characters
        private readonly char[] _illegal_chars = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
        private const char _path_seperator_token = '>';

        private readonly Dictionary<Guid, ProbeFile> _cache;

        public MediaCollection()
        {
            _cache = new Dictionary<Guid, ProbeFile>();
            FilteredFiles = new List<(Guid Key, string Name)>();
            Directories = new List<(Guid Key, string Path, string Name)>();
        }

        public List<(Guid Key, string Name)> FilteredFiles { get; private set; } 
        public List<(Guid Key, string Path, string Name)> Directories { get; private set; } 

        public int TotalFilesCount { get { return Directories.Count(); } }
        public IEnumerable<ProbeFile> CachedFiles { get => _cache.Values; }

        public ProbeFile GetByIndex(int index)
        {
            var (Key, _) = FilteredFiles.ElementAt(index);
            _cache.TryGetValue(Key, out ProbeFile pf);

            return pf;
        }

        public ProbeFile GetFileFromCache(ListViewItem item)
        {
            if (!_cache.ContainsKey(item.ID))
            {
                var pf = new ProbeFile(item.Path, item.ID);
                _cache.Add(pf.ID, pf);
                return pf;
            }
            else
            {
                return _cache[item.ID];
            }
        }

        private void RemoveNonVideoFiles()
        {
            // work out which keys belong to non video files
            var keys_to_remove = _cache
                .Where(entry => entry.Value.videoCodec.codecType != "video")
                .Select(entry => entry.Key);

            // remove the non-video entries from the dictionary. 
            foreach (var k in keys_to_remove)
            {
                _cache.Remove(k);
                int index = Directories.FindIndex(x => x.Key == k);
                Directories.RemoveAt(index);
            }
        }


        public void ProcessDirectory(string path)
        {
            if (Directories.Any(x => x.Path == path))
                return;

            try
            {
                using (new WaitCursor())
                {
                    if (File.Exists(path))
                    {
                        if (!Directories.Any(x => x.Path == path))
                            Directories.Add((Guid.NewGuid(), path, Path.GetFileName(path)));
                    }
                    else if (Directory.Exists(path))
                    {
                        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                        files.ToList().ForEach(x =>
                        {
                            if (!Directories.Any(y => y.Path == x))
                                Directories.Add((Guid.NewGuid(), x, Path.GetFileName(x)));
                        });
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
        }

        public void CacheAll()
        {
            foreach (var item in Directories)
            {
                if (!_cache.ContainsKey(item.Key))
                    _cache.Add(item.Key, new ProbeFile(item.Path, item.Key));
            }
        }
    }
}