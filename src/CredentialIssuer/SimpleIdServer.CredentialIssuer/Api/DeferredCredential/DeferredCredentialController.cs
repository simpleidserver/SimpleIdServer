// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.CredentialIssuer.Api.Credential;
using SimpleIdServer.CredentialIssuer.Api.Credential.Services;
using SimpleIdServer.CredentialIssuer.CredentialFormats;
using SimpleIdServer.CredentialIssuer.DTOs;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.DeferredCredential;

[Route(Constants.EndPoints.DeferredCredential)]
public class DeferredCredentialController : BaseController
{
    private readonly IDeferredCredentialStore _deferredCredentialStore;
    private readonly ICredentialService _credentialService;
    private readonly IEnumerable<ICredentialFormatter> _formatters;
    private readonly CredentialIssuerOptions _options;

    public DeferredCredentialController(
        IDeferredCredentialStore deferredCredentialStore,
        ICredentialService credentialService,
        IEnumerable<ICredentialFormatter> formatters,
        IOptions<CredentialIssuerOptions> options)
    {
        _deferredCredentialStore = deferredCredentialStore;
        _credentialService = credentialService;
        _formatters = formatters;
        _options = options.Value;
    }

    [HttpPost]
    public async Task<IActionResult> Get([FromBody] DeferredCredentialRequest request, CancellationToken cancellationToken)
    {
        if (_options.Version == CredentialIssuerVersion.ESBI)
        {
            if(Request.Headers.ContainsKey("Authorization"))
            {
                var authValue = Request.Headers["Authorization"].First();
                if(!string.IsNullOrWhiteSpace(authValue) && authValue.StartsWith("Bearer"))
                {
                    request = new DeferredCredentialRequest
                    {
                        TransactionId = authValue.Split(" ").Last()
                    };
                }
            }
        }

        var validationResult = await Validate(request, cancellationToken);
        if (validationResult.ErrorResult != null) return Build(validationResult.ErrorResult.Value);
        var formatter = _formatters.Single(f => f.Format == validationResult.DeferredCredential.FormatterName);
        CredentialResponseEncryption encryption = null;
        if(validationResult.DeferredCredential.EncryptionJwk != null)
        {
            encryption = new CredentialResponseEncryption
            {
                Jwk = new Microsoft.IdentityModel.Tokens.JsonWebKey(validationResult.DeferredCredential.EncryptionJwk),
                Alg = validationResult.DeferredCredential.EncryptionAlg,
                Enc = validationResult.DeferredCredential.EncryptionEnc
            };
        }

        var result = _credentialService.BuildImmediateCredential(new BuildImmediateCredentialRequest(
            validationResult.DeferredCredential.UserDid,
            validationResult.DeferredCredential.Configuration,
            null,
            validationResult.DeferredCredential.Claims.ToDictionary(c => c.Name, c => c.Value),
            formatter,
            encryption,
            validationResult.DeferredCredential.Nonce));
        _deferredCredentialStore.Delete(validationResult.DeferredCredential);
        await _deferredCredentialStore.SaveChanges(cancellationToken);
        return new OkObjectResult(result);
    }

    [Authorize("deferredcreds")]
    [HttpPost(".search")]
    public async Task<IActionResult> Search([FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        var result = await _deferredCredentialStore.Search(request, cancellationToken);
        return new OkObjectResult(result);
    }

    [Authorize("deferredcreds")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
    {
        var result = await _deferredCredentialStore.Get(id, cancellationToken);
        if (result == null) return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_DEFERRED_CREDENTIAL, id)));
        return new OkObjectResult(result);
    }

    [Authorize("deferredcreds")]
    [HttpPut("{id}/issue")]
    public async Task<IActionResult> Issue(string id, [FromBody] IssueDeferredCredentialRequest request, CancellationToken cancellationToken)
    {
        if (request == null) return Build(new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST));
        if (request.Claims == null) return Build(new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, DeferredCredentialResultNames.Claims)));
        var result = await _deferredCredentialStore.Get(id, cancellationToken);
        if (result == null) return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_DEFERRED_CREDENTIAL, id)));
        result.Claims = request.Claims.Select(kvp => new Domains.DeferredCredentialClaim
        {
            Id = Guid.NewGuid().ToString(),
            Name = kvp.Key,
            Value = kvp.Value
        }).ToList();
        result.Status = Domains.DeferredCredentialStatus.ISSUED;
        await _deferredCredentialStore.SaveChanges(cancellationToken);
        return new NoContentResult();
    }

    private async Task<DeferredCredentialValidationResult> Validate(
        DeferredCredentialRequest request,
        CancellationToken cancellationToken)
    {
        if (request == null) return DeferredCredentialValidationResult.Error(new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST));
        if (string.IsNullOrWhiteSpace(request.TransactionId)) return DeferredCredentialValidationResult.Error(new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialResultNames.TransactionId)));
        var transaction = await _deferredCredentialStore.Get(request.TransactionId, cancellationToken);
        if (transaction == null) return DeferredCredentialValidationResult.Error(new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_TRANSACTION_ID, ErrorMessages.INVALID_TRANSACTION_ID));
        if (transaction.Status == Domains.DeferredCredentialStatus.PENDING) return DeferredCredentialValidationResult.Error(new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.ISSUANCE_PENDING, ErrorMessages.ISSUANCE_PENDING));
        return DeferredCredentialValidationResult.Ok(transaction);
    }

    private class DeferredCredentialValidationResult : BaseValidationResult
    {
        private DeferredCredentialValidationResult(ErrorResult error) : base(error)
        {

        }

        private DeferredCredentialValidationResult(Domains.DeferredCredential deferredCredential)
        {
            DeferredCredential = deferredCredential;
        }

        public Domains.DeferredCredential DeferredCredential { get; private set; }

        public static DeferredCredentialValidationResult Ok(Domains.DeferredCredential deferredCredential) => new DeferredCredentialValidationResult(deferredCredential);

        public static DeferredCredentialValidationResult Error(ErrorResult error) => new DeferredCredentialValidationResult(error);
    }
}