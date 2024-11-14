using FormBuilder;
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Divider;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.Password;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddTransient<IApplicationBuilder, ApplicationBuilder>();

builder.Services.AddTransient<IFormElementDefinition, FormInputFieldDefinition>();
builder.Services.AddTransient<IFormElementDefinition, FormPasswordFieldDefinition>();
builder.Services.AddTransient<IFormElementDefinition, FormButtonDefinition>();
builder.Services.AddTransient<IFormElementDefinition, FormStackLayoutDefinition>();
builder.Services.AddTransient<IFormElementDefinition, FormCheckboxDefinition>();
builder.Services.AddTransient<IFormElementDefinition, DividerLayoutDefinition>();
builder.Services.AddTransient<IFormElementDefinition, FormAnchorDefinition>();

builder.Services.AddTransient<ITranslationHelper, TranslationHelper>();
builder.Services.AddTransient<IRenderFormElementsHelper, RenderFormElementsHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseRouting();

app.UseEndpoints(edps =>
{
    edps.MapBlazorHub();
    edps.MapFallbackToPage("/_Host");
    edps.MapControllers();
});

app.Run();
