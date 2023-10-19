// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.TokenTypes;

public interface ITokenTypeParser
{
    string Name { get; }
    Dictionary<string, string> Parse(string realm, string token);
}
