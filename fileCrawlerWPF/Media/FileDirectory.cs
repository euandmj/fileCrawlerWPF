using System;
using System.IO;

namespace fileCrawlerWPF.Media
{
    public struct FileDirectory
    {
        public Guid ID { get; set; }
        public DirectoryInfo DirectoryInfo { get; set; }
        public string Path => DirectoryInfo.FullName;
        public FileDirectory(string p)
        {
            ID = Guid.NewGuid();
            DirectoryInfo = new DirectoryInfo(p);
        }
    }
}
