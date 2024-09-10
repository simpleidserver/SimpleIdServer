// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

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
        ErrorDescriptions = new List<string> { errorDescription };
    }

    public ValidationResult(string errorCode, List<string> errorDescriptions)
    {
        ErrorCode = errorCode;
        ErrorDescriptions = errorDescriptions;
    }

    public T Result { get; private set; }
    public string ErrorCode { get; private set; }
    public List<string> ErrorDescriptions { get; private set; }
    public bool HasError
    {
        get
        {
            return !string.IsNullOrWhiteSpace(ErrorCode);
        }
    }

    public static ValidationResult<T> Ok(T content) => new ValidationResult<T>(content);

    public static ValidationResult<T> Fail(string errorCode, string errorDescription) => new ValidationResult<T>(errorCode, errorDescription);

    public static ValidationResult<T> Fail(string errorCode, List<string> errorDescriptions) => new ValidationResult<T>(errorCode, errorDescriptions);
}
