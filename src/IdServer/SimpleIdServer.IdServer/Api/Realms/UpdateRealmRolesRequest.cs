// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Api.Realms;

public class UpdateRealmRolesRequest
{
    public List<string> ScopeIds {  get; set; }
}
