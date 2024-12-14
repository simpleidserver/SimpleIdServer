namespace FormBuilder.Startup.Controllers.ViewModels;

public class AuthViewModel
{
    public string Login { get; set; }
    public string ReturnUrl { get; set; }
    public List<ExternalIdProviderViewModel> ExternalIdProviders { get; set; } 
}
