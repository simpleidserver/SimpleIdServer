// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Formatters;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Did
{
    public class DidDocumentBuilder
    {
        private const string DIDContext = "https://www.w3.org/ns/did/v1";
        private readonly List<string> _contextUrls = new List<string>();
        private readonly List<JsonObject> _context = new List<JsonObject>();
        private readonly List<string> _controllers = new List<string>();
        private readonly List<DidDocumentVerificationMethod> _innerVerificationMethods = new List<DidDocumentVerificationMethod>(); 
        private readonly DidDocument _identityDocument;
        private readonly IEnumerable<IVerificationMethodFormatter> _verificationMethodBuilders = new List<IVerificationMethodFormatter>
        {
            FormatterFactory.BuildJWKVerificationMethod(),
            FormatterFactory.BuildPublicKeyMultibase()
        };

        protected DidDocumentBuilder(DidDocument identityDocument) 
        {
            _identityDocument = identityDocument;
        }

        protected DidDocumentBuilder(DidDocument identityDocument,
            IEnumerable<IVerificationMethodFormatter> verificationMethodBuilders) : this(identityDocument)
        {
            _verificationMethodBuilders = verificationMethodBuilders;
        }

        protected DidDocument IdentityDocument => _identityDocument;

        public static DidDocumentBuilder New(string id) => new DidDocumentBuilder(new DidDocument
        {
            Id = id
        });

        public static DidDocumentBuilder New(DidDocument identityDocument) => new DidDocumentBuilder(identityDocument);

        #region JSON-LD Context

        public DidDocumentBuilder AddContext(params string[] contextLst)
        {
            foreach(var context in contextLst)
                if(!_contextUrls.Contains(context))
                    _contextUrls.Add(context);
            return this;
        }

        public DidDocumentBuilder AddContext(Action<ContextBuilder> callback)
        {
            var builder = new ContextBuilder();
            callback(builder);
            _context.Add(builder.Build());
            return this;
        }

        #endregion

        #region DIDs

        public DidDocumentBuilder AddController(string controller)
        {
            _controllers.Add(controller);
            return this;
        }

        public DidDocumentBuilder AddAlsoKnownAs(string alsoKnownAs)
        {
            if(_identityDocument.AlsoKnownAs == null)
                _identityDocument.AlsoKnownAs = new List<string>();
            _identityDocument.AlsoKnownAs.Add(alsoKnownAs);
            return this;
        }

        #endregion

        #region Verification method

        public DidDocumentBuilder AddJsonWebKeyVerificationMethod(IAsymmetricKey asymmKey, string controller, VerificationMethodUsages usage, bool isReference = true)
            => AddVerificationMethod(asymmKey, controller, JWKVerificationMethodFormatter.JSON_LD_CONTEXT, usage, isReference);

        public DidDocumentBuilder AddPublicKeyMultibaseVerificationMethod(IAsymmetricKey asymmKey, string controller, VerificationMethodUsages usage, bool isReference = true)
            => AddVerificationMethod(asymmKey, controller, PublicKeyMultibaseVerificationMethodFormatter.JSON_LD_CONTEXT, usage, isReference);

        #endregion

        #region Service endpoint
        public DidDocumentBuilder AddServiceEndpoint(string type, string serviceEndpoint)
        {
            var id = $"{_identityDocument.Id}#service-{(_identityDocument.Service.Count() + 1)}";
            _identityDocument.AddService(new DidDocumentService
            {
                Id = id,
                Type = type,
                ServiceEndpoint = serviceEndpoint
            }, false);
            return this;
        }

        #endregion

        public DidDocument Build()
        {
            _identityDocument.Context = BuildContext();
            _identityDocument.Controller = BuildController();
            _identityDocument.Authentication = BuildEmbeddedVerificationMethods(VerificationMethodUsages.AUTHENTICATION);
            _identityDocument.AssertionMethod = BuildEmbeddedVerificationMethods(VerificationMethodUsages.ASSERTION_METHOD);
            _identityDocument.KeyAgreement = BuildEmbeddedVerificationMethods(VerificationMethodUsages.KEY_AGREEMENT);
            _identityDocument.CapabilityInvocation = BuildEmbeddedVerificationMethods(VerificationMethodUsages.CAPABILITY_INVOCATION);
            _identityDocument.CapabilityDelegation = BuildEmbeddedVerificationMethods(VerificationMethodUsages.CAPABILITY_DELEGATION);
            return _identityDocument;
        }

        protected static DidDocument BuildDefaultDocument(string did)
        {
            var result = new DidDocument
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

        private DidDocumentBuilder AddVerificationMethod(IAsymmetricKey asymmKey, string controller, string ldContext, VerificationMethodUsages usage, bool isReference)
        {
            var isSigKey = (asymmKey as ISignatureKey) != null;
            if (usage.HasFlag(VerificationMethodUsages.KEY_AGREEMENT) && isSigKey) throw new ArgumentException("Signature key cannot be used in a Key Agreement");
            if (!isSigKey && !usage.HasFlag(VerificationMethodUsages.KEY_AGREEMENT)) throw new ArgumentException("Key can only be used in a Key Agreement");
            var verificationMethod = BuildVerificationMethod(asymmKey, controller, ldContext);
            verificationMethod.Usage = usage;
            if (isReference)
            {
                _identityDocument.AddVerificationMethod(verificationMethod);
            }
            else
            {
                _innerVerificationMethods.Add(verificationMethod);
            }

            return this;
        }

        private DidDocumentVerificationMethod BuildVerificationMethod(IAsymmetricKey signatureKey, string controller, string ldContext)
        {
            var builder = _verificationMethodBuilders.Single(v => v.JSONLDContext == ldContext);
            var verificationMethod = builder.Format(_identityDocument, signatureKey);
            verificationMethod.Type = builder.Type;
            verificationMethod.Controller = controller;
            AddContext(builder.JSONLDContext);
            return verificationMethod;
        }

        private JsonArray BuildEmbeddedVerificationMethods(VerificationMethodUsages usage)
        {
            var referencedVerificationMethods = _identityDocument.VerificationMethod.Where(m => m.Usage.HasFlag(usage));
            var innerVerificationMethods = _innerVerificationMethods.Where(v => v.Usage.HasFlag(usage));
            if (!referencedVerificationMethods.Any() && !innerVerificationMethods.Any())
                return null;
            var result = new JsonArray();
            foreach (var referencedVerificationMethod in referencedVerificationMethods)
                result.Add(referencedVerificationMethod.Id);
            foreach(var innerVerificationMethod in innerVerificationMethods)
                result.Add(JsonObject.Parse(JsonSerializer.Serialize(innerVerificationMethod)));
            return result;
        }
    }
}