// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Scim.Exceptions
{
    public class SCIMAttributeException : BaseScimException
    {
        public SCIMAttributeException(string message) : base("badAttribute", message) { }
    }
}
