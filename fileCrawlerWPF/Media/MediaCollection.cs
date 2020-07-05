using fileCrawlerWPF.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace fileCrawlerWPF.Media
{
    public class MediaCollection
    {
        private readonly Dictionary<Guid, ProbeFile> _cache;

        public MediaCollection()
        {
            _cache = new Dictionary<Guid, ProbeFile>();
            Directories = new ObservableCollection<FileDirectory>();
        }

        public ObservableCollection<FileDirectory> Directories { get; private set; }

        public int TotalFilesCount { get { return Directories.Count(); } }
        public IReadOnlyCollection<ProbeFile> CachedFiles { get => _cache.Values; }              

        //private void RemoveNonVideoFiles()
        //{
        //    // work out which keys belong to non video files
        //    var keys_to_remove = _cache
        //        .Where(entry => entry.Value.videoCodec.codecType != "video")
        //        .Select(entry => entry.Key);

        //    // remove the non-video entries from the dictionary. 
        //    foreach (var k in keys_to_remove)
        //    {
        //        _cache.Remove(k);
        //        int index = Directories.FindIndex(x => x.Key == k);
        //        Directories.RemoveAt(index);
        //    }
        //}

        private ProbeFile Cache(Guid id, string path)
        {
            var pf = new ProbeFile(path, id);
            _cache.Add(pf.ID, pf);
            return pf;
        }

        public ProbeFile GetFileFromCache(FileDirectory item)
            => !_cache.ContainsKey(item.ID)
            ? Cache(item.ID, item.Path)
            : _cache[item.ID];

        public ProbeFile GetFileFromCache(Guid id)
        {
            _cache.TryGetValue(id, out ProbeFile pf);
            return pf;
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
                            Directories.Add(new FileDirectory(path, Path.GetFileName(path)));
                    }
                    else if (Directory.Exists(path))
                    {
                        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                        files.ToList().ForEach(x =>
                        {
                            if (!Directories.Any(y => y.Path == x))
                                Directories.Add(new FileDirectory(x, Path.GetFileName(x)));
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
                if (!_cache.ContainsKey(item.ID))
                    _cache.Add(item.ID, new ProbeFile(item.Path, item.ID));
            }
        }
    }
}