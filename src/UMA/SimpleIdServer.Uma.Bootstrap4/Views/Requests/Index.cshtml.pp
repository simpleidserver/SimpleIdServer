@model SimpleIdServer.Uma.UI.ViewModels.RequestsIndexViewModel
@using Microsoft.AspNetCore.Localization
@using Microsoft.AspNetCore.Mvc.Localization
@inject IViewLocalizer Localizer

@{
    Layout = "~/Views/Shared/_Layout.cshtml";
    ViewBag.Title = Localizer["my_requests_title"];
    var requestCulture = Context.Features.Get<IRequestCultureFeature>();
}

<div class="container">
    <h1>@Localizer["my_requests_title"]</h1>
    <table class="table table-striped table-bordered" id="my-requests">
        <thead>
            <tr>
                <th>@Localizer["requester"]</th>
                <th>@Localizer["resource"]</th>
                <th>@Localizer["scopes"]</th>
                <th>@Localizer["create_datetime"]</th>
                <th>@Localizer["status"]</th>
            </tr>
        </thead>
        <tbody></tbody>
    </table>
</div>

@section Scripts {
    <script type="text/javascript">
        $(document).ready(function() {
            let currentCultureKey = "@requestCulture.RequestCulture.UICulture.Name";
            $("#my-requests").DataTable({
                "processing": true,
                "serverSide": true,
                "searching": false,
                "ajax": {
                    "url": "@Url.Action("SearchRequests", "RequestsAPI")",
                    "type": "GET",
                    "data": function (d) {
                        let orderColumnSort = d.order[0].dir;
                        let orderColumnIndex = d.order[0].column;
                        let orderColumnName = d.columns[orderColumnIndex].name;
                        var query = "startIndex=" + d.start + "&count=" + d.length + "&sort=" + orderColumnName + "&order=" + orderColumnSort;
                        return query;
                    },
                    "beforeSend": function (xhr) {
                        xhr.setRequestHeader("Authorization", "@Model.IdToken");
                    },
                    "dataFilter": function (inData) {
                        let json = JSON.parse(inData);
                        let newData = [];
                        json.data.forEach(function (jObj) {
                            var newObj = [];
                            let resource = jObj["resource"];
                            let name = "";
                            for (let resourceKey in resource) {
                                if (resourceKey.startsWith('name')) {
                                    let splittedKey = resourceKey.split('#');
                                    if (splittedKey[1] === currentCultureKey) {
                                        if (splittedKey[0] === 'name') {
                                            name = jObj[resourceKey];
                                        }
                                    }
                                }
                            }

                            newObj.push(jObj["requester"]);
                            newObj.push(name);
                            newObj.push(jObj["scopes"].join());
                            newObj.push(jObj["create_datetime"]);
                            newObj.push(jObj["status"]);
                            newData.push(newObj);
                        });
                        var result = {
                            data: newData,
                            recordsTotal: json.totalResults,
                            recordsFiltered: json.count
                        };
                        return JSON.stringify(result);
                    }
                },
                "columnDefs": [
                    { "name": "Requester", targets: 0, orderable: false },
                    { "name": "Resource", targets: 1, orderable: false },
                    { "name": "Scopes", targets: 2, orderable: false },
                    { "name": "CreateDateTime", targets: 3 },
                    {
                        "name": "Status", targets: 4, render: function (data) {
                        if (data === 'tobeconfirmed') {
                            return "<span class='badge badge-primary'>@Localizer["to_be_confirmed"]</span>";
                        }

                        if (data === 'confirmed') {
                            return "<span class='badge badge-success'>@Localizer["confirmed"]</span>";
                        }

                        return "<span class='badge badge-success'>@Localizer["rejected"]</span>";

                    }}
                ],
                "order": [[3, "desc"]]
            });

        })
    </script>
}