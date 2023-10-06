using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using SimpleIdServer.Mobile.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels;

public class EnrollViewModel : INotifyPropertyChanged
{
    private readonly IBluetoothService _bluetoothService;
    private readonly IAdapter _adapter;
    private DeviceInfoViewModel _selectedDevice = new DeviceInfoViewModel("Select a device");
    private bool _isScanning = false;
    private bool _isDeveloperModeEnabled = false;
    private bool _isDeviceConnectionDisplayed = true;
    private bool _isScanDisplayed = false;
    private bool _isListeningDevice = false;
    private string _selectedDeviceName;

    public EnrollViewModel(SettingsPageViewModel settings, IBluetoothService bluetoothService)
    {
        _bluetoothService = bluetoothService;
        // https://developer.android.com/guide/topics/connectivity/companion-device-pairing
        _adapter = CrossBluetoothLE.Current.Adapter;
        _adapter.ScanTimeout = 5000;
        _adapter.ScanMode = ScanMode.LowLatency;
        _adapter.DeviceDiscovered += HandleDeviceDiscovered;
        Settings = settings;
        ScanDevicesCommand = new Command(() =>
        {
            ScanDevices();
        }, () =>
        {
            return !_isScanning;
        });
        ConnectDeviceCommand = new Command(async () =>
        {
            await ConnectDevice();
        }, () =>
        {
            return SelectedDevice.Device != null;
        });
        SubmitQRCodeCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("enrollsubmitqrcode");
        });
        ScanQRCodeCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("enrollscanqrcode");
        });
        Devices.Add(SelectedDevice);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public DeviceInfoViewModel SelectedDevice
    {
        get
        {
            return _selectedDevice;
        }
        set
        {
            _selectedDevice = value;
            Refresh(ConnectDeviceCommand);
        }
    }

    public ObservableCollection<DeviceInfoViewModel> Devices { get; set; } = new ObservableCollection<DeviceInfoViewModel>();

    public bool IsDeveloperModeEnabled
    {
        get => _isDeveloperModeEnabled;
        set
        {
            if (_isDeveloperModeEnabled != value)
            {
                _isDeveloperModeEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public string SelectedDeviceName
    {
        get => _selectedDeviceName;
        set
        {
            if (_selectedDeviceName != value)
            {
                _selectedDeviceName = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsDeviceConnectionDisplayed
    {
        get => _isDeviceConnectionDisplayed;
        set
        {
            if(_isDeviceConnectionDisplayed != value)
            {
                _isDeviceConnectionDisplayed = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsScanDisplayed
    {
        get => _isScanDisplayed;
        set
        {
            if(_isScanDisplayed != value)
            {
                _isScanDisplayed = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsListeningDevice
    {
        get => _isListeningDevice;
        set
        {
            if (_isListeningDevice != value)
            {
                _isListeningDevice = value;
                OnPropertyChanged();
            }
        }
    }

    public SettingsPageViewModel Settings { get; private set; }

    public ICommand ScanDevicesCommand { get; private set; }

    public ICommand ConnectDeviceCommand { get; private set; }

    public ICommand SubmitQRCodeCommand { get; private set; }

    public ICommand ScanQRCodeCommand { get; private set; }

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void HandleDeviceDiscovered(object sender, DeviceEventArgs e)
    {
        if (!e.Device.IsConnectable) return;
        const string gg = "0000fffd-0000-1000-8000-00805f9b34fb";
        if (Devices.Any(d => e.Device.Id == d.Id)) return;
        Devices.Add(new DeviceInfoViewModel(e.Device.Id, e.Device.Name, e.Device));
    }

    private async void ScanDevices()
    {
        ToggleIsScanning(true);
        /*
        for (var i = Devices.Count() - 1; i > 0; i--) Devices.RemoveAt(i);
        await _adapter.StartScanningForDevicesAsync();
        */
        _bluetoothService.Listen();
        ToggleIsScanning(false);
    }

    private async Task ConnectDevice()
    {
        try
        {
            await _adapter.ConnectToKnownDeviceAsync(SelectedDevice.Id);
            // await _adapter.ConnectToDeviceAsync(SelectedDevice.Device);
            var services = await SelectedDevice.Device.GetServicesAsync();
            string ss = "";
        }
        catch(Exception ex)
        {

        }
    }

    public void Init()
    {
        MobileSettings mobileSettings = null;
        Task.Run(async () =>
        {
            mobileSettings = await App.Database.GetMobileSettings();
        }).Wait();
        IsDeveloperModeEnabled = mobileSettings.IsDeveloperModeEnabled;
    }

    private void ToggleIsScanning(bool v)
    {
        _isScanning = v;
        Refresh(ScanDevicesCommand);
        _isScanning = v;
        Refresh(ScanDevicesCommand);
    }

    private void Refresh(ICommand cmd)
    {
        (cmd as Command).ChangeCanExecute();
    }
}

public record DeviceInfoViewModel
{
    public DeviceInfoViewModel(string name)
    {
        Name = name;
    }

    public DeviceInfoViewModel(Guid id, string name, IDevice device) : this(name)
    {
        Id = id;
        Device = device;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public IDevice Device { get; private set; }
}