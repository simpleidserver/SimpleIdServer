@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@using Microsoft.AspNetCore.Builder
@using Microsoft.AspNetCore.Localization
@using Microsoft.AspNetCore.Mvc.Localization
@using Microsoft.Extensions.Options
@inject IViewLocalizer Localizer
@inject IOptions<RequestLocalizationOptions> LocOptions

@{
    var requestCulture = Context.Features.Get<IRequestCultureFeature>();
    var cultureItems = LocOptions.Value.SupportedUICultures
        .Select(c => new SelectListItem { Value = c.Name, Text = c.DisplayName })
        .ToList();
}

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>SimpleIdServer - @ViewBag.Title</title>
    <link rel="stylesheet" href="@Url.Content("~/lib/twitter-bootstrap/css/bootstrap.css")" />
    <link rel="stylesheet" href="@Url.Content("~/lib/dataTables/css/dataTables.bootstrap4.css")" />
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
                        <a class="nav-link" href="@Url.Action("Index", "Resources")">@Localizer["my_resources"]</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="@Url.Action("Index", "Requests")">@Localizer["my_requests"]</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="@Url.Action("Received", "Requests")">@Localizer["received_requests"]</a>
                    </li>
                }
            </ul>
            <ul class="navbar-nav">
                <li class="nav-item dropdown">
                    <a class="nav-link dropdown-toggle" data-toggle="dropdown">@requestCulture.RequestCulture.UICulture.DisplayName</a>
                    <div class="dropdown-menu">
                        @foreach (var uiCulture in LocOptions.Value.SupportedUICultures)
                        {
                            <a class="dropdown-item" href="@Url.Action("SetLanguage", "Home", new { culture = uiCulture.Name, returnUrl = Context.Request.Path })">@uiCulture.DisplayName</a>
                        }
                    </div>
                </li>
                @if (User.Identity.IsAuthenticated)
                {
                    <li class="nav-item">
                        <a class="nav-link">
                            @Localizer["welcome", @User.Identity.Name]
                        </a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="@Url.Action("Disconnect", "Home")">@Localizer["disconnect"]</a>
                    </li>
                }
                else
                {
                    <li class="nav-item">
                        <a class="nav-link" href="@Url.Action("Login", "Home")">@Localizer["authenticate"]</a>
                    </li>
                }
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
    <script type="text/javascript" src="@Url.Content("~/lib/dataTables/js/jquery.dataTables.js")"></script>
    <script type="text/javascript" src="@Url.Content("~/lib/dataTables/js/dataTables.bootstrap4.js")"></script>
    @RenderSection("Scripts", required: false)
</body>
</html>