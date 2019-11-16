// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Scim.Exceptions
{
    public class SCIMBadRequestException : BaseScimException
    {
        public SCIMBadRequestException(string code, string message) : base(code, message) { }
    }
}
