// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.Models;
using System.Collections.Generic;

namespace SimpleIdServer.Vc
{
    public class VerifiablePresentationBuilder
    {
        private readonly VerifiablePresentation _verifiablePresentation;

        private VerifiablePresentationBuilder(VerifiablePresentation verifiablePresentation) 
        { 
            _verifiablePresentation = verifiablePresentation;
        }

        public static VerifiablePresentationBuilder New()
        {
            return new VerifiablePresentationBuilder(new VerifiablePresentation
            {
                Context = new List<string> { Constants.DefaultVerifiableCredentialContext },
                Type = new List<string> { Constants.DefaultVerifiablePresentationType }
            });
        }

        public VerifiablePresentationBuilder AddContext(string ctx)
        {
            _verifiablePresentation.Context.Add(ctx);
            return this;
        }

        public VerifiablePresentationBuilder AddType(string type)
        {
            _verifiablePresentation.Type.Add(type);
            return this;
        }

        public VerifiablePresentationBuilder AddVerifiableCredential(VerifiableCredential credential)
        {
            _verifiablePresentation.VerifiableCredentials.Add(credential);
            return this;
        }

        public VerifiablePresentation Build() => _verifiablePresentation;
    }
}
