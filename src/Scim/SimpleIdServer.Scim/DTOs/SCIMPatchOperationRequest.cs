// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Scim.DTOs
{
    public class SCIMPatchOperationRequest
    {
        public SCIMPatchOperationRequest(SCIMPatchOperations operation, string path, object value = null)
        {
            Operation = operation;
            Path = path;
            Value = value;
        }

        public SCIMPatchOperations Operation { get; set; }
        public string Path { get; set; }
        public object Value { get; set; }
    }
}
