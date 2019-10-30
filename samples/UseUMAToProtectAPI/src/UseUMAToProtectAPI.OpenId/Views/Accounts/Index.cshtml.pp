@using $rootnamespace$.Resources
@model SimpleIdServer.OpenID.UI.ViewModels.AccountsIndexViewModel
@{
    Layout = "~/Views/Shared/_Layout.cshtml";
 ViewBag.Title = Global.select_session;
}

<h1>@Global.active_sessions</h1>

<div class="row">
    @foreach (var account in Model.Accounts)
    {
        <div class="col-md-4">
            @using (Html.BeginForm("Index", "Accounts", FormMethod.Post))
            {
                @Html.AntiForgeryToken()
                <div class="card">
                    <input type="hidden" name="AccountName" value="@account.Name" />
                    <input type="hidden" name="ReturnUrl" value="@Model.ReturnUrl" />
                    <div class="card-header">@account.Name</div>
                    <div class="card-body">
                        @if (account.IssuedUtc != null)
                        {
                            <p>
                                @string.Format(Global.authentication_time, account.IssuedUtc.Value.ToString("s"))
                            </p>
                        }
                        @if (account.ExpiresUct != null)
                        {
                            <p>
                                @string.Format(Global.expiration_time, account.ExpiresUct.Value.ToString("s"))
                            </p>
                        }
                    </div>
                    <div class="card-footer">
                        <input type="submit" class="btn btn-primary" value="@Global.choose_session" />
                    </div>
                </div>
            }
        </div>
    }
</div>