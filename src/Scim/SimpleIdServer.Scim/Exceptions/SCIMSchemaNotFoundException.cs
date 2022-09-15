// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains.Exceptions;
using System;

namespace SimpleIdServer.Scim.Exceptions
{
    public class SCIMSchemaNotFoundException : BaseScimException
    {
        public SCIMSchemaNotFoundException() : base(String.Empty)
        {
        }
    }
}
