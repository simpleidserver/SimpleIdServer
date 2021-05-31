// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OpenID.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.ClaimsEnrichers
{
    public class AggregateHttpClaimsSource : IClaimsSource
    {
        private readonly AggregateHttpClaimsSourceOptions _httpClaimsSourceOptions;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IJwtParser _jwtParser;

        public AggregateHttpClaimsSource(AggregateHttpClaimsSourceOptions httpClaimsSourceOptions, IJwtBuilder jwtBuilder, IJwtParser jwtParser)
        {
            _httpClaimsSourceOptions = httpClaimsSourceOptions;
            _jwtBuilder = jwtBuilder;
            _jwtParser = jwtParser;
        }

        public async Task Enrich(JwsPayload jwsPayload, OpenIdClient client, CancellationToken cancellationToken)
        {
            var subject = jwsPayload.GetClaimValue(Jwt.Constants.UserClaims.Subject);
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(_httpClaimsSourceOptions.Url),
                    Method = HttpMethod.Get,
                    // Content = new StringContent(JsonConvert.SerializeObject(new { sub = subject }), Encoding.UTF8, "application/json")
                };
                if (!string.IsNullOrWhiteSpace(_httpClaimsSourceOptions.ApiToken))
                {
                    request.Headers.Add("Authorization", $"Bearer {_httpClaimsSourceOptions.ApiToken}");
                }

                var result = await httpClient.SendAsync(request).ConfigureAwait(false);
                var json = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                var kvp = result.Content.Headers.FirstOrDefault(k => k.Key == "Content-Type");
                if (!kvp.Equals(default(KeyValuePair<string, IEnumerable<string>>)) && !string.IsNullOrWhiteSpace(kvp.Key) && result.IsSuccessStatusCode)
                {
                    string jwt = null;
                    JwsPayload jObj = null;
                    if (kvp.Value.Any(v => v.Contains("application/json")))
                    {
                        jObj = JsonConvert.DeserializeObject<JwsPayload>(json);
                        jwt = await _jwtBuilder.BuildClientToken(client, jObj, client.IdTokenSignedResponseAlg, client.IdTokenEncryptedResponseAlg, client.IdTokenEncryptedResponseEnc, cancellationToken);
                    }
                    else if (kvp.Value.Any(v => v.Contains("application/jwt")))
                    {
                        throw new NotImplementedException();
                        /*
                        jwt = json;
                        jObj = Extract(json);
                        */
                    }

                    if (!string.IsNullOrWhiteSpace(jwt))
                    {
                        ClaimsSourceHelper.AddAggregate(jwsPayload, jObj, jwt, _httpClaimsSourceOptions.Name);
                    }
                }
            }
        }

        /*
        private JwsPayload Extract(string jwt)
        {
            string jws = jwt;
            if (_jwtParser.IsJweToken(jwt))
            {
                jws = _jwtParser.Decrypt(jwt);
            }

            return _jwtParser.Unsign(jws);
        }
        */
    }
}
