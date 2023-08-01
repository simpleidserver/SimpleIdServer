namespace SimpleIdServer.Mobile.Services
{
    public interface IPromptService
    {
        Task ShowAlert(string title, string message) => Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    public class PromptService : IPromptService
    {
        public async Task ShowAlert(string title, string message)
        {
            await App.Current.Dispatcher.DispatchAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            });
        }
    }
}
