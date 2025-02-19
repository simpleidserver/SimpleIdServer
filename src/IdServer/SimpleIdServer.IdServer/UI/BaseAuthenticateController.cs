// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Repositories;
using FormBuilder.Stores;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI;

public class BaseAuthenticateController : BaseController
{
    private readonly IClientRepository _clientRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserSessionResitory _userSessionRepository;
    private readonly IAmrHelper _amrHelper;
    private readonly IBusControl _busControl;
    private readonly IUserTransformer _userTransformer;
    private readonly IDataProtector _dataProtector;
    private readonly IAuthenticationHelper _authenticationHelper;
    private readonly IdServerHostOptions _options;

    public BaseAuthenticateController(
        IClientRepository clientRepository,
        IUserRepository userRepository,
        IUserSessionResitory userSessionResitory,
        IAmrHelper amrHelper,
        IBusControl busControl,
        IUserTransformer userTransformer,
        IDataProtectionProvider dataProtectionProvider,
        IAuthenticationHelper authenticationHelper,
        ITransactionBuilder transactionBuilder,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder,
        IWorkflowStore workflowStore,
        IFormStore formStore,
        IAcrHelper acrHelper,
        IOptions<IdServerHostOptions> options) : base(tokenRepository, jwtBuilder)
    {
        _clientRepository = clientRepository;
        _userRepository = userRepository;
        _userSessionRepository = userSessionResitory;
        _amrHelper = amrHelper;
        _busControl = busControl;
        _userTransformer = userTransformer;
        _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
        _authenticationHelper = authenticationHelper;
        TransactionBuilder = transactionBuilder;
        WorkflowStore = workflowStore;
        FormStore = formStore;
        AcrHelper = acrHelper;
        _options = options.Value;
    }

    protected IUserRepository UserRepository => _userRepository;
    protected IClientRepository ClientRepository => _clientRepository;
    protected IdServerHostOptions Options => _options;
    protected IAmrHelper AmrHelper => _amrHelper;
    protected IBusControl Bus => _busControl;
    protected IAuthenticationHelper AuthenticationHelper => _authenticationHelper;
    protected ITransactionBuilder TransactionBuilder { get; }
    protected IWorkflowStore WorkflowStore { get; }
    protected IFormStore FormStore { get; }
    protected IAcrHelper AcrHelper { get; }
    protected IRealmStore RealmStore { get; }

    protected JsonObject ExtractQuery(string returnUrl) => ExtractQueryFromUnprotectedUrl(Unprotect(returnUrl));

    protected JsonObject ExtractQueryFromUnprotectedUrl(string returnUrl)
    {
        var query = returnUrl.GetQueries().ToJsonObject();
        if (query.ContainsKey("returnUrl"))
            return ExtractQuery(query["returnUrl"].GetValue<string>());

        return query;
    }

    protected string Unprotect(string returnUrl)
    {
        var unprotectedUrl = _dataProtector.Unprotect(returnUrl);
        return unprotectedUrl;
    }

