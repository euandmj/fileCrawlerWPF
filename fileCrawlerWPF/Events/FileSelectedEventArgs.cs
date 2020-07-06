using fileCrawlerWPF.Media;
using System;

namespace fileCrawlerWPF.Events
{
    public class FileSelectedEventArgs
        : EventArgs
    {
        public Guid ID;
        public readonly FileDirectory Directory;

        public FileSelectedEventArgs(Guid id)
        {
            ID = id;
        }
        
        public FileSelectedEventArgs(FileDirectory fd)
        {
            Directory = fd;
        }
    }
}
