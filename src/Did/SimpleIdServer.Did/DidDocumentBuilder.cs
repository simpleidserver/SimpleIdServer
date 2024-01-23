// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Encoders;
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
        private readonly IVerificationMethodEncoding _verificationMethodEncoding;

        protected DidDocumentBuilder(DidDocument identityDocument,
            IVerificationMethodEncoding verificationMethodEncoding)
        {
            _identityDocument = identityDocument;
            _verificationMethodEncoding = verificationMethodEncoding;
        }

        protected DidDocument IdentityDocument => _identityDocument;

        public static DidDocumentBuilder New(string id, IVerificationMethodEncoding verificationMethodEncoding) => new DidDocumentBuilder(new DidDocument
        {
            Id = id
        }, verificationMethodEncoding);

        public static DidDocumentBuilder New(string id) => new DidDocumentBuilder(new DidDocument
        {
            Id = id
        }, new VerificationMethodEncoding(VerificationMethodStandardFactory.GetAll(), MulticodecSerializerFactory.Build(), MulticodecSerializerFactory.AllVerificationMethods));

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

        public DidDocumentBuilder AddVerificationMethod(string verificationMethodStandard,
            IAsymmetricKey asymmKey,
            string controller,
            VerificationMethodUsages usage,
            bool isReference = true,
            bool includePrivateKey = false,
            SignatureKeyEncodingTypes? encoding = null,
            Action<DidDocumentVerificationMethod> callback = null)
        {
            var verificationMethod = _verificationMethodEncoding.Encode(
                verificationMethodStandard,
                controller,
                asymmKey,
                encoding,
                includePrivateKey);
            if (callback != null) callback(verificationMethod);
            return this.AddVerificationMethod(verificationMethod, usage, isReference);
        }

        public DidDocumentBuilder AddKeyAggreement(
            string verificationMethodStandard,
            X25519AgreementKey asymmKey,
            string controller,
            bool isReference = true)
        {
            var verificationMethod = _verificationMethodEncoding.Encode(
                verificationMethodStandard,
                controller,
                asymmKey);
            return this.AddVerificationMethod(verificationMethod, VerificationMethodUsages.KEY_AGREEMENT, isReference);
        }

        public DidDocumentBuilder AddVerificationMethod(
            DidDocumentVerificationMethod verificationMethod,
            VerificationMethodUsages usage,
            bool isReference = true)
        {
            var standard = _verificationMethodEncoding.Standards.Single(s => s.Type == verificationMethod.Type);
            if(!string.IsNullOrWhiteSpace(standard.JSONLDContext))
                AddContext(standard.JSONLDContext);
            verificationMethod.Usage = usage;
            if(string.IsNullOrWhiteSpace(verificationMethod.Id))
            {
                var index = (_identityDocument.VerificationMethod.Count() + _innerVerificationMethods.Count()) + 1;
                verificationMethod.Id = $"{verificationMethod.Controller}#keys-{index}";
            }

            if (isReference)
                _identityDocument.AddVerificationMethod(verificationMethod);
            else
                _innerVerificationMethods.Add(verificationMethod);

            return this;
        }

        #endregion

        #region Service endpoint

        public DidDocumentBuilder AddServiceEndpoint(string type, string serviceEndpoint)
        {
            if (_identityDocument.Service == null)
                _identityDocument.Service = new List<DidDocumentService>();
            var id = $"{_identityDocument.Id}#service-{(_identityDocument.Service.Count() + 1)}";
            _identityDocument.AddService(new DidDocumentService
            {
                Id = id,
                Type = type,
                ServiceEndpoint = serviceEndpoint
            }, false);
            return this;
        }

        public DidDocumentBuilder AddServiceEndpoint(DidDocumentService service)
        {
            if (_identityDocument.Service == null)
                _identityDocument.Service = new List<DidDocumentService>();
            _identityDocument.Service.Add(service);
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