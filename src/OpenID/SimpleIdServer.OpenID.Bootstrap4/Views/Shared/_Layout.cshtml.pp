@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@using $rootnamespace$.Resources
@using Microsoft.Extensions.Options
@using SimpleIdServer.OAuth.Options 
@using System.Globalization

@inject IOptions<OAuthHostOptions> Options

@{
    var returnUrl = string.IsNullOrEmpty(Context.Request.Path) ? "~/" : $"~{Context.Request.Path.Value}{Context.Request.QueryString}";
    var currentCultureInfo = CultureInfo.DefaultThreadCurrentUICulture;
    string languagesLabel = Global.ResourceManager.GetString("languages");
    if (currentCultureInfo != null && !string.IsNullOrWhiteSpace(currentCultureInfo.Name))
    {
        var str = Global.ResourceManager.GetString(currentCultureInfo.Name);
        if (!string.IsNullOrWhiteSpace(str))
        {
            languagesLabel = string.Format(Global.ResourceManager.GetString("selected_language"), str);
        }
    }
}

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>SimpleIdServer - @ViewBag.Title</title>
    <link rel="stylesheet" href="@Url.Content("~/lib/twitter-bootstrap/css/bootstrap.css")" />
</head>
<body>
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
        <a class="navbar-brand" href="#">SimpleIdServer</a>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarSupportedContent">
            <ul class="navbar-nav mr-auto">
                @if (User.Identity.IsAuthenticated)
                {
                    <li class="nav-item">
                        <a class="nav-link">
                            Welcome @User.Identity.Name
                        </a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="@Url.Action("Disconnect", "Home", new { area = "" })">@Global.disconnect</a>
                    </li>
                }
            </ul>
            <ul class="navbar-nav">
                <li class="nav-item dropdown">
                    <a class="nav-link dropdown-toggle" href="#" id="navbarDropdown" data-toggle="dropdown">
                        @languagesLabel
                    </a>
                    <div class="dropdown-menu">
                        @foreach (var uiCulture in Options.Value.SupportedUICultures)
                        {
                            <form asp-controller="Home" asp-action="SwitchLanguage" asp-area="" method="post">
                                <input type="hidden" name="culture" value="@uiCulture.Name" />
                                <input type="hidden" name="returnUrl" value="@returnUrl" />
                                <button type="submit" class="dropdown-item" href="#">@Global.ResourceManager.GetString(uiCulture.Name)</button>
                            </form>
                        }
                    </div>
                </li>
            </ul>
        </div>
    </nav>
    <div class="container">
        <div>
            @RenderSection("PageTitle", false)
        </div>
        <div>
            @RenderBody()
        </div>
    </div>
    <script type="text/javascript" src="@Url.Content("~/lib/jquery/jquery.js")"></script>
    <script type="text/javascript" src="@Url.Content("~/lib/twitter-bootstrap/js/bootstrap.js")"></script>
    @RenderSection("Scripts", required: false)
</body>
</html>