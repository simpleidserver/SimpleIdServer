// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Scim.Parser.Expressions
{
    public abstract class SCIMExpression : ICloneable
    {
        public abstract object Clone();
    }
}