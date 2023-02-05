// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Exceptions;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.SubjectTypeBuilders
{
    /// <summary>
    /// RFC : https://openid.net/specs/openid-connect-core-1_0.html#PairwiseAlg
    /// </summary>
    public class PairWiseSubjectTypeBuidler : ISubjectTypeBuilder
    {
        public string SubjectType => SUBJECT_TYPE;
        public static string SUBJECT_TYPE = "pairwise";

        public Task<string> Build(HandlerContext context, CancellationToken cancellationToken)
        {
            var client = context.Client;
            if (!client.RedirectionUrls.Any() && string.IsNullOrWhiteSpace(client.SectorIdentifierUri))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CANNOT_GENERATE_PAIRWISE_SUBJECT_BECAUSE_NO_SECTOR_IDENTIFIER);

            if (string.IsNullOrWhiteSpace(client.SectorIdentifierUri) && client.RedirectionUrls.Count() > 1)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CANNOT_GENERATE_PAIRWISE_SUBJECT_MORE_THAN_ONE_REDIRECT_URLS);
           
            var url = client.RedirectionUrls.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(client.SectorIdentifierUri))
                url = client.SectorIdentifierUri;

            var uri = new Uri(url);
            var host = uri.Host;
            // SHA256(sector_id |local_sub|salt)
            var str = $"{host}|{context.User.Name}|{client.PairWiseIdentifierSalt}";
            using (var sha256 = SHA256.Create())
                return Task.FromResult(sha256.ComputeHash(Encoding.UTF8.GetBytes(str)).Base64EncodeBytes());
        }
    }
}
