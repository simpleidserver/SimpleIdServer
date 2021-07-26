@using $rootnamespace$.Resources
@model SimpleIdServer.OpenID.UI.ViewModels.ErrorViewModel

@{
    ViewBag.Title = "Error";
}

<div class="alert alert-danger" role="alert">
    @Global.ResourceManager.GetString(Model.Code)
</div>