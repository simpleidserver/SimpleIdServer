// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Captcha;

public interface ICaptchaValidatorFactory
{
    Task<bool> Validate<T>(T type, CancellationToken cancellationToken) where T : ISidStepViewModel;
}

public class CaptchaValidatorFactory : ICaptchaValidatorFactory
{
    private readonly IEnumerable<ICaptchaValidator> _validators;

    public CaptchaValidatorFactory(IEnumerable<ICaptchaValidator> validators)
    {
        _validators = validators;
    }

    public async Task<bool> Validate<T>(T type, CancellationToken cancellationToken) where T : ISidStepViewModel
    {
        var validator = _validators.SingleOrDefault(v => v.Type == type.CaptchaType);
        if(validator == null)
        {
            return true;
        }

        return await validator.Validate(type, cancellationToken);
    }
}
