// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Captcha;

public class V2ReCaptchaValidator : ICaptchaValidator
{
    private readonly V2ReCaptchaOptions _options;
    private readonly Helpers.IHttpClientFactory _httpClientFactory;

    public V2ReCaptchaValidator(
        IOptions<V2ReCaptchaOptions> options, 
        Helpers.IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _httpClientFactory = httpClientFactory;
    }

    public string Type => "V2ReCaptcha";

    public async Task<bool> Validate<T>(T request, CancellationToken cancellationToken) where T : ISidStepViewModel
    {
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            var url = "https://www.google.com/recaptcha/api/siteverify";
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"secret", _options.Secret },
                {"response", request.CaptchaValue}
            });
            var response = await httpClient.PostAsync(url, content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var verificationResult = await response.Content.ReadFromJsonAsync<V2ReCaptchaVerificationResult>(cancellationToken);
            return verificationResult.Success;
        }
    }
}
