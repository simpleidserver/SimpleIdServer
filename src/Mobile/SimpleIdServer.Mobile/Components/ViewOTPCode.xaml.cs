using SimpleIdServer.Mobile.Models;
using System.ComponentModel;

namespace SimpleIdServer.Mobile.Components;

public partial class ViewOTPCode : ContentView, INotifyPropertyChanged
{
	private string _displayMessage;
	private bool _isOTPCodeExists = false;
	public static readonly BindableProperty OTPCodeProperty = BindableProperty.Create(nameof(OTPCode), typeof(OTPCode), typeof(ViewOTPCode), propertyChanged: OnOTPCodeChanged);

	public ViewOTPCode()
	{
		InitializeComponent();
	}

	public OTPCode OTPCode
	{
		get {  return (OTPCode)GetValue(OTPCodeProperty); }
		set { SetValue(OTPCodeProperty, value); }
    }


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

    static void OnOTPCodeChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var viewModelOTPCode = bindable as ViewOTPCode;
		var newOTPCode = newValue as OTPCode;
		MainThread.BeginInvokeOnMainThread(() =>
        {
            viewModelOTPCode.IsOTPCodeExists = true;
            viewModelOTPCode.DisplayMessage = $"{newOTPCode.Issuer} : the token for {newOTPCode.Name} is ";
        });
    }
}