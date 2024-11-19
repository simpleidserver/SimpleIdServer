using FormBuilder.Models;
using Microsoft.AspNetCore.Antiforgery;
using System.Text.Json.Nodes;

namespace FormBuilder.Startup.Controllers.ViewModels;

public class IndexAuthViewModel
{
    public FormRecord Form { get; set; }
    public JsonObject Input { get; set; }
    public AntiforgeryTokenRecord AntiforgeryToken { get; set; }
}
