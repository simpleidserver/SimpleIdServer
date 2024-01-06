// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Options;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token;

[AllowAnonymous]
public class TokenController : Controller
{
    private readonly ITokenRequestHandler _tokenRequestHandler;
    private readonly IRevokeTokenRequestHandler _revokeTokenRequestHandler;
    private readonly IdServerHostOptions _options;

    public TokenController(ITokenRequestHandler tokenRequestHandler, IRevokeTokenRequestHandler revokeTokenRequestHandler, IOptions<IdServerHostOptions> options)
    {
        _tokenRequestHandler = tokenRequestHandler;
        _revokeTokenRequestHandler = revokeTokenRequestHandler;
        _options = options.Value;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromRoute] string prefix, [FromForm] TokenRequest request, CancellationToken token)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var clientCertificate = await Request.HttpContext.Connection.GetClientCertificateAsync();
        var claimName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        var userSubject = claimName == null ? string.Empty : claimName.Value;
        var jObjHeader = Request.Headers.ToJsonObject();
        var jObjBody = Request.Form.ToJsonObject();
        var context = new HandlerContext(
            new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), userSubject, jObjBody, jObjHeader, Request.Cookies, clientCertificate, HttpContext.Request.Method), 
            prefix,
            _options,
            new HandlerContextResponse(HttpContext.Response));
        var result = await _tokenRequestHandler.Handle(context, token);

        // rfc6749 : the authorization server must include the HTTP "Cache-Control" response header field with a value of "no-store"
        // in any response containing tokens, credentials, or sensitive information.
        Response.SetNoCache();
        return result;
    }

    [HttpPost]
    public async Task<IActionResult> Revoke([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        try
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var clientCertificate = await Request.HttpContext.Connection.GetClientCertificateAsync();
            var jObjHeader = Request.Headers.ToJsonObject();
            var jObjBody = Request.Form.ToJsonObject();
            await _revokeTokenRequestHandler.Handle(prefix, jObjHeader, jObjBody, clientCertificate, Request.GetAbsoluteUriWithVirtualPath(), cancellationToken);
            return new OkResult();
        }
        catch (OAuthException ex)
        {
            var jObj = new JsonObject
            {
                [ErrorResponseParameters.Error] = ex.Code,
                [ErrorResponseParameters.ErrorDescription] = ex.Message
            };
            return new BadRequestObjectResult(jObj);
        }
    }
}

public class TokenRequest
{
    [FromForm(Name = TokenRequestParameters.ClientId)]
    public string ClientId { get; set; }
    [FromForm(Name = TokenRequestParameters.ClientSecret)]
    public string ClientSecret { get; set; }
    [FromForm(Name = TokenRequestParameters.GrantType)]
    public GrantTypes GrantType { get; set; }
    [FromForm(Name = TokenRequestParameters.Scope)]
    public string Scope { get; set; }
    [FromForm(Name = TokenRequestParameters.Username)]
    public string Username { get; set; }
    [FromForm(Name = TokenRequestParameters.Password)]
    public string Password { get; set; }
    [FromForm(Name = TokenRequestParameters.Code)]
    public string Code { get; set; }
    [FromForm(Name = TokenRequestParameters.RedirectUri)]
    public string RedirectUri { get; set; }
}

public enum GrantTypes
{
    [EnumMember(Value = AuthorizationCodeHandler.GRANT_TYPE)]
    AuthorizationCode = 0,
    [EnumMember(Value = CIBAHandler.GRANT_TYPE)]
    CIBA = 1,
    [EnumMember(Value = ClientCredentialsHandler.GRANT_TYPE)]
    ClientCredentials = 2,
    [EnumMember(Value = DeviceCodeHandler.GRANT_TYPE)]
    DeviceCode = 3,
    [EnumMember(Value = PasswordHandler.GRANT_TYPE)]
    Password = 4,
    [EnumMember(Value = PreAuthorizedCodeHandler.GRANT_TYPE)]
    PreAuthorizedCode = 5,
    [EnumMember(Value = RefreshTokenHandler.GRANT_TYPE)]
    RefreshToken = 6,
    [EnumMember(Value = TokenExchangeHandler.GRANT_TYPE)]
    TokenExchanged = 7,
    [EnumMember(Value = UmaTicketHandler.GRANT_TYPE)]
    UmaTicket = 8
}