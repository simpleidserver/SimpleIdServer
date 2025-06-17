// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Captcha;

public interface ICaptchaValidator
{
    string Type
    {
        get;
    }

    Task<bool> Validate<T>(T request, CancellationToken cancellationToken) where T : ISidStepViewModel;
}
