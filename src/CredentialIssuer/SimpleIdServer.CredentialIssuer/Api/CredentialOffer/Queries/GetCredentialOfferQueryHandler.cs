// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using QRCoder;
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialOffer;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Text.Json;
using System.Web;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialOffer.Queries;

public interface IGetCredentialOfferQueryHandler
{
    CredentialOfferResult Get(GetCredentialOfferQuery query, string authorizationServer);
    byte[] GetQrCode(GetCredentialOfferQuery query, string authorizationServer);
    CredentialOfferRecordResult ToDto(CredentialOfferRecord credentialOffer, string issuer, string authorizationServer);
}

public class GetCredentialOfferQuery
{
    public CredentialOfferRecord CredentialOffer { get; set; }
    public string Issuer { get; set; }
}

public class GetCredentialOfferQueryHandler : IGetCredentialOfferQueryHandler
{
    private readonly CredentialIssuerOptions _options;

    public GetCredentialOfferQueryHandler(IOptions<CredentialIssuerOptions> options)
    {
        _options = options.Value;
    }

    public CredentialOfferResult Get(GetCredentialOfferQuery query, string authorizationServer)
    {
        return ToOfferDtoResult(query.CredentialOffer, query.Issuer, authorizationServer);
    }

    public byte[] GetQrCode(GetCredentialOfferQuery query, string authorizationServer)
    {
        var dto = ToDto(query.CredentialOffer, query.Issuer, authorizationServer);
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(dto.OfferUri, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }

    public CredentialOfferRecordResult ToDto(CredentialOfferRecord credentialOffer, string issuer, string authorizationServer)
    {
        var result = new CredentialOfferRecordResult
        {
            Id = credentialOffer.Id,
            GrantTypes = credentialOffer.GrantTypes,
            Subject = credentialOffer.Subject,
            CredentialConfigurationIds = credentialOffer.CredentialConfigurationIds,
            CreateDateTime = credentialOffer.CreateDateTime,
            Offer = ToOfferDtoResult(credentialOffer, issuer, authorizationServer)
        };
        if (_options.IsCredentialOfferReturnedByReference)
            result.OfferUri = SerializeByReference(result, issuer);
        else
            result.OfferUri = SerializeByValue(result);

        return result;
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

    private string SerializeByReference(CredentialOfferRecordResult offerRecord, string issuer)
    {
        var url = $"{issuer}/{Constants.EndPoints.CredentialOffer}/{offerRecord.Id}";
        return $"openid-credential-offer://?credential_offer_uri={url}";
    }

    private string SerializeByValue(CredentialOfferRecordResult offerRecord)
    {
        var json = JsonSerializer.Serialize(offerRecord.Offer);
        var encodedJson = HttpUtility.UrlEncode(json);
        return $"openid-credential-offer://?credential_offer={encodedJson}";
    }
}
