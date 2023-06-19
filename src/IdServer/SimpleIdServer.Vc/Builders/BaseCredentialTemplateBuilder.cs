// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.Models;

namespace SimpleIdServer.Vc.Builders
{
    public class BaseCredentialTemplateBuilder<T> where T : BaseCredentialTemplate
    {
        private readonly T _credentialTemplate;

        protected BaseCredentialTemplateBuilder(T credentialTemplate)
        {
            _credentialTemplate = credentialTemplate;
        }

        protected T CredentialTemplate => _credentialTemplate;

        public T Build() => _credentialTemplate;
    }
}
