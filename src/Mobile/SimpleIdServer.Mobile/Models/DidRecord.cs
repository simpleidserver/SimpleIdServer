using SimpleIdServer.Did;
using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleIdServer.Mobile.Models
{
    public class DidRecord : INotifyPropertyChanged
    {
        private bool _isActive;
        private bool _isSelected;
        [PrimaryKey]
        public string Did { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string SerializedPrivateKey { get; set; }
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public string DisplayName
        {
            get
            {
                var extractor = DidExtractor.Extract(Did);
                return $"{CreateDateTime.ToString()} - did:{extractor.Method}";
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
