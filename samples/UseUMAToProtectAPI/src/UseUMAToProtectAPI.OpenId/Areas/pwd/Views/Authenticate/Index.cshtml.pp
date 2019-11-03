@model SimpleIdServer.UI.Authenticate.LoginPassword.ViewModels.AuthenticateViewModel
@using $rootnamespace$.Resources

@{
    ViewBag.Title = Global.authenticate_pwd;
    Layout = "~/Views/Shared/_Layout.cshtml";
}

<div>
    <div class="card" id="loginpwd-card">
        <div class="card-header">@Global.authenticate_pwd</div>
        @using (Html.BeginForm("Index", "Authenticate", new { area = "pwd", returnUrl = Model.ReturnUrl }, FormMethod.Post))
        {
            @Html.AntiForgeryToken()
            <div class="card-body">
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
                <input type="hidden" value="@Model.ReturnUrl" name="ReturnUrl" />
                <div class="form-group">
                    <label>@Global.login</label>
                    <input type="text" value="@Model.Login" name="Login" class="form-control" />
                </div>
                <div class="form-group">
                    <label>@Global.password</label>
                    <input type="password" value="@Model.Password" name="Password" class="form-control" />
                </div>
                <div>
                    <input type="checkbox" value="@Model.RememberLogin" name="RememberLogin" />
                    <label>@Global.remember_login</label>
                </div>
            </div>
            <div class="card-footer">
                <button type="submit" class="btn btn-primary card-link">@Global.submit</button>
            </div>
        }
    </div>
</div>

@section Scripts {
    <script type="text/javascript">
        $(document).ready(function () {
            $("#loginpwd-card form input[name='RememberLogin']").change(function (e) {
                $(this).val($(this).is(':checked'));
            });
        });
    </script>
}