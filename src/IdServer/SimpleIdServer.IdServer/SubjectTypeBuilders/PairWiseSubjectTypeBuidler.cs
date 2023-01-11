// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.SubjectTypeBuilders
{
    public class PairWiseSubjectTypeBuidler : ISubjectTypeBuilder
    {
        private readonly IClientHelper _clientHelper;
        private readonly IKeyStore _keyStore;

        public PairWiseSubjectTypeBuidler(IClientHelper clientHelper, IKeyStore keyStore)
        {
            _clientHelper = clientHelper;
            _keyStore = keyStore;
        }

        public string SubjectType => SUBJECT_TYPE;
        public static string SUBJECT_TYPE = "pairwise";

        public async Task<string> Build(HandlerContext context, CancellationToken cancellationToken)
        {
            var encryptedKeys = _keyStore.GetAllEncryptingKeys();
            var encryptedKey = encryptedKeys.First(k => k.Key.KeyId == "subject-encrypt");
            // Check if this kind of request is working
            /*
             * {
            "scope"        : [ "read", "write" ],
            "audience"     : [ "https://api.example.com" ],
            "access_token" : { "sub_type" : "PAIRWISE" }
            }
            */
            var client = context.Client;
            var redirectUri = context.Request.RequestData.GetRedirectUriFromAuthorizationRequest();
            var url = redirectUri;
            var sectorIdentifierUrls = await _clientHelper.GetSectorIdentifierUrls(client, cancellationToken);
            if (sectorIdentifierUrls.Contains(url))
                url = client.SectorIdentifierUri;

            var uri = new Uri(url);
            var host = uri.Host;
            // RSA(sector_id |local_sub)
            var str = $"{host}|{context.User.Id}";
            var rsa = (encryptedKey.Key as RsaSecurityKey).Rsa;
            return rsa.EncryptValue(Encoding.UTF8.GetBytes(str)).Base64EncodeBytes();
        }
    }
}
