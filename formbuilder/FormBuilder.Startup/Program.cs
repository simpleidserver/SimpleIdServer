using FormBuilder;
using FormBuilder.EF;
using FormBuilder.Startup;
using FormBuilder.Startup.Fakers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// TODO: https://learn.microsoft.com/en-us/aspnet/core/blazor/components/integration?view=aspnetcore-9.0

const string cookieName = "XSFR-TOKEN";
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddFormBuilder()
    .UseEF();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IFakerDataService, AuthViewModelFakeService>();
builder.Services.Configure<FormBuilderStartupOptions>(cb => cb.AntiforgeryCookieName = cookieName);
builder.Services.AddAntiforgery(c =>
{
    c.Cookie.Name = cookieName;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

SeedData(app);
app.Use(async (context, next) =>
{
    var cookie = context.Request.Headers;
    await next.Invoke();
});
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseRouting();
app.UseEndpoints(edps =>
{
    edps.MapBlazorHub(); 
    edps.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

void SeedData(WebApplication application)
{
    using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        using (var dbContext = scope.ServiceProvider.GetService<FormBuilderDbContext>())
        {
            dbContext.Forms.AddRange(FormBuilder.Startup.Constants.AllForms);
            dbContext.Workflows.AddRange(FormBuilder.Startup.Constants.AllWorkflows);
            dbContext.SaveChanges();
        }
    }
}