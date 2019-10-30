@model SimpleIdServer.Uma.UI.ViewModels.ResourcesEditViewModel
@using Microsoft.AspNetCore.Localization
@using Microsoft.AspNetCore.Mvc.Localization
@inject IViewLocalizer Localizer

@{
    Layout = "~/Views/Shared/_Layout.cshtml";
    ViewData["Title"] = Localizer["edit_resource_title"];
    var requestCulture = Context.Features.Get<IRequestCultureFeature>();
}

<div class="container">
    <div class="spinner-border" id="loading">
        <span class="sr-only">@Localizer["loading"]</span>
    </div>
    <div id="error-message" class="alert alert-danger" style="display: none;">
        @Localizer["resource_unknown"]
    </div>
    <div id="content" style="display: none;">
        <h1>Resource</h1>
        <div class="from-group">
            <label>@Localizer["identifier"]</label>
            <input type="text" class="form-control" name="_id" disabled />
        </div>
        <div class="form-group">
            <label>@Localizer["name"]</label>
            <input type="text" class="form-control" name="name" disabled />
        </div>
        <div class="from-group">
            <label>@Localizer["description"]</label>
            <input type="text" class="form-control" name="description" disabled />
        </div>
        <div class="from-group">
            <label>@Localizer["scopes"]</label>
            <input type="text" class="form-control" name="resource_scopes" disabled />
        </div>
        <div class="from-group">
            <label>@Localizer["type"]</label>
            <input type="text" class="form-control" name="type" disabled />
        </div>
    </div>
</div>

@section Scripts {
    <script type="text/javascript">
        let currentCultureKey = "@requestCulture.RequestCulture.UICulture.Name";
        $.ajax({
            "url": "@Url.Action("GetMe", "ResourcesAPI", new { id = Model.ResourceId })",
            "type": "GET",
            "beforeSend": function (xhr) {
                xhr.setRequestHeader("Authorization", "@Model.IdToken");
            },
            "success": function (jObj) {
                let name = "", description = "";
                for (var jObjKey in jObj) {
                    if (jObjKey.startsWith('name') || jObjKey.startsWith('description')) {
                        let splittedKey = jObjKey.split('#');
                        if (splittedKey[1] === currentCultureKey) {
                            if (splittedKey[0] === 'name') {
                                name = jObj[jObjKey];
                            }
                            else if (splittedKey[0] === 'description') {
                                description = jObj[jObjKey];
                            }
                        }
                    }
                }

                $("#content input[name='_id']").val(jObj['_id']);
                $("#content input[name='name']").val(name);
                $("#content input[name='description']").val(description);
                $("#content input[name='resource_scopes']").val(jObj['resource_scopes'].join());
                $("#content input[name='type']").val(jObj['type']);
                $("#content").show();
                $("#loading").hide();
            },
            "error": function () {
                $("#error-message").show();
                $("#loading").hide();
            }
        });
    </script>
}