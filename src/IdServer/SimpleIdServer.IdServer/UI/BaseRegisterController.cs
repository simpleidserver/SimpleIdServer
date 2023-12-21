// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI;

public abstract class BaseRegisterController<TViewModel> : BaseController where TViewModel : IRegisterViewModel
{
    public BaseRegisterController(
        IOptions<IdServerHostOptions> options, 
        IDistributedCache distributedCache,
        IUserRepository userRepository,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        Options = options.Value;
        DistributedCache = distributedCache;
        UserRepository = userRepository;
    }

    protected IdServerHostOptions Options { get; }
    protected IDistributedCache DistributedCache { get; }
    protected IUserRepository UserRepository { get; }

    protected async Task<UserRegistrationProgress> GetRegistrationProgress()
    {
        var cookieName = Options.GetRegistrationCookieName();
        if (!Request.Cookies.ContainsKey(cookieName)) return null;
        var cookieValue = Request.Cookies[cookieName];
        var json = await DistributedCache.GetStringAsync(cookieValue);
        if (string.IsNullOrWhiteSpace(json)) return null;
        var registrationProgress = JsonConvert.DeserializeObject<UserRegistrationProgress>(json);
        return registrationProgress;
    }

    protected async Task<IActionResult> CreateUser(UserRegistrationProgress registrationProgress, TViewModel viewModel, string prefix, string amr)
    {
        var user = registrationProgress.User ?? new Domains.User
        {
            Id = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
        EnrichUser(user, viewModel);
        var lastStep = registrationProgress.Steps.Last();
        if(lastStep == amr)
        {
            user.Realms.Add(new Domains.RealmUser
            {
                RealmsName = prefix
            });
            UserRepository.Add(user);
            await UserRepository.SaveChanges(CancellationToken.None);
            viewModel.IsUpdated = true;
            viewModel.RedirectUrl = registrationProgress.RedirectUrl;
            return View(viewModel);
        }

        registrationProgress.NextAmr();
        registrationProgress.User = user;
        var json = JsonConvert.SerializeObject(registrationProgress);
        await DistributedCache.SetStringAsync(registrationProgress.RegistrationProgressId, json);
        return RedirectToAction("Index", "Register", new { area = registrationProgress.Amr });
    }

    protected async Task<IActionResult> UpdateUser(UserRegistrationProgress registrationProgress, TViewModel viewModel, string amr)
    {
        var lastStep = registrationProgress?.Steps?.Last();
        if (lastStep == amr || registrationProgress == null)
        {
            viewModel.IsUpdated = true;
            viewModel.RedirectUrl = registrationProgress.RedirectUrl;
            return View(viewModel);
        }

        registrationProgress.NextAmr();
        var json = JsonConvert.SerializeObject(registrationProgress);
        await DistributedCache.SetStringAsync(registrationProgress.RegistrationProgressId, json);
        return RedirectToAction("Index", "Register", new { area = registrationProgress.Amr });
    }

    protected abstract void EnrichUser(User user, TViewModel viewModel);
}