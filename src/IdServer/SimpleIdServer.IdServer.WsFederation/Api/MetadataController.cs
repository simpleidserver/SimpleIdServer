// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.WsFederation;
using Microsoft.IdentityModel.Xml;
using SimpleIdServer.IdServer.Extensions;
using SimpleIdServer.IdServer.Stores;
using System.Net;
using System.Text;
using System.Xml;

namespace SimpleIdServer.IdServer.WsFederation.Api
{
    public class MetadataController : BaseWsFederationController
    {

        public MetadataController(IOptions<IdServerWsFederationOptions> options, IKeyStore keyStore) : base(options, keyStore) { }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var sigKeys = KeyStore.GetAllSigningKeys();

            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var configuration = new WsFederationConfiguration
            {
                Issuer = issuer,
                TokenEndpoint = $"{issuer}/{WsFederationConstants.EndPoints.SSO}"
            };
            foreach (var sigKey in sigKeys)
                configuration.KeyInfos.Add(new KeyInfo(sigKey.Key));

            configuration.SigningCredentials = GetSigningCredentials(sigKeys);

            var xml = Serialize(configuration);
            return new ContentResult
            {
                Content = xml,
                ContentType = "application/xml",
                StatusCode = (int)HttpStatusCode.OK
            };

            string Serialize(WsFederationConfiguration configuration)
            {
                var serializer = new WsFederationMetadataSerializer();
                using (var memStream = new MemoryStream())
                {
                    using (var writer = XmlWriter.Create(memStream))
                    {
                        serializer.WriteMetadata(writer, configuration);
                    }

                    return Encoding.UTF8.GetString(memStream.GetBuffer());
                }
            }
        }
    }
}
