using System;

namespace fileCrawlerWPF
{
    public struct ListViewItem
    {
        public Guid ID { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
    }
}
