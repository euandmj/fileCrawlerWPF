using System;

namespace fileCrawlerWPF.Events
{
    public class DirectorySelectedEventArgs
            : EventArgs
    {
        public string Path { get; private set; }


        public DirectorySelectedEventArgs(string path)
        {
            Path = path;
        }
    }
}
