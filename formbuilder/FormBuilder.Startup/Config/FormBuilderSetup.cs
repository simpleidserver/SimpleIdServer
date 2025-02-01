using FormBuilder.Startup.Fakers;
using FormBuilder.Startup.Workflows;

namespace FormBuilder.Startup.Config;

public class FormBuilderSetup
{
    public static void ConfigureFormBuilder(WebApplicationBuilder builder, string cookieName)
    {
        builder.Services.AddFormBuilder().UseEF();
        builder.Services.AddTransient<IFakerDataService, PwdAuthFakerDataService>();
        builder.Services.AddTransient<IFakerDataService, MobileAuthFakerDataService>();
        builder.Services.AddTransient<IWorkflowLayoutService, MobileAuthWorkflowLayout>();
        builder.Services.AddTransient<IWorkflowLayoutService, PwdAuthWorkflowLayout>();
        builder.Services.Configure<FormBuilderStartupOptions>(cb => cb.AntiforgeryCookieName = cookieName);
    }
}
