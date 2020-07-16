using fileCrawlerWPF.Exceptions;
using fileCrawlerWPF.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace fileCrawlerWPF.Media
{
    public sealed class MediaCollection
    {
        private readonly object _lock = new object();
        private readonly Dictionary<Guid, ProbeFile> _cache;

        public MediaCollection()
        {
            _cache = new Dictionary<Guid, ProbeFile>();
            Directories = new ObservableCollection<FileDirectory>();
        }

        public ObservableCollection<FileDirectory> Directories { get; private set; }
        public IReadOnlyCollection<ProbeFile> CachedFiles { get => _cache.Values; }

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

        public ProbeFile GetFile(FileDirectory item)
            => Cache(item.ID, item.Path);

        public ProbeFile GetFile(Guid id)
            => Cache(id, string.Empty);

        public void ProcessDirectory(string path)
        {
            lock (_lock)
            {
                if (Directories.Any(x => x.Path == path))
                    throw new DirectoryAlreadyExistsException(path);

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
                catch (Exception)
                {
                    // attempt to remove this directory from the list. 
                    var logged = Directories.FirstOrDefault(x => x.Path == path);
                    if (!(logged.Name is null))
                        Directories.Remove(logged);
                    throw;
                }
            }
        }

        public void CacheAll()
        {
            foreach (var item in Directories)
            {
                Cache(item.ID, item.Path);
            }
        }

        public void RemoveFile(Guid id)
        {
            lock (_lock)
            {
                if (!Directories.Any(x => x.ID == id)) throw new ArgumentException("id is not recognised");

                var dir = Directories.First(x => x.ID == id);

                Directories.Remove(dir);
                _cache.Remove(id);
            }
        }

        public void Reset()
        {
            lock(_lock)
            {
                _cache.Clear();
                Directories.Clear();
            }
        }
    }
}