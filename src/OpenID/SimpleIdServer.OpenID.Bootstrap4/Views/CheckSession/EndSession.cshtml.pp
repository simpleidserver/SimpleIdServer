@model SimpleIdServer.OpenID.UI.ViewModels.RevokeSessionViewModel
@using $rootnamespace$.Resources

@{
    Layout = "~/Views/Shared/_Layout.cshtml";
    ViewBag.Title = Global.revoke_session_title;
}

<a href="@Model.RevokeSessionCallbackUrl" class="btn btn-danger">@Global.revoke_session_title</a>