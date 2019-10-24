// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Uma.Exceptions
{
    public class BaseUMAException : Exception
    {
        public BaseUMAException() { }

        public BaseUMAException(string message) : base(message) { }
    }
}
