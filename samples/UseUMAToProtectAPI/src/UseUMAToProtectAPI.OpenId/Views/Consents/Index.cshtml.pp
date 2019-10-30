@using $rootnamespace$.Resources
@model SimpleIdServer.OpenID.UI.ViewModels.ConsentsIndexViewModel

@{
    Layout = "~/Views/Shared/_Layout.cshtml";
	ViewBag.Title = Global.consents;
}

@using (Html.BeginForm("Index", "Consents", FormMethod.Post))
{
    @Html.AntiForgeryToken()
    @if (!ViewData.ModelState.IsValid)
    {
        <div class="alert alert-danger">
            <ul class="list-group">
                @foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        <li class="list-group-item list-group-item-danger">@Global.ResourceManager.GetString(error.ErrorMessage)</li>
                    }
                }
            </ul>
        </div>
    }
    <input type="hidden" name="ReturnUrl" value="@Model.ReturnUrl" />
    <div class="card">
        <div class="card-header">
            @string.Format(Global.consent_client_access, Model.ClientName)
        </div>
        <div class="card-body">
            <h5 class="card-title">@Global.scopes</h5>
            <ul class="list-group">
                @foreach (var scopeName in Model.ScopeNames)
                {
                    <li class="list-group-item">@Global.ResourceManager.GetString($"scope_{scopeName}")</li>
                }
            </ul>
            <h5 class="card-title">@Global.claims</h5>
            <ul class="list-group">
                @foreach (var claim in Model.ClaimNames)
                {
                    <li class="list-group-item">@claim</li>
                }
            </ul>
        </div>
        <div class="card-footer">
            <button type="submit" class="btn btn-success card-link">@Global.confirm</button>
            <a href="@Model.CancellationUrl" class="btn btn-danger">@Global.reject</a>
        </div>
    </div>
}