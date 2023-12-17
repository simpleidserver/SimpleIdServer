// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Did
{
    public class IdentityDocumentBuilder
    {
        private const string DIDContext = "https://www.w3.org/ns/did/v1";
        private readonly List<string> _contextUrls = new List<string>();
        private readonly List<JsonObject> _context = new List<JsonObject>();
        private readonly List<string> _controllers = new List<string>();
        private readonly Dictionary<IdentityDocumentVerificationMethodUsages, List<IdentityDocumentVerificationMethod>> _innerVerificationMethods = new Dictionary<IdentityDocumentVerificationMethodUsages, List<IdentityDocumentVerificationMethod>>(); 
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

        public static IdentityDocumentBuilder New(string id) => new IdentityDocumentBuilder(new IdentityDocument
        {
            Id = id
        });

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

        public IdentityDocumentBuilder AddJsonWebKeyVerificationMethod(ISignatureKey signatureKey, string controller, IdentityDocumentVerificationMethodUsages usage)
            => AddVerificationMethod(signatureKey, controller, JWKVerificationMethodBuilder.JSON_LD_CONTEXT, usage);

        public IdentityDocumentBuilder AddPublicKeyMultibaseVerificationMethod(ISignatureKey signatureKey, string controller, IdentityDocumentVerificationMethodUsages usage)
            => AddVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodBuilder.JSON_LD_CONTEXT, usage);

        public IdentityDocumentBuilder AddJsonWebKeyAuthentication(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, JWKVerificationMethodBuilder.JSON_LD_CONTEXT, IdentityDocumentVerificationMethodUsages.AUTHENTICATION);

        public IdentityDocumentBuilder AddPublicKeyMultibaseAuthentication(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodBuilder.JSON_LD_CONTEXT, IdentityDocumentVerificationMethodUsages.AUTHENTICATION);

        public IdentityDocumentBuilder AddJsonWebKeyAssertionMethod(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, JWKVerificationMethodBuilder.JSON_LD_CONTEXT, IdentityDocumentVerificationMethodUsages.ASSERTION_METHOD);

        public IdentityDocumentBuilder AddPublicKeyMultibaseAssertionMethod(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodBuilder.JSON_LD_CONTEXT, IdentityDocumentVerificationMethodUsages.ASSERTION_METHOD);

        public IdentityDocumentBuilder AddJsonWebKeyAgreement(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, JWKVerificationMethodBuilder.JSON_LD_CONTEXT, IdentityDocumentVerificationMethodUsages.KEY_AGREEMENT);

        public IdentityDocumentBuilder AddPublicKeyMultibaseAgreement(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodBuilder.JSON_LD_CONTEXT, IdentityDocumentVerificationMethodUsages.KEY_AGREEMENT);

        public IdentityDocumentBuilder AddJsonWebKeyCapabilityInvocation(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, JWKVerificationMethodBuilder.JSON_LD_CONTEXT, IdentityDocumentVerificationMethodUsages.CAPABILITY_INVOCATION);

        public IdentityDocumentBuilder AddPublicKeyMultibaseCapabilityInvocation(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodBuilder.JSON_LD_CONTEXT, IdentityDocumentVerificationMethodUsages.CAPABILITY_INVOCATION);

        public IdentityDocumentBuilder AddJsonWebKeyCapabilityDelegation(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, JWKVerificationMethodBuilder.JSON_LD_CONTEXT, IdentityDocumentVerificationMethodUsages.CAPABILITY_DELEGATION);

        public IdentityDocumentBuilder AddPublicKeyMultibaseCapabilityDelegation(ISignatureKey signatureKey, string controller)
            => AddInnerVerificationMethod(signatureKey, controller, PublicKeyMultibaseVerificationMethodBuilder.JSON_LD_CONTEXT, IdentityDocumentVerificationMethodUsages.CAPABILITY_DELEGATION);

        #endregion

        #region Service endpoint
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

        #endregion

        public IdentityDocument Build()
        {
            _identityDocument.Context = BuildContext();
            _identityDocument.Controller = BuildController();
            _identityDocument.Authentication = BuildInnerVerificationMethods(IdentityDocumentVerificationMethodUsages.AUTHENTICATION);
            _identityDocument.AssertionMethod = BuildInnerVerificationMethods(IdentityDocumentVerificationMethodUsages.ASSERTION_METHOD);
            _identityDocument.KeyAgreement = BuildInnerVerificationMethods(IdentityDocumentVerificationMethodUsages.KEY_AGREEMENT);
            _identityDocument.CapabilityInvocation = BuildInnerVerificationMethods(IdentityDocumentVerificationMethodUsages.CAPABILITY_INVOCATION);
            _identityDocument.CapabilityDelegation = BuildInnerVerificationMethods(IdentityDocumentVerificationMethodUsages.CAPABILITY_DELEGATION);
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

        private IdentityDocumentBuilder AddVerificationMethod(ISignatureKey signatureKey, string controller, string ldContext, IdentityDocumentVerificationMethodUsages usage)
        {
            var verificationMethod = BuildVerificationMethod(signatureKey, controller, ldContext);
            verificationMethod.Usage = usage;
            _identityDocument.AddVerificationMethod(verificationMethod);
            return this;
        }

        private IdentityDocumentBuilder AddInnerVerificationMethod(ISignatureKey signatureKey, string controller, string ldContext, IdentityDocumentVerificationMethodUsages usage)
        {
            if(!_innerVerificationMethods.ContainsKey(usage))
            {
                _innerVerificationMethods.Add(usage, new List<IdentityDocumentVerificationMethod>());
            }

            var verificationMethod = BuildVerificationMethod(signatureKey, controller, ldContext);
            _innerVerificationMethods[usage].Add(verificationMethod);
            return this;
        }

        private IdentityDocumentVerificationMethod BuildVerificationMethod(ISignatureKey signatureKey, string controller, string ldContext)
        {
            var builder = _verificationMethodBuilders.Single(v => v.JSONLDContext == ldContext);
            var verificationMethod = builder.Build(_identityDocument, signatureKey);
            verificationMethod.Type = builder.Type;
            verificationMethod.Controller = controller;
            AddContext(builder.JSONLDContext);
            return verificationMethod;
        }

        private JsonArray BuildInnerVerificationMethods(IdentityDocumentVerificationMethodUsages usage)
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