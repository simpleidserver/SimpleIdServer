@using $rootnamespace$.Resources

@{
    Layout = "~/Views/Shared/_Layout.cshtml";
    ViewBag.Title = "Error";
}

@model SimpleIdServer.OpenID.UI.ViewModels.ErrorViewModel

<div class="alert alert-danger" role="alert">
    @Global.ResourceManager.GetString(Model.Code)
</div>