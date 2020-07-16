using System;

namespace fileCrawlerWPF.Exceptions
{
    class DirectoryAlreadyExistsException
        : Exception
    {
        public string Directory { get; private set; }

        public DirectoryAlreadyExistsException(string d)
        {
            Directory = d;
        }
    }
}
