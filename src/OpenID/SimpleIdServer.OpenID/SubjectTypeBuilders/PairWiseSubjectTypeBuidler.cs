// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Infrastructures;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        public async Task<string> Build(HandlerContext context)
        {
            var redirectUri = context.Request.QueryParameters.GetRedirectUriFromAuthorizationRequest();
            var url = redirectUri;
            var sectorIdentifierUrls = await context.Client.GetSectorIdentifierUrls(_httpClientFactory);
            if (sectorIdentifierUrls.Contains(url))
            {
                url = context.Client.SectorIdentifierUri;
            }

            var uri = new Uri(url);
            var host = uri.Host;
            var str = $"{host}{context.User.Id}{context.Client.PairWiseIdentifierSalt}";
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(str)).Base64EncodeBytes();
            }
        }
    }
}