@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@using $rootnamespace$.Resources

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