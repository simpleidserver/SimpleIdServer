@using Microsoft.AspNetCore.Mvc.Localization
@inject IViewLocalizer Localizer

@{
    Layout = "~/Views/Shared/_Layout.cshtml";
    ViewBag.Title = Localizer["home_page_title"];
}

<h1>Welcome to UMA2.0 server</h1>