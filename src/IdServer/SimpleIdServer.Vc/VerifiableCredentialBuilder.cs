// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Vc
{
    public class VerifiableCredentialBuilder
    {
        private readonly VerifiableCredential _vc;

        private VerifiableCredentialBuilder(VerifiableCredential vc) 
        {
            _vc = vc;
        }

        public static VerifiableCredentialBuilder New() 
        {
            return new VerifiableCredentialBuilder(new VerifiableCredential
            {
                Context = new List<string> { Constants.DefaultVerifiableCredentialContext },
                Type = new List<string> { Constants.DefaultVerifiableCredentialType }
            });
        }

        public VerifiableCredentialBuilder AddContext(string ctx)
        {
            _vc.Context.Add(ctx);
            return this;
        }

        public VerifiableCredentialBuilder AddType(string type)
        {
            _vc.Type.Add(type);
            return this;
        }

        public VerifiableCredentialBuilder SetCredentialSubject(string json) => SetCredentialSubject(JsonObject.Parse(json).AsObject());

        public VerifiableCredentialBuilder SetCredentialSubject(JsonObject jsonObj)
        {
            _vc.CredentialSubject = jsonObj;
            return this;
        }

        public VerifiableCredentialBuilder SetIssuer(string issuer)
        {
            _vc.Issuer = issuer;
            return this;
        }

        public VerifiableCredentialBuilder SetIssuanceDate(DateTime issuanceDate)
        {
            _vc.IssuanceDate = issuanceDate;
            return this;
        }

        public VerifiableCredential Build() => _vc;
    }
}
