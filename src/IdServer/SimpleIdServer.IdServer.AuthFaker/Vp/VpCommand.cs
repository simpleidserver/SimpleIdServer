// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Encoders;
using SimpleIdServer.Did.Jwt;
using SimpleIdServer.Did.Key;
using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc;
using SimpleIdServer.Vc.CredentialFormats.Serializers;
using SimpleIdServer.Vc.Models;
using SimpleIdServer.Vp;
using SimpleIdServer.Vp.Models;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

namespace SimpleIdServer.IdServer.AuthFaker.Vp;

public class VpCommand
{
    private const string _json = "{\r\n    \"@context\": [\r\n      \"https://www.w3.org/2018/credentials/v1\",\r\n      \"https://www.w3.org/2018/credentials/examples/v1\",\r\n      \"https://w3id.org/security/suites/jws-2020/v1\"\r\n    ],\r\n    \"id\": \"http://example.gov/credentials/3732\",\r\n    \"type\": [\"VerifiableCredential\", \"UniversityDegreeCredential\"],\r\n   \"issuanceDate\": \"2020-03-10T04:24:12.164Z\",\r\n    \"credentialSubject\": {\r\n      \"id\": \"did:example:456\",\r\n      \"degree\": {\r\n        \"type\": \"BachelorDegree\",\r\n        \"name\": \"Bachelor of Science and Arts\"\r\n      }\r\n    }\r\n}";

    public async Task Execute(string request)
    {
        request = HttpUtility.UrlDecode(request);
        var serializedRequest = JsonObject.Parse(request.Replace("openid4vp://authorize?", string.Empty)).AsObject();
        var responseUri = serializedRequest["response_uri"].ToString();
        var state = serializedRequest["state"].ToString();
        var vpToken = CreateVerifiablePresentation();
        await SendVerifiablePresentation(vpToken, state, responseUri);
    }

    private string CreateVerifiablePresentation()
    {
        var ed25119Sig = Ed25519SignatureKey.Generate();
        var did = DidKeyGenerator.New(Ed25519VerificationKey2020Standard.TYPE).SetSignatureKey(ed25119Sig).Export();
        var identityDocument = did.Document;
        var credential = JsonSerializer.Deserialize<W3CVerifiableCredential>(_json);
        var vc = SecuredDocument.New();
        credential.Issuer = did.Did;
        vc.Secure(credential, identityDocument, identityDocument.VerificationMethod.First().Id, asymKey: ed25119Sig);
        var vp = new VerifiablePresentation
        {
            Id = "id",
            VerifiableCredential = new List<string>
            {
                JsonSerializer.Serialize(credential)
            }
        };
        return JsonSerializer.Serialize(vp);
    }

    private async Task SendVerifiablePresentation(string vpToken, string state, string responseUri)
    {
        using (var client = new HttpClient())
        {
            var presentationSubmission = new PresentationSubmission
            {
                Id = "ps-1",
                DefinitionId = "universitydegree_vp",
                DescriptorMap = new List<DescriptorMap>
                {
                    new DescriptorMap
                    {
                        Id = "UniversityDegree",
                        PathNested = new PathNested
                        {
                            Path = "$.verifiableCredential[0]",
                            Format = "ldp_vc"
                        }
                    }
                }
            };
            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "vp_token", vpToken },
                { "state", state },
                { "presentation_submission", JsonSerializer.Serialize(presentationSubmission) }
            });
            var httpResult = await client.PostAsync(responseUri, formContent);
            var sss = "";
        }
    }
}
