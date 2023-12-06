// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI.Services;

public interface IResetPasswordService
{
    string NotificationMode { get; }
    Task SendResetLink(ResetPasswordParameter parameter, CancellationToken cancellationToken);
    Task<ResetPasswordLink> Verify(long code, CancellationToken cancellationToken);
    string GetDestination(User user);
}

public class ResetPasswordParameter
{
    public ResetPasswordParameter(
        string link,
        User user,
        string realm,
        string body,
        string title,
        double defaultExpirationTime)
    {
        Link = link;
        User = user;
        Realm = realm;
        Body = body;
        Title = title;
        DefaultExpirationTime = defaultExpirationTime;

    }

    public string Link { get; private set; }
    public User User { get; private set; }
    public string Realm { get; private set; }
    public string Body { get; private set; }
    public string Title { get; private set; }
    public double DefaultExpirationTime { get; private set; }
}

public abstract class BaseOTPResetPasswordService : IResetPasswordService
{
    private readonly IEnumerable<IOTPAuthenticator> _otpAuthenticators;
    private readonly IUserNotificationService _notificationService;
    private readonly IGrantedTokenHelper _grantedTokenHelper;
    private readonly IAuthenticationHelper _authenticationHelper;

    public BaseOTPResetPasswordService(
        IConfiguration configuration,
        IEnumerable<IOTPAuthenticator> otpAuthenticators,
        IUserNotificationService notificationService,
        IGrantedTokenHelper grantedTokenHelper,
        IAuthenticationHelper authenticationHelper)
    {
        Configuration =  configuration;
        _otpAuthenticators = otpAuthenticators;
        _notificationService = notificationService;
        _grantedTokenHelper = grantedTokenHelper;
        _authenticationHelper = authenticationHelper;
    }

    protected IConfiguration Configuration { get; private set; }

    public abstract string NotificationMode { get; }

    public async Task SendResetLink(ResetPasswordParameter parameter, CancellationToken cancellationToken)
    {
        var destination = GetDestination(parameter.User);
        var login = _authenticationHelper.GetLogin(parameter.User);
        if (string.IsNullOrWhiteSpace(destination)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_DESTINATION);
        var otpOptions = GetOTPOptions();
        var otpAuthenticator = _otpAuthenticators.First(o => o.Alg == otpOptions.OTPAlg);
        var userCredential = new Domains.UserCredential
        {
            OTPAlg = otpOptions.OTPAlg,
            Value = otpOptions.OTPValue,
            OTPCounter = otpOptions.OTPCounter,
            CredentialType = UserCredential.OTP,
            TOTPStep = otpOptions.TOTPStep,
            HOTPWindow = otpOptions.HOTPWindow,
            IsActive = true
        };
        var otp = otpAuthenticator.GenerateOtp(userCredential);
        var link = parameter.Link;
        link = $"{link}?code={otp}";
        var message = string.Format(parameter.Body, link);
        await _notificationService.Send(parameter.Title,
            message,
            new Dictionary<string, string>(),
            destination);
        var expirationTimeInSeconds = GetExpirationTimeInSeconds(userCredential, otpOptions, parameter);
        await _grantedTokenHelper.AddResetPasswordLink(otp.ToString(), login, parameter.Realm, expirationTimeInSeconds, cancellationToken);
    }

    public async Task<ResetPasswordLink> Verify(long code, CancellationToken cancellationToken)
    {
        var otpOptions = GetOTPOptions();
        var otpAuthenticator = _otpAuthenticators.First(o => o.Alg == otpOptions.OTPAlg);
        if (!otpAuthenticator.Verify(code, new Domains.UserCredential
        {
            OTPAlg = otpOptions.OTPAlg,
            Value = otpOptions.OTPValue,
            OTPCounter = otpOptions.OTPCounter,
            CredentialType = UserCredential.OTP,
            IsActive = true
        })) return null;
        var passwordLink = await _grantedTokenHelper.GetResetPasswordLink(code.ToString(), cancellationToken);
        if (passwordLink == null) return null;
        return passwordLink;
    }

    public abstract string GetDestination(User user);

    protected abstract IOTPRegisterOptions GetOTPOptions();

    private double GetExpirationTimeInSeconds(
        UserCredential credential, 
        IOTPRegisterOptions otpOptions,
        ResetPasswordParameter parameter)
    {
        if (credential.OTPAlg == OTPAlgs.TOTP) return otpOptions.TOTPStep;
        return parameter.DefaultExpirationTime;
    }
}
