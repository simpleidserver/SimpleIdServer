@using $rootnamespace$.Resources
@model IEnumerable<SimpleIdServer.OpenID.UI.ViewModels.ConsentViewModel>

@{
    Layout = "~/Views/Shared/_Layout.cshtml";
    ViewBag.Title = Global.consents;
}

<h1>@Global.manage_consents</h1>

@foreach (var consent in Model)
{
    <div class="card">
        <div class="card-header">
            @string.Format(Global.consent_client_access, consent.ClientName)
        </div>
        <div class="card-body">
            <h5 class="card-title">@Global.scopes</h5>
            <ul class="list-group">
                @foreach (var scopeName in consent.ScopeNames)
                {
                    <li class="list-group-item">@Global.ResourceManager.GetString($"scope_{scopeName}")</li>
                }
            </ul>
            <h5 class="card-title">@Global.claims</h5>
            <ul class="list-group">
                @foreach (var claim in consent.ClaimNames)
                {
                    <li class="list-group-item">@claim</li>
                }
            </ul>
        </div>
        <div class="card-footer">
            <a href="@Url.Action("RejectConsent", "Consents", new { consentId = consent.ConsentId })" class="btn btn-danger">@Global.reject</a>
        </div>
    </div>
}