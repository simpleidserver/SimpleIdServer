// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Common.Exceptions
{
    public class BaseUIException : Exception
    {
        public BaseUIException(string code) : base(string.Empty)
        {
            Code = code;
        }

        public string Code { get; set; }
    }
}
