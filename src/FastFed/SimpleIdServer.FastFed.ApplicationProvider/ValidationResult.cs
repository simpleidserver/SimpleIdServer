// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.FastFed.ApplicationProvider;

public class ValidationResult<T>
{
    public ValidationResult(T result)
    {
        Result = result;
    }

    public ValidationResult(string errorCode, string errorDescription)
    {
        ErrorCode = errorCode;
        ErrorDescription = errorDescription;
    }

    public T Result { get; private set; }
    public string ErrorCode { get; private set; }
    public string ErrorDescription { get; private set; }
    public bool HasError
    {
        get
        {
            return !string.IsNullOrWhiteSpace(ErrorCode);
        }
    }

    public static ValidationResult<T> Ok(T content) => new ValidationResult<T>(content);

    public static ValidationResult<T> Fail(string errorCode, string errorDescription) => new ValidationResult<T>(errorCode, errorDescription);
}
