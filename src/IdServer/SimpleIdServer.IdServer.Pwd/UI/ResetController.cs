// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Pwd.UI.ViewModels;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.Services;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Pwd.UI;

[Area(Constants.Areas.Password)]
public class ResetController : BaseController
{
    private readonly IEnumerable<IResetPasswordService> _resetPasswordServices;
    private readonly IAuthenticationHelper _authenticationHelper;
    private readonly IConfiguration _configuration;
    private readonly IGrantedTokenHelper _grantedTokenHelper;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ResetController> _logger;

    public ResetController(
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder,
        IEnumerable<IResetPasswordService> resetPasswordServices,
        IAuthenticationHelper authenticationHelper,
        IConfiguration configuration,
        IGrantedTokenHelper grantedTokenHelper,
        IUserRepository userRepository,
        ILogger<ResetController> logger) : base(tokenRepository, jwtBuilder)
    {
        _resetPasswordServices = resetPasswordServices;
        _authenticationHelper = authenticationHelper;
        _configuration = configuration;
        _grantedTokenHelper = grantedTokenHelper;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix, string returnUrl, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        string login = null;
        var notificationMode = GetOptions().NotificationMode;
        var service = _resetPasswordServices.Single(p => p.NotificationMode == notificationMode);
        if (User.Identity.IsAuthenticated)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _authenticationHelper.GetUserByLogin(nameIdentifier, prefix, cancellationToken);
            login = _authenticationHelper.GetLogin(user);
        }
        
        var viewModel = new ResetPasswordViewModel
        {
            Login = login,
            NotificationMode = notificationMode,
            Value = null,
            ReturnUrl = returnUrl
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromRoute] string prefix, ResetPasswordViewModel viewModel, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        viewModel.Validate(ModelState);
        if(!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var user = await GetUser();
        if(user == null)
        {
            ModelState.AddModelError("unknown_user", "unknown_user");
            return View(viewModel);
        }

        var options = GetOptions();
        var notificationMode = options.NotificationMode;
        var service = _resetPasswordServices.Single(p => p.NotificationMode == notificationMode);
        var destination = service.GetDestination(user);
        if(string.IsNullOrWhiteSpace(destination))
        {
            ModelState.AddModelError("missing_destination", "missing_destination");
            return View(viewModel);
        }

        if(viewModel.Value != destination)
        {
            ModelState.AddModelError("invalid_destination", "invalid_destination");
            return View(viewModel);
        }

        var url = Url.Action("Confirm", "Reset", new
        {
            area = Constants.Areas.Password
        });
        var issuer = Request.GetAbsoluteUriWithVirtualPath();
        var parameter = new ResetPasswordParameter(
            $"{issuer}{url}", 
            user, 
            prefix, 
            options.ResetPasswordBody, 
            options.ResetPasswordTitle, 
            options.ResetPasswordLinkExpirationInSeconds,
            viewModel.ReturnUrl);
        try
        {
            await service.SendResetLink(parameter, cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            ModelState.AddModelError("cannot_send_otpcode", "cannot_send_otpcode");
            return View(viewModel);
        }

        viewModel.IsResetLinkedSent = true;
        return View(viewModel);

        async Task<User> GetUser()
        {
            var login = viewModel.Login;
            if(User.Identity.IsAuthenticated)
            {
                login = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            }

            var user = await _authenticationHelper.GetUserByLogin(login, prefix, cancellationToken);
            return user;
        }
    }

    [HttpGet]
    public async Task<IActionResult> Confirm([FromRoute] string prefix, string code, string returnUrl, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var viewModel = new ConfirmResetPasswordViewModel();
        var resetPasswordLink = await _grantedTokenHelper.GetResetPasswordLink(code.ToString(), cancellationToken);
        if(resetPasswordLink == null)
        {
            ModelState.AddModelError("invalid_link", "invalid_link");
            return View(viewModel);
        }

        var user = await _authenticationHelper.GetUserByLogin(resetPasswordLink.Login, prefix, cancellationToken);
        var notificationMode = GetOptions().NotificationMode;
        var service = _resetPasswordServices.Single(p => p.NotificationMode == notificationMode);
        var destination = service.GetDestination(user);
        viewModel.Destination = destination;
        viewModel.Code = code;
        viewModel.ReturnUrl = returnUrl;
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm([FromRoute] string prefix, ConfirmResetPasswordViewModel viewModel, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        viewModel.Validate(ModelState);
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var notificationMode = GetOptions().NotificationMode;
        var service = _resetPasswordServices.Single(p => p.NotificationMode == notificationMode);
        var resetPasswordLink = await service.Verify(viewModel.Code, cancellationToken);
        if(resetPasswordLink == null)
        {
            ModelState.AddModelError("invalid_otpcode", "invalid_otpcode");
            return View(viewModel);
        }

        var user = await _authenticationHelper.GetUserByLogin(resetPasswordLink.Login, prefix, cancellationToken);
        var credential = user.Credentials.SingleOrDefault(c => c.CredentialType == Constants.Areas.Password && c.IsActive);
        if(credential == null)
        {
            credential = new UserCredential
            {
                Id = Guid.NewGuid().ToString(),
                CredentialType = Constants.Areas.Password,
                IsActive = true
            };
            user.Credentials.Add(credential);
        }

        credential.Value = PasswordHelper.ComputeHash(viewModel.Password);
        await _userRepository.SaveChanges(cancellationToken);
        viewModel.IsPasswordUpdated = true;
        return View(viewModel);
    }

    private IdServerPasswordOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(IdServerPasswordOptions).Name);
        return section.Get<IdServerPasswordOptions>();
    }
}
