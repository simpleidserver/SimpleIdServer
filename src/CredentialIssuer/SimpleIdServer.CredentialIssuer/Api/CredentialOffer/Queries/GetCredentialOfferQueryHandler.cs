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
    CredentialOfferResult Get(GetCredentialOfferQuery query, string authorizationServer);
    Task<byte[]> GetQrCode(GetCredentialOfferQuery query, string authorizationServer, CancellationToken cancellationToken);
    Task<BaseCredentialOfferRecordResult> ToDto(CredentialOfferRecord credentialOffer, string issuer, string authorizationServer, CancellationToken cancellationToken);
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

    public CredentialOfferResult Get(GetCredentialOfferQuery query, string authorizationServer)
    {
        return ToOfferDtoResult(query.CredentialOffer, query.Issuer, authorizationServer);
    }

    public async Task<byte[]> GetQrCode(GetCredentialOfferQuery query, string authorizationServer, CancellationToken cancellationToken)
    {
        var dto = await ToDto(query.CredentialOffer, query.Issuer, authorizationServer, cancellationToken);
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(dto.OfferUri, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }

    public async Task<BaseCredentialOfferRecordResult> ToDto(CredentialOfferRecord credentialOffer, string issuer, string authorizationServer, CancellationToken cancellationToken)
    {
        BaseCredentialOfferRecordResult result = null;
        if (_options.Version == CredentialIssuerVersion.LAST)
        {
            result = new LastCredentialOfferRecordResult
            {
                CredentialConfigurationIds = credentialOffer.CredentialConfigurationIds
            };
        }
        else
        {
            var credentialConfigurations = await _credentialConfigurationStore.GetByServerIds(credentialOffer.CredentialConfigurationIds, cancellationToken);
            result = new EsbiCredentialOfferResult
            {
                Credentials = credentialConfigurations.Select(c =>
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
                }).ToList()
            };
        }

        Enrich(result);
        return result;

        void Enrich(BaseCredentialOfferRecordResult r)
        {
            r.Id = credentialOffer.Id;
            r.GrantTypes = credentialOffer.GrantTypes;
            r.Subject = credentialOffer.Subject;
            r.CreateDateTime = credentialOffer.CreateDateTime;
            r.Offer = ToOfferDtoResult(credentialOffer, issuer, authorizationServer);
            if (_options.IsCredentialOfferReturnedByReference)
                r.OfferUri = SerializeByReference(r, issuer);
            else
                r.OfferUri = SerializeByValue(r);
        }
    }

    private CredentialOfferResult ToOfferDtoResult(CredentialOfferRecord credentialOffer, string issuer, string authorizationServer)
    {
        AuthorizedCodeGrant authorizedCodeGrant = null;
        PreAuthorizedCodeGrant preAuthorizedCodeGrant = null;
        if (credentialOffer.GrantTypes.Contains(CredentialOfferResultNames.AuthorizedCodeGrant))
        {
            authorizedCodeGrant = new AuthorizedCodeGrant
            {
                IssuerState = credentialOffer.IssuerState
            };
        }

        if (credentialOffer.GrantTypes.Contains(CredentialOfferResultNames.PreAuthorizedCodeGrant))
        {
            preAuthorizedCodeGrant = new PreAuthorizedCodeGrant
            {
                PreAuthorizedCode = credentialOffer.PreAuthorizedCode,
                AuthorizationServer = authorizationServer
            };
        }

        return new CredentialOfferResult
        {
            CredentialIssuer = issuer,
            CredentialConfigurationIds = credentialOffer.CredentialConfigurationIds,
            Grants = new CredentialOfferGrants
            {
                AuthorizedCodeGrant = authorizedCodeGrant,
                PreAuthorizedCodeGrant = preAuthorizedCodeGrant
            }
        };
    }

    private string SerializeByReference<T>(T offerRecord, string issuer) where T : BaseCredentialOfferRecordResult
    {
        var url = $"{issuer}/{Constants.EndPoints.CredentialOffer}/{offerRecord.Id}";
        return $"openid-credential-offer://?credential_offer_uri={url}";
    }

    private string SerializeByValue<T>(T offerRecord) where T : BaseCredentialOfferRecordResult
    {
        var json = JsonSerializer.Serialize(offerRecord.Offer);
        var encodedJson = HttpUtility.UrlEncode(json);
        return $"openid-credential-offer://?credential_offer={encodedJson}";
    }
}
