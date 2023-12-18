// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.Multicodec;
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
        private readonly Dictionary<DidDocumentVerificationMethodUsages, List<DidDocumentVerificationMethod>> _innerVerificationMethods = new Dictionary<DidDocumentVerificationMethodUsages, List<DidDocumentVerificationMethod>>(); 
        private readonly DidDocument _identityDocument;
        private readonly IEnumerable<IVerificationMethodFormatter> _verificationMethodBuilders = new List<IVerificationMethodFormatter>
        {
            new JWKVerificationMethodFormatter(),
            new PublicKeyMultibaseVerificationMethodFormatter(
                new MulticodecSerializer(
                    new IVerificationMethod[] 
                    { 
                        new Ed25519VerificationMethod(),
                        new Es256KVerificationMethod()
                    }
                )
            )
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

        public DidDocumentBuilder AddJsonWebKeyVerificationMethod(ISignatureKey signatureKey, string controller, DidDocumentVerificationMethodUsages usage)
            => AddVerificationMethod(signatureKey, controller, JWKVerificationMethodFormatter.JSON_LD_CONTEXT, usage);

        public DidDocumentBuilder AddPublicKeyMultibaseVerificationMethod(ISignatureKey signatureKey, string controller, DidDocumentVerificationMethodUsages usage)
            => AddVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodFormatter.JSON_LD_CONTEXT, usage);

        public DidDocumentBuilder AddJsonWebKeyAuthentication(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, JWKVerificationMethodFormatter.JSON_LD_CONTEXT, DidDocumentVerificationMethodUsages.AUTHENTICATION);

        public DidDocumentBuilder AddPublicKeyMultibaseAuthentication(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodFormatter.JSON_LD_CONTEXT, DidDocumentVerificationMethodUsages.AUTHENTICATION);

        public DidDocumentBuilder AddJsonWebKeyAssertionMethod(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, JWKVerificationMethodFormatter.JSON_LD_CONTEXT, DidDocumentVerificationMethodUsages.ASSERTION_METHOD);

        public DidDocumentBuilder AddPublicKeyMultibaseAssertionMethod(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodFormatter.JSON_LD_CONTEXT, DidDocumentVerificationMethodUsages.ASSERTION_METHOD);

        public DidDocumentBuilder AddJsonWebKeyAgreement(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, JWKVerificationMethodFormatter.JSON_LD_CONTEXT, DidDocumentVerificationMethodUsages.KEY_AGREEMENT);

        public DidDocumentBuilder AddPublicKeyMultibaseAgreement(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodFormatter.JSON_LD_CONTEXT, DidDocumentVerificationMethodUsages.KEY_AGREEMENT);

        public DidDocumentBuilder AddJsonWebKeyCapabilityInvocation(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, JWKVerificationMethodFormatter.JSON_LD_CONTEXT, DidDocumentVerificationMethodUsages.CAPABILITY_INVOCATION);

        public DidDocumentBuilder AddPublicKeyMultibaseCapabilityInvocation(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodFormatter.JSON_LD_CONTEXT, DidDocumentVerificationMethodUsages.CAPABILITY_INVOCATION);

        public DidDocumentBuilder AddJsonWebKeyCapabilityDelegation(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, JWKVerificationMethodFormatter.JSON_LD_CONTEXT, DidDocumentVerificationMethodUsages.CAPABILITY_DELEGATION);

        public DidDocumentBuilder AddPublicKeyMultibaseCapabilityDelegation(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodFormatter.JSON_LD_CONTEXT, DidDocumentVerificationMethodUsages.CAPABILITY_DELEGATION);

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
            _identityDocument.Authentication = BuildEmbeddedVerificationMethods(DidDocumentVerificationMethodUsages.AUTHENTICATION);
            _identityDocument.AssertionMethod = BuildEmbeddedVerificationMethods(DidDocumentVerificationMethodUsages.ASSERTION_METHOD);
            _identityDocument.KeyAgreement = BuildEmbeddedVerificationMethods(DidDocumentVerificationMethodUsages.KEY_AGREEMENT);
            _identityDocument.CapabilityInvocation = BuildEmbeddedVerificationMethods(DidDocumentVerificationMethodUsages.CAPABILITY_INVOCATION);
            _identityDocument.CapabilityDelegation = BuildEmbeddedVerificationMethods(DidDocumentVerificationMethodUsages.CAPABILITY_DELEGATION);
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

        private DidDocumentBuilder AddVerificationMethod(ISignatureKey signatureKey, string controller, string ldContext, DidDocumentVerificationMethodUsages usage)
        {
            var verificationMethod = BuildVerificationMethod(signatureKey, controller, ldContext);
            verificationMethod.Usage = usage;
            _identityDocument.AddVerificationMethod(verificationMethod);
            return this;
        }

        private DidDocumentBuilder AddInnerVerificationMethod(ISignatureKey signatureKey, string controller, string ldContext, DidDocumentVerificationMethodUsages usage)
        {
            if(!_innerVerificationMethods.ContainsKey(usage))
            {
                _innerVerificationMethods.Add(usage, new List<DidDocumentVerificationMethod>());
            }

            var verificationMethod = BuildVerificationMethod(signatureKey, controller, ldContext);
            _innerVerificationMethods[usage].Add(verificationMethod);
            return this;
        }

        private DidDocumentVerificationMethod BuildVerificationMethod(ISignatureKey signatureKey, string controller, string ldContext)
        {
            var builder = _verificationMethodBuilders.Single(v => v.JSONLDContext == ldContext);
            var verificationMethod = builder.Format(_identityDocument, signatureKey);
            verificationMethod.Type = builder.Type;
            verificationMethod.Controller = controller;
            AddContext(builder.JSONLDContext);
            return verificationMethod;
        }

        private JsonArray BuildEmbeddedVerificationMethods(DidDocumentVerificationMethodUsages usage)
        {
            var referencedVerificationMethods = _identityDocument.VerificationMethod.Where(m => m.Usage.HasFlag(usage));
            var kvp = _innerVerificationMethods.SingleOrDefault(kvp => kvp.Key == usage);
            if (!referencedVerificationMethods.Any() && kvp.Value == null)
                return null;
            var result = new JsonArray();
            foreach (var referencedVerificationMethod in referencedVerificationMethods)
                result.Add(referencedVerificationMethod.Id);
            if(kvp.Value != null)
                foreach(var innerVerificationMethod in kvp.Value)
                    result.Add(JsonObject.Parse(JsonSerializer.Serialize(innerVerificationMethod)));
            return result;
        }
    }
}