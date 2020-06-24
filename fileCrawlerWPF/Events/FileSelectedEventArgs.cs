using System;

namespace fileCrawlerWPF.Events
{
    public class FileSelectedEventArgs
        : EventArgs
    {
        public readonly Guid FileID;

        public FileSelectedEventArgs(Guid id)
        {
            FileID = id;
        }

        public delegate void FileSelectedEventHandler(object sender, FileSelectedEventArgs e);
    }
}
