using System;
using System.Data.SqlTypes;

namespace fileCrawlerWPF
{
    public struct FileDirectory
    {
        public Guid ID { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }

        public FileDirectory(string p, string n)
        {
            ID = Guid.NewGuid();
            Path = p;
            Name = n;
        }
    }
}
