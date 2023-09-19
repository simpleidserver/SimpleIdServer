// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI;

[Area(Constants.Areas.Password)]
public class RegisterController : BaseController
{
    private readonly IUserRepository _userRepository;
    private readonly IRealmRepository _realmRepository;

    public RegisterController(IUserRepository userRepository, IRealmRepository realmRepository)
    {
        _userRepository = userRepository;
        _realmRepository = realmRepository;
    }

    [HttpGet]
    public IActionResult Index([FromRoute] string prefix)
    {
        prefix = prefix ?? Constants.Prefix;
        var viewModel = new PwdRegisterViewModel();
        if(User.Identity.IsAuthenticated)
        {
            var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            viewModel.Login = nameIdentifier;
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromRoute] string prefix, PwdRegisterViewModel viewModel)
    {
        prefix = prefix ?? Constants.Prefix;
        viewModel.Validate(ModelState);
        if (!ModelState.IsValid) return View(viewModel);
        if (!User.Identity.IsAuthenticated) return await CreateUser();
        return await UpdateUser();

        async Task<IActionResult> CreateUser()
        {
            var userExists = await _userRepository.Query().Include(u => u.Realms).AsNoTracking().AnyAsync(u => u.Name == viewModel.Login && u.Realms.Any(r => r.RealmsName == prefix));
            if(userExists)
            {
                ModelState.AddModelError("user_exists", "user_exists");
                return View(viewModel);
            }

            var realm = await _realmRepository.Query().SingleAsync(r => r.Name == prefix);
            var user = UserBuilder.Create(viewModel.Login, viewModel.Password, realm: realm)
                .Build();
            _userRepository.Add(user);
            await _userRepository.SaveChanges(CancellationToken.None);
            viewModel.IsUpdated = true;
            return View(viewModel);
        }

        async Task<IActionResult> UpdateUser()
        {
            var user = await _userRepository.Query().Include(u => u.Credentials).SingleAsync(u => u.Name == viewModel.Login);
            var passwordCredential = user.Credentials.FirstOrDefault(c => c.CredentialType == UserCredential.PWD);
            if (passwordCredential != null) passwordCredential.Value = viewModel.Password;
            else user.Credentials.Add(new UserCredential
            {
                Id = Guid.NewGuid().ToString(),
                Value = viewModel.Password,
                CredentialType = UserCredential.PWD,
                IsActive = true
            });
            await _userRepository.SaveChanges(CancellationToken.None);
            return View(viewModel);
        }
    }
}