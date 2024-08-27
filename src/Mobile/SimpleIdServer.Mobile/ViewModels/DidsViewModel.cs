using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Key;
using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Stores;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class DidsViewModel : INotifyPropertyChanged
{
    private bool _isLoading;
    private readonly DidRecordState _didRecordState;

    public DidsViewModel(DidRecordState didRecordState)
    {
        _didRecordState = didRecordState;
        AddCommand = new Command(async () =>
        {
            var exportResult = DidKeyGenerator.New().GenerateRandomES256Key().Export(false, true);
            var export = SignatureKeySerializer.Serialize(exportResult.Key);
            await _didRecordState.Add(new DidRecord { Did = exportResult.Did, SerializedPrivateKey = System.Text.Json.JsonSerializer.Serialize(export), CreateDateTime = DateTime.Now, IsActive = false });
        });
        DeleteCommand = new Command(async () =>
        {
            IsLoading = true;
            var activeDid = Dids.Single(d => d.IsSelected);
            await _didRecordState.Delete(activeDid);
            IsLoading = false;
            RefreshCommands();
        }, () =>
        {
            return Dids.Any() && Dids.Any(d => d.IsSelected);
        });
        SetActiveCommand = new Command(async () =>
        {
            IsLoading = true;
            foreach(var did in Dids)
            {
                if (did.IsSelected) did.IsActive = true;
                else did.IsActive = false;
            }

            await _didRecordState.Update(Dids);
            IsLoading = false;
        }, () =>
        {
            return Dids.Any() && Dids.Any(d => d.IsSelected);
        });
        CopyCommand = new Command<DidRecord>(async (d) =>
        {
            await Clipboard.Default.SetTextAsync(d.Did);
        });
        SelectCommand = new Command<DidRecord>((d) =>
        {
            foreach (var did in Dids)
                did.IsSelected = false;
            d.IsSelected = true;
            OnPropertyChanged(nameof(Dids));
            RefreshCommands();
        });
    }

    public bool IsLoading
    {
        get
        {
            return _isLoading;
        }
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddCommand { get; private set; }

    public ICommand DeleteCommand { get; private set; }

    public ICommand SetActiveCommand { get; private set; }

    public Command<DidRecord> SelectCommand { get; private set; }

    public Command<DidRecord> CopyCommand { get; private set; }

    public ObservableCollection<DidRecord> Dids
    {
        get
        {
            return _didRecordState.Dids;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void RefreshCommands()
    {
        ((Command)SetActiveCommand).ChangeCanExecute();
        ((Command)DeleteCommand).ChangeCanExecute();
    }
}