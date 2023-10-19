namespace SimpleIdServer.Mobile.Services;

public interface INavigationService
{
    Task GoBack();
}

public class NavigationService : INavigationService
{
    public Task GoBack()
    {
        return App.Current.Dispatcher.DispatchAsync(async () =>
        {
            await Shell.Current.GoToAsync("..");
        });
    }
}
