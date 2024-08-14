namespace SimpleIdServer.Mobile.Services;

public interface INavigationService
{
    Task GoBack();
    Task<T> DisplayModal<T>() where T : ContentPage;
    Task<T> DisplayModal<T>(T content) where T : ContentPage;
}

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task GoBack()
    {
        return App.Current.Dispatcher.DispatchAsync(async () =>
        {
            await Shell.Current.GoToAsync("..");
        });
    }

    public Task<T> DisplayModal<T>() where T : ContentPage
    {
        var service = _serviceProvider.GetRequiredService<T>();
        return DisplayModal(service);
    }

    public async Task<T> DisplayModal<T>(T content) where T : ContentPage
    {
        await App.Current.Dispatcher.DispatchAsync(async () =>
        {
            var navigation = App.Current.MainPage.Navigation;
            await navigation.PushModalAsync(content);
        });
        return content;
    }
}
