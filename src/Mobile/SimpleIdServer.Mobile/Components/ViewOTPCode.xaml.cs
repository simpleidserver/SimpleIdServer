using SimpleIdServer.Mobile.Extensions;
using SimpleIdServer.Mobile.Models;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.Components;

public partial class ViewOTPCode : ContentView, INotifyPropertyChanged
{
	private CancellationTokenSource _totpCancellationTokenSource;
	private string _displayMessage;
	private string _code;
	private string _description;
	private bool _isOTPCodeExists = false;
	public static readonly BindableProperty OTPCodeProperty = BindableProperty.Create(nameof(OTPCode), typeof(OTPCode), typeof(ViewOTPCode), propertyChanged: OnOTPCodeChanged);

	public ViewOTPCode()
    {
        GenerateHOTPCommand = new Command(() =>
        {
            GenerateHOTPCode();
        }, () =>
        {
            return OTPCode?.Type == OTPCodeTypes.HOTP;
        });
        InitializeComponent();
    }

	public OTPCode OTPCode
	{
		get {  return (OTPCode)GetValue(OTPCodeProperty); }
		set { SetValue(OTPCodeProperty, value); }
    }

	public ICommand GenerateHOTPCommand { get; set; } 

    public string DisplayMessage
	{
		get { return _displayMessage; }
		set 
		{
			if(_displayMessage != value)
			{
				_displayMessage = value;
				base.OnPropertyChanged(nameof(DisplayMessage));
			}
		}
    }

	public bool IsOTPCodeExists
	{
		get { return _isOTPCodeExists; }
		set
		{
			if(_isOTPCodeExists != value)
			{
				_isOTPCodeExists = value;
				base.OnPropertyChanged(nameof(IsOTPCodeExists));
            }
		}
	}

	public string Code
	{
		get
		{
			return _code;
		}
		set
		{
			if(_code != value)
			{
				_code = value;
				base.OnPropertyChanged(nameof(Code));
			}
		}
	}

	public string Description
	{
		get
		{
			return _description;
		}
		set
		{
			if(_description != value)
			{
				_description = value;
				base.OnPropertyChanged(nameof(Description));
			}
		}
	}

    static void OnOTPCodeChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var viewModelOTPCode = bindable as ViewOTPCode;
		var newOTPCode = newValue as OTPCode;
		if(newOTPCode == null)
        {
            viewModelOTPCode.IsOTPCodeExists = false;
			viewModelOTPCode.DisplayMessage = null;
            if (viewModelOTPCode._totpCancellationTokenSource != null)
            {
                viewModelOTPCode._totpCancellationTokenSource.Cancel();
            }
        }
		else
        {

            viewModelOTPCode.IsOTPCodeExists = true;
            viewModelOTPCode.DisplayMessage = $"{newOTPCode.Issuer} : the token for {newOTPCode.Name} is ";
            viewModelOTPCode.GenerateCode(newOTPCode);
        }
    }

	private void GenerateCode(OTPCode otpCode)
	{
		if(_totpCancellationTokenSource != null)
		{
			_totpCancellationTokenSource.Cancel();
		}

		if(otpCode.Type == OTPCodeTypes.TOTP)
		{
			_totpCancellationTokenSource = new CancellationTokenSource();
			Task.Run(async () => await GenerateTOTPCode());
        }
		else
		{
			GenerateHOTPCode();
		}

		var cmd = (Command)GenerateHOTPCommand;
		cmd.ChangeCanExecute();
	}

	private async Task GenerateTOTPCode()
	{
		int remainingTimeInSeconds = OTPCode.Period;
		Description = string.Empty;
		while (!_totpCancellationTokenSource.IsCancellationRequested)
		{
			if(remainingTimeInSeconds == OTPCode.Period || remainingTimeInSeconds == 0)
			{
                var code = GenerateCode(OTPCode.Secret, CalculateTimeStep(DateTime.UtcNow, OTPCode));
				if (remainingTimeInSeconds == 0) remainingTimeInSeconds = OTPCode.Period;
				Code = code.ToString();
            }

			await Task.Delay(1000);
			if (_totpCancellationTokenSource.IsCancellationRequested) return;
			remainingTimeInSeconds--;
			Description = $"The code is still valid {remainingTimeInSeconds} seconds";
		}
    }

	private void GenerateHOTPCode()
    {
        var code = GenerateCode(OTPCode.Secret, OTPCode.Counter);
		OTPCode.Counter++;
		Code = code.ToString();
		Description = $"Counter is {OTPCode.Counter}";
    }

    protected long GenerateCode(string key, long counter)
    {
		var payload = key.ConvertToBase32();
        var data = BitConverter.GetBytes(counter);
        Array.Reverse(data);
        byte[] hashed;
        using (var hmac = new HMACSHA1())
        {
            hmac.Key = payload;
            hashed = hmac.ComputeHash(data);
        }

        int offset = hashed[hashed.Length - 1] & 0x0F;
        var otp = (hashed[offset] & 0x7f) << 24
            | (hashed[offset + 1] & 0xff) << 16
            | (hashed[offset + 2] & 0xff) << 8
            | (hashed[offset + 3] & 0xff) % 1000000;
        var truncatedValue = ((int)otp % (int)Math.Pow(10, 6));
        return long.Parse(truncatedValue.ToString().PadLeft(6, '0'));
    }

    private long CalculateTimeStep(DateTime dateTime, OTPCode otpCode)
    {
        var unixTimestamp = dateTime.ConvertToUnixTimestamp();
        var window = (long)unixTimestamp / (long)otpCode.Period;
        return window;
    }
}