using System;

namespace fileCrawlerWPF.Events
{
    public class PathSelectedEventArgs
            : EventArgs
    {
        public string Path { get; private set; }


        public PathSelectedEventArgs(string path)
        {
            Path = path;
        }
    }
}
