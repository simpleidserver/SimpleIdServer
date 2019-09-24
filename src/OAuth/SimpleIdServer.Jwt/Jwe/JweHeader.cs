// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Runtime.Serialization;

namespace SimpleIdServer.Jwt.Jwe
{
    [DataContract]
    public class JweHeader
    {
        public JweHeader() { }

        public JweHeader(string alg, string enc, string kid)
        {
            Alg = alg;
            Enc = enc;
            Kid = kid;
        }

        [DataMember(Name = "alg")]
        public string Alg { get; set; }
        [DataMember(Name = "enc")]
        public string Enc { get; set; }
        [DataMember(Name = "kid")]
        public string Kid { get; set; }
    }
}
