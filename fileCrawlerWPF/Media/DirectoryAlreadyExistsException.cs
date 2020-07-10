using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileCrawlerWPF.Media
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
