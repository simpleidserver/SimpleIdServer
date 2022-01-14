@using $rootnamespace$.Resources
@model SimpleIdServer.OpenID.UI.ViewModels.ConsentsIndexViewModel

@{
    ViewBag.Title = OpenIdGlobal.consents;
    Layout = "~/Views/Shared/_OpenIdLayout.cshtml";
}

<form method="post" action="@Url.Action("Reject", "Consents")" id="rejectForm">
    @Html.AntiForgeryToken()
    <input name="ReturnUrl" type="hidden" value="@Model.ReturnUrl" />
</form>

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
                        <li class="list-group-item list-group-item-danger">@OpenIdGlobal.ResourceManager.GetString(error.ErrorMessage)</li>
                    }
                }
            </ul>
        </div>
    }
    <input type="hidden" name="ReturnUrl" value="@Model.ReturnUrl" />
    <div class="card">
        <div class="card-header">
            @string.Format(OpenIdGlobal.consent_client_access, Model.ClientName)
        </div>
        <div class="card-body">
            <h5 class="card-title">@OpenIdGlobal.scopes</h5>
            <ul class="list-group">
                @foreach (var scopeName in Model.ScopeNames)
                {
                    <li class="list-group-item">@OpenIdGlobal.ResourceManager.GetString($"scope_{scopeName}")</li>
                }
            </ul>
            <h5 class="card-title">@OpenIdGlobal.claims</h5>
            <ul class="list-group">
                @foreach (var claim in Model.ClaimNames)
                {
                    <li class="list-group-item">@claim</li>
                }
            </ul>
        </div>
        <div class="card-footer">
            <button type="submit" class="btn btn-success card-link">@OpenIdGlobal.confirm</button>
            <button type="submit" form="rejectForm" class="btn btn-danger">@OpenIdGlobal.reject</button>
        </div>
    </div>
}