// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.TokenTypes;

public interface ITokenTypeService
{
    string Name { get; }
    string TokenType { get; }
    TokenResult Parse(string realm, string token);
    Task<string> Build(string realm, string issuer, Client client, Dictionary<string, object> claims, CancellationToken cancellationToken);
}

public record TokenResult
{
    public string Subject { get; set; }
    public Dictionary<string, object> Claims { get; set; }
}