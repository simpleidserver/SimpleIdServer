// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleIdServer.Saml.DTOs
{
    public class SAMLResponseDto
    {
        public string SAMLResponse { get; set; }
        public string RelayState { get; set; }
        public string SigAlg { get; set; }
        public string Signature { get; set; }

        public string ToQuery(bool withSignature = true)
        {
            var result = new Dictionary<string, string>
            {
                { nameof(SAMLResponse), HttpUtility.UrlEncode(SAMLResponse) },
                { nameof(RelayState), HttpUtility.UrlEncode(RelayState) }
            };
            if (!string.IsNullOrWhiteSpace(SigAlg))
            {
                result.Add(nameof(SigAlg), HttpUtility.UrlEncode(SigAlg));
            }

            if (!string.IsNullOrWhiteSpace(Signature) && withSignature)
            {
                result.Add(nameof(Signature), HttpUtility.UrlEncode(Signature) );
            }

            return string.Join("&", result.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }

        public static SAMLResponseDto Build(Dictionary<string, string> collection)
        {
            var result = new SAMLResponseDto();
            if (collection.ContainsKey(nameof(SAMLResponse)))
            {
                result.SAMLResponse = collection[nameof(SAMLResponse)];
            }

            if (collection.ContainsKey(nameof(RelayState)))
            {
                result.RelayState = collection[nameof(RelayState)];
            }

            if (collection.ContainsKey(nameof(SigAlg)))
            {
                result.SigAlg = collection[nameof(SigAlg)];
            }

            if (collection.ContainsKey(nameof(Signature)))
            {
                result.Signature = collection[nameof(Signature)];
            }

            return result;
        }
    }
}
