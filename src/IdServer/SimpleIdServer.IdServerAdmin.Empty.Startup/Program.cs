using SimpleIdServer.IdServerAdmin.Empty.Startup.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddRazorComponents()
//     .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddIdserverAdmin("https://localhost:5001")
    .EnableRealm();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseIdserverAdmin();
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapFallbackToPage("/App");
// app.MapRazorComponents<App>()
//     .AddInteractiveServerRenderMode();
app.Run();
