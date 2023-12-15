// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Did
{
    public class IdentityDocumentBuilder
    {
        private const string DIDContext = "https://www.w3.org/ns/did/v1";
        private readonly List<string> _contextUrls = new List<string>();
        private readonly List<JsonObject> _context = new List<JsonObject>();
        private readonly List<string> _controllers = new List<string>();
        private readonly IdentityDocument _identityDocument;
        private readonly IEnumerable<IVerificationMethodBuilder> _verificationMethodBuilders = new List<IVerificationMethodBuilder>
        {
            new JWKVerificationMethodBuilder(),
            new PublicKeyMultibaseVerificationMethodBuilder()
        };

        protected IdentityDocumentBuilder(IdentityDocument identityDocument) 
        {
            _identityDocument = identityDocument;
        }

        protected IdentityDocument IdentityDocument => _identityDocument;

        public static IdentityDocumentBuilder New(string did, string publicAdr) => new IdentityDocumentBuilder(BuildDefaultDocument(did));

        public static IdentityDocumentBuilder New(IdentityDocument identityDocument) => new IdentityDocumentBuilder(identityDocument);

        #region JSON-LD Context

        public IdentityDocumentBuilder AddContext(params string[] contextLst)
        {
            foreach(var context in contextLst)
                if(!_contextUrls.Contains(context))
                    _contextUrls.Add(context);
            return this;
        }

        public IdentityDocumentBuilder AddContext(Action<ContextBuilder> callback)
        {
            var builder = new ContextBuilder();
            callback(builder);
            _context.Add(builder.Build());
            return this;
        }

        #endregion

        #region DIDs

        public IdentityDocumentBuilder AddController(string controller)
        {
            _controllers.Add(controller);
            return this;
        }

        public IdentityDocumentBuilder AddAlsoKnownAs(string alsoKnownAs)
        {
            if(_identityDocument.AlsoKnownAs == null)
                _identityDocument.AlsoKnownAs = new List<string>();
            _identityDocument.AlsoKnownAs.Add(alsoKnownAs);
            return this;
        }

        #endregion

        #region Verification method

        public IdentityDocumentBuilder AddJsonWebKeyVerificationMethod(ISignatureKey signatureKey, string controller)
            => AddVerificationMethod(signatureKey, controller, JWKVerificationMethodBuilder.JSON_LD_CONTEXT);

        public IdentityDocumentBuilder AddPublicKeyMultibaseVerificationMethod(ISignatureKey signatureKey, string controller)
            => AddVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodBuilder.JSON_LD_CONTEXT);

        public IdentityDocumentBuilder AddVerificationMethod(ISignatureKey signatureKey, string publicKeyFormat, KeyPurposes purpose = KeyPurposes.VerificationKey)
        {
            /// did:example:123#public-key-0 (UNIQUE VERIFICATION METHOD IN A DID DOCUMENT).
            // var verificationMethod = signatureKey.ExtractVerificationMethodWithPublicKey();
            IdentityDocumentVerificationMethod verificationMethod = null;
            var id = $"{_identityDocument.Id}#delegate-{(_identityDocument.VerificationMethod.Where(m => m.Id.Contains("#delegate")).Count() + 1)}";
            verificationMethod.Controller = _identityDocument.Id;
            verificationMethod.Id = id;
            verificationMethod.Type = publicKeyFormat;
            _identityDocument.AddVerificationMethod(verificationMethod, false);
            return AddVerificationMethod(verificationMethod, purpose);
        }

        public IdentityDocumentBuilder AddVerificationMethod(IdentityDocumentVerificationMethod verificationMethod, KeyPurposes purpose = KeyPurposes.VerificationKey)
        {
            switch (purpose)
            {
                case KeyPurposes.VerificationKey:
                    _identityDocument.AddAssertionMethod(verificationMethod.Id);
                    break;
                case KeyPurposes.SigAuthentication:
                    _identityDocument.AddAuthentication(verificationMethod.Id);
                    break;
                default:
                    throw new InvalidOperationException("enc is not supported");
            }

            return this;
        }

        #endregion

        public IdentityDocumentBuilder AddServiceEndpoint(string type, string serviceEndpoint)
        {
            var id = $"{_identityDocument.Id}#service-{(_identityDocument.Service.Count() + 1)}";
            _identityDocument.AddService(new IdentityDocumentService
            {
                Id = id,
                Type = type,
                ServiceEndpoint = serviceEndpoint
            }, false);
            return this;
        }

        public IdentityDocument Build()
        {
            _identityDocument.Context = BuildContext();
            _identityDocument.Controller = BuildController();
            return _identityDocument;
        }

        protected static IdentityDocument BuildDefaultDocument(string did)
        {
            var result = new IdentityDocument
            {
                Id = did,
                Context = Constants.DefaultIdentityDocumentContext
            };
            return result;
        }

        private JsonNode BuildContext()
        {
            if (!_contextUrls.Any() && !_context.Any())
                return DIDContext;
            var result = new JsonArray
            {
                DIDContext
            };
            foreach (var url in _contextUrls)
                result.Add(url);

            foreach (var context in _context)
                result.Add(context);

            return result;
        }

        private JsonNode BuildController()
        {
            if (!_controllers.Any()) return null;
            if (_controllers.Count() == 1) return _controllers.First();
            var result = new JsonArray();
            foreach(var controller in _controllers)
                result.Add(controller);
            return result;
        }

        private IdentityDocumentBuilder AddVerificationMethod(ISignatureKey signatureKey, string controller, string ldContext)
        {
            var builder = _verificationMethodBuilders.Single(v => v.JSONLDContext == ldContext);
            var verificationMethod = builder.Build(_identityDocument, signatureKey);
            verificationMethod.Type = builder.Type;
            verificationMethod.Controller = controller;
            AddContext(builder.JSONLDContext);
            _identityDocument.AddVerificationMethod(verificationMethod);
            return this;
        }
    }
}