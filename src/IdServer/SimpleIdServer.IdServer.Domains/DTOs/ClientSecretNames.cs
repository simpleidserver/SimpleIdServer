// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.Contracts;

namespace SimpleIdServer.IdServer.Domains.DTOs;

public class ClientSecretNames
{
    public const string Alg = "alg";
    public const string Value = "value";
    public const string Id = "id";
    public const string ExpirationDateTime = "expiration_date_time";
    public const string CreateDateTime = "create_date_time";
    public const string IsActive = "is_active";
}
