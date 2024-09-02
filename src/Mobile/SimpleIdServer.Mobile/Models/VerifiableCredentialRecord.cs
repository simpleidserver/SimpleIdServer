using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleIdServer.Mobile.Models;

public class VerifiableCredentialRecord : INotifyPropertyChanged
{
    private bool _isSelected;
    [PrimaryKey]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Format { get; set; }
    public string BackgroundColor { get; set; }
    public string TextColor { get; set; }
    public string Logo { get; set; }
    public string Description { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string SerializedVc { get; set; }
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
    public string DisplayLogo
    {
        get
        {
            return Logo ?? "credential.png";
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
