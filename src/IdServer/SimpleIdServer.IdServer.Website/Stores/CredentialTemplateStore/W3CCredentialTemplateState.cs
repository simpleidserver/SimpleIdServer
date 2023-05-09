// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.Vc.Models;

namespace SimpleIdServer.IdServer.Website.Stores.CredentialTemplateStore
{
    [FeatureState]
    public record W3CCredentialTemplateState
    {
        public W3CCredentialTemplateState()
        {

        }

        public string TechnicalId { get; set; }
        public bool IsLoading { get; set; }
        public IEnumerable<string> Types { get; set; }
        public ICollection<SelectableW3CCredentialSubjectRecord> CredentialSubjects { get; set; }
        public string ConcatenatedTypes
        {
            get
            {
                return Types == null ? null : string.Join(";", Types);
            }
        }
        public int Count { get; set; }
    }

    public class SelectableW3CCredentialSubjectRecord
    {
        public SelectableW3CCredentialSubjectRecord(string parameterId, string claimName, W3CCredentialSubject credentialSubject)
        {
            ParameterId = parameterId;
            ClaimName = claimName;
            CredentialSubject = credentialSubject;
        }

        public bool IsSelected { get; set; }
        public bool IsNew { get; set; }
        public string ParameterId { get; set; }
        public string ClaimName { get; set; }
        public W3CCredentialSubject CredentialSubject { get; set; }
    }
}
