// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.CredentialIssuer.Services;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialOffer.Commands;

public class CreateCredentialOfferCommand
{
    public string AccessToken { get; set; }
    public List<string> Grants { get; set; }
    public List<string> CredentialConfigurationIds { get; set; } = new List<string>();
    public string Subject { get; set; }
}

public class CredentialCredentialOfferResult
{
    public ErrorResult? Error { get; set; }
    public Domains.CredentialOfferRecord CredentialOffer { get; set; }
}

public interface ICreateCredentialOfferCommandHandler
{
    Task<CredentialCredentialOfferResult> Handle(CreateCredentialOfferCommand command, CancellationToken cancellationToken);
}

public class CreateCredentialOfferCommandHandler : ICreateCredentialOfferCommandHandler
{
    private readonly ICredentialOfferStore _credentialOfferStore;
    private readonly ICredentialConfigurationStore _credentialConfigurationStore;
    private readonly IPreAuthorizedCodeService _preAuthorizedCodeService;

    public CreateCredentialOfferCommandHandler(
        ICredentialOfferStore credentialOfferStore,
        ICredentialConfigurationStore credentialConfigurationStore,
        IPreAuthorizedCodeService preAuthorizedCodeService)
    {
        _credentialOfferStore = credentialOfferStore;
        _credentialConfigurationStore = credentialConfigurationStore;
        _preAuthorizedCodeService = preAuthorizedCodeService;
    }

    public async Task<CredentialCredentialOfferResult> Handle(CreateCredentialOfferCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await Validate(command, cancellationToken);
        if (validationResult.ErrorResult != null)
            return new CredentialCredentialOfferResult { Error = validationResult.ErrorResult };
        var credentialOffer = validationResult.CredentialOffer;
        if (credentialOffer.GrantTypes.Contains(CredentialOfferResultNames.AuthorizedCodeGrant))
        {
            credentialOffer.IssuerState = Guid.NewGuid().ToString();
        }

        if (credentialOffer.GrantTypes.Contains(CredentialOfferResultNames.PreAuthorizedCodeGrant))
        {
            credentialOffer.PreAuthorizedCode = await _preAuthorizedCodeService.Get(
                command.AccessToken, 
                validationResult.CredentialConfigurations.Select(c => c.Scope).Distinct().ToList(), 
                cancellationToken);
        }

        _credentialOfferStore.Add(credentialOffer);
        await _credentialOfferStore.SaveChanges(cancellationToken);
        return new CredentialCredentialOfferResult { CredentialOffer = credentialOffer };
    }

    private async Task<CredentialOfferValidationResult> Validate(CreateCredentialOfferCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return CredentialOfferValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_OFFER_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST));
        if (command.Grants == null || !command.Grants.Any())
            return CredentialOfferValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_OFFER_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialOfferResultNames.Grants)));
        if (command.CredentialConfigurationIds == null || !command.CredentialConfigurationIds.Any())
            return CredentialOfferValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_OFFER_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialOfferResultNames.CredentialConfigurationIds)));
        if (string.IsNullOrWhiteSpace(command.Subject))
            return CredentialOfferValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_OFFER_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, "sub")));
        var invalidGrants = command.Grants.Where(g => g != CredentialOfferResultNames.PreAuthorizedCodeGrant && g != CredentialOfferResultNames.AuthorizedCodeGrant);
        if (invalidGrants.Any())
            return CredentialOfferValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_OFFER_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_GRANT_TYPES, string.Join(',', invalidGrants))));
        var existingCredentials = await _credentialConfigurationStore.GetByServerIds(command.CredentialConfigurationIds, cancellationToken);
        var unknownCredentials = command.CredentialConfigurationIds.Where(id => !existingCredentials.Any(c => c.ServerId == id));
        if (unknownCredentials.Any())
            return CredentialOfferValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_OFFER_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_CREDENTIAL, string.Join(',', unknownCredentials))));
        var credentialOffer = new Domains.CredentialOfferRecord
        {
            Id = Guid.NewGuid().ToString(),
            GrantTypes = command.Grants,
            CredentialConfigurationIds = command.CredentialConfigurationIds,
            Subject = command.Subject,
            CreateDateTime = DateTime.UtcNow,
        };
        return CredentialOfferValidationResult.Ok(credentialOffer, existingCredentials);
    }

    private class CredentialOfferValidationResult : BaseValidationResult
    {
        private CredentialOfferValidationResult(Domains.CredentialOfferRecord credentialOffer, List<Domains.CredentialConfiguration> credentialConfigurations)
        {
            CredentialOffer = credentialOffer;
            CredentialConfigurations = credentialConfigurations;
        }

        private CredentialOfferValidationResult(ErrorResult error) : base(error)
        {
        }

        public Domains.CredentialOfferRecord CredentialOffer { get; private set; }
        public List<Domains.CredentialConfiguration> CredentialConfigurations { get; private set; }

        public static CredentialOfferValidationResult Ok(Domains.CredentialOfferRecord credentialOffer, List<Domains.CredentialConfiguration> credentialConfigurations) => new CredentialOfferValidationResult(credentialOffer, credentialConfigurations);

        public static CredentialOfferValidationResult Error(ErrorResult error) => new CredentialOfferValidationResult(error);
    }
}