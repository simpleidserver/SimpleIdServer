using System.ComponentModel;

namespace SimpleIdServer.Mobile.Components;

public partial class Badge : ContentView, INotifyPropertyChanged
{
    private string _title;

	public Badge()
	{
		InitializeComponent();
        BindingContext = this;
    }

    public string Title
    {
        get
        {
            return _title;
        }
        set
        {
            _title = value;
            base.OnPropertyChanged(nameof(Title));
        }
    }
}