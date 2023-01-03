// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Infrastructures;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.SubjectTypeBuilders
{
    public class PairWiseSubjectTypeBuidler : ISubjectTypeBuilder
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PairWiseSubjectTypeBuidler(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string SubjectType => SUBJECT_TYPE;
        public static string SUBJECT_TYPE = "pairwise";

        public async Task<string> Build(HandlerContext context, CancellationToken cancellationToken)
        {
            var client = context.Client;
            var redirectUri = context.Request.RequestData.GetRedirectUriFromAuthorizationRequest();
            var url = redirectUri;
            var sectorIdentifierUrls = await client.GetSectorIdentifierUrls(_httpClientFactory, cancellationToken);
            if (sectorIdentifierUrls.Contains(url))
            {
                url = client.SectorIdentifierUri;
            }

            var uri = new Uri(url);
            var host = uri.Host;
            var str = $"{host}{context.User.Id}{client.PairWiseIdentifierSalt}";
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(str)).Base64EncodeBytes();
            }
        }
    }
}