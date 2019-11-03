// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Runtime.Serialization;

namespace SimpleIdServer.Jwt.Jws
{
    [DataContract]
    public class JwsHeader
    {
        public JwsHeader() { }

        public JwsHeader(string type, string alg)
        {
            Type = type;
            Alg = alg;
        }

        public JwsHeader(string type, string alg, string kid) : this(type, alg)
        {
            Kid = kid;
        }

        [DataMember(Name = "typ")]
        public string Type { get; set; }
        [DataMember(Name = "alg")]
        public string Alg { get; set; }
        [DataMember(Name = "kid")]
        public string Kid { get; set; }
    }
}