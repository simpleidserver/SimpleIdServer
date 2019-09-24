// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Jwt.Jws
{
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

        public string Type { get; set; }
        public string Alg { get; set; }
        public string Kid { get; set; }
    }
}