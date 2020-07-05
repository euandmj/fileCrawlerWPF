using fileCrawlerWPF.Media;

namespace fileCrawlerWPF.Controls.model
{
    class FileInfoModel
    {
        private readonly ProbeFile _file;

        public FileInfoModel(ProbeFile pf)
        {
            _file = pf;
        }

        public string FileName { get => _file?.Name; }
        public string Resolution { get => _file?.Resolution; }
        public string Path { get => _file?.Path;  }
        public string FrameRate { get => $"{_file?.FrameRate}"; }
        public string VideoCodec { get => _file?.VideoCodec; }
        public string AudioCodec { get => _file?.AudioCodec; }
        public string Size { get => _file?.FileSize; }
        public string Hash { get => _file?.HashAsHex; }



    }
}
