namespace SimpleIdServer.Mobile.Services
{
    public interface IPromptService
    {
        Task ShowAlert(string title, string message) => Application.Current.MainPage.DisplayAlert(title, message, "OK");
        Task<bool> ShowYesNo(string title, string message);
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

        public async Task<bool> ShowYesNo(string title, string message)
        {
            var isAccepted = false;
            await App.Current.Dispatcher.DispatchAsync(async () =>
            {
                isAccepted = await Application.Current.MainPage.DisplayAlert(title, message, "Yes", "No");
            });
            return isAccepted;
        }
    }
}
