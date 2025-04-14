using DataSeeder;
using FormBuilder.Startup.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
const string cookieName = "XSFR-TOKEN";
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataseeder();
builder.Services.AddTransient<IDataSeederExecutionHistoryRepository, DefaultDataSeederExecutionHistoryRepository>();
builder.Services.AddAntiforgery(c =>
{
    c.Cookie.Name = cookieName;
});
FormBuilderSetup.ConfigureFormBuilder(builder, cookieName);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

FormBuilder.Startup.Config.DataSeeder.SeedData(app);
app.Services.SeedData();
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