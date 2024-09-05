// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.FastFed.ApplicationProvider.UIs.ViewModels;

public class ConfirmIdentityProviderViewModel
{
    public ConfirmIdentityProviderViewModel(string errorCode, string errorDescription)
    {
        ErrorCode = errorCode;
        ErrorDescription = errorDescription;
    }

    public string ErrorCode { get; set; }
    public string ErrorDescription { get; set; }
}
