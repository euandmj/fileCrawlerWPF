using fileCrawlerWPF.Media;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace fileCrawlerWPF.Controls.model
{
    public class FileInformation_ViewModel
        : INotifyPropertyChanged
    {
        private ProbeFile _file;

        public event PropertyChangedEventHandler PropertyChanged;

        public FileInformation_ViewModel()
        {
        }

        public FileInformation_ViewModel(ProbeFile pf)
            => _file = pf;

        public ProbeFile ProbeFile
        {
            get => _file;
            set
            {
                _file = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Enabled)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileName)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Resolution)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Directory)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FrameRate)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VideoCodec)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AudioCodec)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Size)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hash)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
            }
        }

        public async Task CalculateHash()
        {
            await _file?.ComputeHashAsync();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hash)));
        }

        public bool Enabled => _file != null;
        public Guid ID => _file.ID;
        public string FileName => _file?.Name;                    
        public string Resolution => _file?.Resolution;
        public string Directory => _file?.Directory;
        public string FrameRate => $"{_file?.FrameRate}";
        public string VideoCodec => _file?.VideoCodec;
        public string AudioCodec => _file?.AudioCodec;
        public string Size => _file?.FileSize;
        public string Hash => _file?.HashAsHex;
        public BitmapSource Image => _file?.GetThumbnail();

        public static bool operator == (FileInformation_ViewModel left, FileInformation_ViewModel right)
            => left?._file == right?._file;
        public static bool operator !=(FileInformation_ViewModel left, FileInformation_ViewModel right)
            => left?._file != right?._file;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
