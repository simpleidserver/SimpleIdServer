// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains;

public class GotifySession
{
    public string ApplicationToken { get; set; } = null!;
    public string ClientToken { get; set; } = null!;
}
