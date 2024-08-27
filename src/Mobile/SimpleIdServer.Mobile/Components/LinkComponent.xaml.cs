using System.ComponentModel;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.Components;

public partial class LinkComponent : ContentView, INotifyPropertyChanged
{
	private string _title;
	private ImageSource _source;
	private string _url;

	public LinkComponent()
	{
		InitializeComponent();
		NavigateCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync(_url);
        });
        BindingContext = this;
    }
    public ICommand NavigateCommand { get; private set; }

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

	public ImageSource Source
	{
		get
		{
			return _source;
		}
		set
        {
            _source = value;
            base.OnPropertyChanged(nameof(Source));
        }
	}

	public string Url
    {
        get
        {
            return _url;
        }
        set
        {
            _url = value;
            base.OnPropertyChanged(nameof(Url));
        }
    }
}