﻿@using $rootnamespace$.Resources
@model IEnumerable<SimpleIdServer.OpenID.UI.ViewModels.ConsentViewModel>

@{
    ViewBag.Title = OpenIdGlobal.consents;
    Layout = "~/Views/Shared/_OpenIdLayout.cshtml";
}

<h1>@OpenIdGlobal.manage_consents</h1>

@foreach (var consent in Model)
{
    <div class="card">
        <div class="card-header">
            @string.Format(OpenIdGlobal.consent_client_access, consent.ClientName)
        </div>
        <div class="card-body">
            <h5 class="card-title">@OpenIdGlobal.scopes</h5>
            <ul class="list-group">
                @foreach (var scopeName in consent.ScopeNames)
                {
                    <li class="list-group-item">@OpenIdGlobal.ResourceManager.GetString($"scope_{scopeName}")</li>
                }
            </ul>
            <h5 class="card-title">@OpenIdGlobal.claims</h5>
            <ul class="list-group">
                @foreach (var claim in consent.ClaimNames)
                {
                    <li class="list-group-item">@claim</li>
                }
            </ul>
        </div>
        <div class="card-footer">
            <a href="@Url.Action("RejectConsent", "Consents", new { consentId = consent.ConsentId })" class="btn btn-danger">@OpenIdGlobal.reject</a>
        </div>
    </div>
}