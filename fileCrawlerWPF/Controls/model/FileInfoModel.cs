using fileCrawlerWPF.Media;
using System.Windows.Media.Imaging;

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
        public string Directory { get => _file?.Directory;  }
        public string FrameRate { get => $"{_file?.FrameRate}"; }
        public string VideoCodec { get => _file?.VideoCodec; }
        public string AudioCodec { get => _file?.AudioCodec; }
        public string Size { get => _file?.FileSize; }
        public string Hash { get => _file?.HashAsHex; }
        public BitmapSource Image { get => _file?.GetThumbnail(); }


        public static bool operator == (FileInfoModel left, FileInfoModel right)
            => left?._file == right?._file;
        public static bool operator !=(FileInfoModel left, FileInfoModel right)
            => left?._file != right?._file;

    }
}
