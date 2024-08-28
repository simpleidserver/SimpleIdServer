// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using QRCoder;
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialOffer;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialOffer.Queries;

public interface IGetCredentialOfferQueryHandler
{
    Task<BaseCredentialOfferResult> Get(GetCredentialOfferQuery query, string authorizationServer, CancellationToken cancellationToken);
    Task<byte[]> GetQrCode(GetCredentialOfferQuery query, string authorizationServer, CancellationToken cancellationToken);
    Task<CredentialOfferRecordResult> ToDto(CredentialOfferRecord credentialOffer, string issuer, string authorizationServer, CancellationToken cancellationToken);
}

public class GetCredentialOfferQuery
{
    public CredentialOfferRecord CredentialOffer { get; set; }
    public string Issuer { get; set; }
}

public class GetCredentialOfferQueryHandler : IGetCredentialOfferQueryHandler
{
    private readonly CredentialIssuerOptions _options;
    private readonly ICredentialConfigurationStore _credentialConfigurationStore;

    public GetCredentialOfferQueryHandler(IOptions<CredentialIssuerOptions> options, ICredentialConfigurationStore credentialConfigurationStore)
    {
        _options = options.Value;
        _credentialConfigurationStore = credentialConfigurationStore;
    }

    public async Task<BaseCredentialOfferResult> Get(GetCredentialOfferQuery query, string authorizationServer, CancellationToken cancellationToken)
    {
        if (_options.Version == CredentialIssuerVersion.LAST)
        {
            return ToLastOfferDtoResult(query.CredentialOffer, query.Issuer, authorizationServer);
        }
        
        return await ToEsbiOfferDtoResult(query.CredentialOffer, query.Issuer, authorizationServer, cancellationToken);
    }

    public async Task<byte[]> GetQrCode(GetCredentialOfferQuery query, string authorizationServer, CancellationToken cancellationToken)
    {
        var dto = await ToDto(query.CredentialOffer, query.Issuer, authorizationServer, cancellationToken);
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(dto.OfferUri, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }

    public async Task<CredentialOfferRecordResult> ToDto(CredentialOfferRecord credentialOffer, string issuer, string authorizationServer, CancellationToken cancellationToken)
    {
        var result = new CredentialOfferRecordResult
        {
            Id = credentialOffer.Id,
            CredentialConfigurationIds = credentialOffer.CredentialConfigurationIds,
            GrantTypes = credentialOffer.GrantTypes,
            Subject = credentialOffer.Subject,
            CreateDateTime = credentialOffer.CreateDateTime,
        };
        if (_options.Version == CredentialIssuerVersion.LAST)
        {
            result.Offer = ToLastOfferDtoResult(credentialOffer, issuer, authorizationServer);
        }
        else
        {
            result.Offer = await ToEsbiOfferDtoResult(credentialOffer, issuer, authorizationServer, cancellationToken);
        }

        if (_options.IsCredentialOfferReturnedByReference)
            result.OfferUri = SerializeByReference(result, issuer);
        else
            result.OfferUri = SerializeByValue(result);

        return result;
    }

    private LastCredentialOfferResult ToLastOfferDtoResult(CredentialOfferRecord credentialOffer, string issuer, string authorizationServer)
    {
        var result = new LastCredentialOfferResult();
        Enrich(result, credentialOffer, issuer, authorizationServer);
        return result;
    }

    private async Task<EsbiCredentialOfferResult> ToEsbiOfferDtoResult(CredentialOfferRecord credentialOffer, string issuer, string authorizationServer, CancellationToken cancellationToken)
    {
        var result = new EsbiCredentialOfferResult();
        var credentialConfigurations = await _credentialConfigurationStore.Get(credentialOffer.CredentialConfigurationIds, cancellationToken);
        Enrich(result, credentialOffer, issuer, authorizationServer);
        result.Credentials = credentialConfigurations.Select(c =>
        {
            var types = new List<string>();
            if (c.AdditionalTypes != null)
                types.AddRange(c.AdditionalTypes);
            types.Add(c.Type);
            return new EsbiCredential
            {
                Format = c.Format,
                Types = types
            };
        }).ToList();
        return result;
    }

    private void Enrich(BaseCredentialOfferResult credentialOffer, CredentialOfferRecord credentialOfferRecord, string issuer, string authorizationServer)
    {
        var result = new LastCredentialOfferResult();
        AuthorizedCodeGrant authorizedCodeGrant = null;
        PreAuthorizedCodeGrant preAuthorizedCodeGrant = null;
        if (credentialOfferRecord.GrantTypes.Contains(CredentialOfferResultNames.AuthorizedCodeGrant))
        {
            authorizedCodeGrant = new AuthorizedCodeGrant
            {
                IssuerState = credentialOfferRecord.IssuerState
            };
        }

        if (credentialOfferRecord.GrantTypes.Contains(CredentialOfferResultNames.PreAuthorizedCodeGrant))
        {
            preAuthorizedCodeGrant = new PreAuthorizedCodeGrant
            {
                PreAuthorizedCode = credentialOfferRecord.PreAuthorizedCode,
                AuthorizationServer = authorizationServer
            };
        }

        credentialOffer.CredentialIssuer = issuer;
        credentialOffer.Grants = new CredentialOfferGrants
        {
            AuthorizedCodeGrant = authorizedCodeGrant,
            PreAuthorizedCodeGrant = preAuthorizedCodeGrant
        };
    }

    private string SerializeByReference<T>(T offerRecord, string issuer) where T : CredentialOfferRecordResult
    {
        var url = $"{issuer}/{Constants.EndPoints.CredentialOffer}/{offerRecord.Id}";
        return $"openid-credential-offer://?credential_offer_uri={url}";
    }

    private string SerializeByValue<T>(T offerRecord) where T : CredentialOfferRecordResult
    {
        var json = JsonSerializer.Serialize(offerRecord.Offer);
        var encodedJson = HttpUtility.UrlEncode(json);
        return $"openid-credential-offer://?credential_offer={encodedJson}";
    }
}
