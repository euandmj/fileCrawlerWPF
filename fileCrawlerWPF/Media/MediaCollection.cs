using fileCrawlerWPF.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace fileCrawlerWPF.Media
{
    public sealed class MediaCollection
    {
        private readonly Dictionary<Guid, ProbeFile> _cache;

        public MediaCollection()
        {
            _cache = new Dictionary<Guid, ProbeFile>();
            Directories = new ObservableCollection<FileDirectory>();
        }


        public int TotalFilesCount { get { return Directories.Count(); } }
        public ObservableCollection<FileDirectory> Directories { get; private set; }        
        public IReadOnlyCollection<ProbeFile> CachedFiles { get => _cache.Values; }

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
                int index = Directories.ToList().FindIndex(x => x.ID == k);
                Directories.RemoveAt(index);
            }            
        }

        private ProbeFile Cache(Guid id, string path)
        {
            if (_cache.ContainsKey(id))
            {
                return _cache[id];
            }
            else
            {
                var pf = new ProbeFile(path, id);
                _cache.Add(pf.ID, pf);
                return pf;
            }
        }

        public ProbeFile GetFileFromCache(FileDirectory item)
            => Cache(item.ID, item.Path);

        public ProbeFile GetFileFromCache(Guid id)
            => Cache(id, string.Empty);

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
                        Directories.Add(new FileDirectory(path));
                    }
                    else if (Directory.Exists(path))
                    {
                        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                        files.ToList().ForEach(x =>
                        {
                            Directories.Add(new FileDirectory(x));
                        });
                    }
                }
            }
            catch (Exception)
            {
                var logged = Directories.FirstOrDefault(x => x.Path == path);
                if (!(logged.DirectoryInfo.Name is null))
                    Directories.Remove(logged);
                throw;
            }
        }

        public void CacheAll()
        {
            foreach (var item in Directories)
            {
                Cache(item.ID, item.Path);
            }
        }

        public void Reset()
        {
            _cache.Clear();
            Directories.Clear();
        }
    }
}