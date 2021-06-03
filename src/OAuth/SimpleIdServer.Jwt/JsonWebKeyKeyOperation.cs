// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Jwt
{
    public class JsonWebKeyKeyOperation : ICloneable
    {
        public KeyOperations Operation { get; set; }

        public object Clone()
        {
            return new JsonWebKeyKeyOperation
            {
                Operation = Operation
            };
        }
    }
}