    protected bool IsProtected(string returnUrl)
    {
        try
        {
            _dataProtector.Unprotect(returnUrl);
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected async Task<IActionResult> Authenticate(string realm, string returnUrl, string currentAmr, User user, CancellationToken cancellationToken, bool? rememberLogin = null)
    {
        string nextAmr = null;
        Client client = null;
        var unprotectedUrl = returnUrl;
        List<string> amrs = null;
        string currentAcr = null;
        if (!IsProtected(returnUrl))
        {
            var result = await GetNextAmrFromNormalAuthentication(realm, currentAmr, cancellationToken);
            nextAmr = result.nextAmr;
            amrs = result.amrs;
        }
        else
        {
            unprotectedUrl = Unprotect(returnUrl);
            var result = await GetNextAmrFormAuthorizationRequestAuthentication(realm, currentAmr, unprotectedUrl, cancellationToken);
            nextAmr = result.nextAmr;
            client = result.client;
            amrs = result.acr.AllAmrs;
            currentAcr = result.acr.Acr.Name;
        }

        if (WorkflowHelper.IsLastStep(nextAmr))
        {
            if(rememberLogin == null)
            {
                if (HttpContext.Request.Cookies.ContainsKey(Constants.DefaultRememberMeCookieName))
                    rememberLogin = bool.Parse(HttpContext.Request.Cookies[Constants.DefaultRememberMeCookieName]);
                else
                    rememberLogin = false;
            }

            return await Sign(realm, amrs, unprotectedUrl, currentAmr, user, client, cancellationToken, rememberLogin.Value);
        }

        using (var transaction = TransactionBuilder.Build())
        {
            if (rememberLogin != null)
                HttpContext.Response.Cookies.Append(Constants.DefaultRememberMeCookieName, rememberLogin.Value.ToString());

            var login = _authenticationHelper.GetLogin(user);
            await AcrHelper.StoreAcr(new AcrAuthInfo(user.Id, login, user.Email, currentAcr, nextAmr, user.OAuthUserClaims.Select(c => new KeyValuePair<string, string>(c.Name, c.Value)).ToList(), rememberLogin ?? false), cancellationToken);
            _userRepository.Update(user);
            await transaction.Commit(cancellationToken);
            return RedirectToAction("Index", "Authenticate", new { area = nextAmr, ReturnUrl = returnUrl });
        }
    }

    protected async Task<IActionResult> Sign(string realm, List<string> amrs, string returnUrl, string currentAmr, User user, Client client, CancellationToken token, bool rememberLogin = false)
    {
        var expirationTimeInSeconds = GetCookieExpirationTimeInSeconds(client);
        var claims = _userTransformer.Transform(user);
        await AddSession(realm, claims, user, client, token, rememberLogin);
        var offset = DateTimeOffset.UtcNow.AddSeconds(expirationTimeInSeconds);
        if(amrs != null)
            claims.Add(new Claim(Constants.UserClaims.Amrs, string.Join(" ", amrs)));
        var claimsIdentity = new ClaimsIdentity(claims, currentAmr);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        if (rememberLogin)
        {
            await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = offset
            });
        }
        else
        {
            await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = offset
            });
        }

        await _busControl.Publish(new UserLoginSuccessEvent
        {
            Realm = realm,
            UserName = user.Name,
            Amr = currentAmr
        }, token);
        return Redirect(returnUrl);
    }

    protected async Task AddSession(string realm, ICollection<Claim> claims, User user, Client client, CancellationToken cancellationToken, bool rememberLogin = false)
    {
        using(var transaction = TransactionBuilder.Build())
        {
            var currentDateTime = DateTime.UtcNow;
            var expirationTimeInSeconds = GetCookieExpirationTimeInSeconds(client);
            var expirationDateTime = currentDateTime.AddSeconds(expirationTimeInSeconds);
            var session = new UserSession
            {
                SessionId = Guid.NewGuid().ToString(),
                AuthenticationDateTime = DateTime.UtcNow,
                ExpirationDateTime = expirationDateTime,
                State = UserSessionStates.Active,
                Realm = realm,
                UserId = user.Id,
                ClientIds = new List<string> { }
            };
            _userSessionRepository.Add(session);
            _userRepository.Update(user);
            await transaction.Commit(cancellationToken);
            var cookieOptions = new CookieOptions
            {
                Secure = true,
                HttpOnly = false,
                SameSite = SameSiteMode.None
            };
            if (rememberLogin)
            {
                cookieOptions.MaxAge = TimeSpan.FromSeconds(expirationTimeInSeconds);
            }

            var sub = claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Response.Cookies.Append(_options.GetSessionCookieName(realm, sub), session.SessionId, cookieOptions);
        }
    }

    private async Task<(string nextAmr, List<string> amrs)> GetNextAmrFromNormalAuthentication(string realm, string currentAmr, CancellationToken cancellationToken)
    {
        var workflow = await WorkflowStore.Get(realm, Options.DefaultAuthenticationWorkflowId, cancellationToken);
        var forms = await FormStore.GetLatestPublishedVersionByCategory(realm, FormCategories.Authentication, cancellationToken);
        var nextAmr = WorkflowHelper.GetNextAmr(workflow, forms, currentAmr);
        var amrs = WorkflowHelper.ExtractAmrs(workflow, forms);
        return (nextAmr, amrs);
    }

    private async Task<(string nextAmr, Client client, AcrResult acr)> GetNextAmrFormAuthorizationRequestAuthentication(string realm, string currentAmr, string unprotectedUrl, CancellationToken cancellationToken)
    {
        var query = ExtractQueryFromUnprotectedUrl(unprotectedUrl);
        var acrValues = query.GetAcrValuesFromAuthorizationRequest();
        var clientId = query.GetClientIdFromAuthorizationRequest();
        var requestedClaims = query.GetClaimsFromAuthorizationRequest();
        var client = await _clientRepository.GetByClientId(realm, clientId, cancellationToken);
        var acr = await _amrHelper.FetchDefaultAcr(realm, FormCategories.Authentication, acrValues, requestedClaims, client, cancellationToken);
        var nextAmr = acr == null ? null : WorkflowHelper.GetNextAmr(acr.Workflow, acr.Forms, currentAmr);
        return (nextAmr, client, acr);
    }

    private double GetCookieExpirationTimeInSeconds(Client client)
    {
        var expirationTimeInSeconds = client == null || client.TokenExpirationTimeInSeconds == null ?
           _options.DefaultTokenExpirationTimeInSeconds : client.TokenExpirationTimeInSeconds.Value;
        return expirationTimeInSeconds;
    }
}
