// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Net;

namespace SimpleIdServer.Scim.Infrastructure
{
    public class GenericResult<T>
    {
        private GenericResult(T result)
        {
            Result = result;
            HasError = false;
        }

        private GenericResult(HttpStatusCode statusCode, string errorMessage, string scimType)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
            ScimType = scimType;
            HasError = true;
        }

        public T Result { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public string ScimType { get; private set; }
        public bool HasError { get; private set; }

        public static GenericResult<T> Ok(T result) => new GenericResult<T>(result);

        public static GenericResult<T> Error(HttpStatusCode statusCode, string errorMessage, string scimType) => new GenericResult<T>(statusCode, errorMessage, scimType);
    }
}
