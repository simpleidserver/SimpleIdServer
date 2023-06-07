// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    [Authorize(Constants.Policies.Authenticated)]
    public class DeviceController : Controller
    {
        private readonly IDeviceAuthCodeRepository _deviceAuthCodeRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IBusControl _busControl;

        public DeviceController(IDeviceAuthCodeRepository deviceAuthCodeRepository, IClientRepository clientRepository, IBusControl busControl)
        {
            _deviceAuthCodeRepository = deviceAuthCodeRepository;
            _clientRepository = clientRepository;
            _busControl = busControl;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] string prefix, string userCode, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(userCode))
            {
                var deviceAuthCode = await _deviceAuthCodeRepository.Query().Include(c => c.Client).ThenInclude(c => c.Scopes).SingleAsync(a => a.UserCode == userCode, cancellationToken);
                return View(BuildViewModel(userCode, deviceAuthCode));
            }

            return View(new DeviceCodeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromRoute] string prefix, ConfirmDeviceCodeViewModel viewModel, CancellationToken cancellationToken)
        {
            var deviceAuthCode = await _deviceAuthCodeRepository.Query().Include(c => c.Client).ThenInclude(c => c.Scopes).SingleOrDefaultAsync(v => v.UserCode == viewModel.UserCode, cancellationToken);
            if (deviceAuthCode == null)
            {
                ModelState.AddModelError("unknown_user_code", "unknown_user_code");
                return View(new DeviceCodeViewModel
                {
                    UserCode = viewModel.UserCode
                });
            }

            if (deviceAuthCode.Status != Domains.DeviceAuthCodeStatus.PENDING)
            {
                ModelState.AddModelError("not_pending_auth_device_code", "not_pending_auth_device_code");
                return View(BuildViewModel(viewModel.UserCode, deviceAuthCode));
            }

            if (deviceAuthCode.ExpirationDateTime <= DateTime.UtcNow)
            {
                ModelState.AddModelError("auth_device_code_expired", "auth_device_code_expired");
                return View(BuildViewModel(viewModel.UserCode, deviceAuthCode));
            }

            var nameIdentifier = GetNameIdentifier();
            deviceAuthCode.Accept(nameIdentifier);
            await _deviceAuthCodeRepository.SaveChanges(cancellationToken);
            var result = BuildViewModel(viewModel.UserCode, deviceAuthCode);
            result.IsConfirmed = true;
            return View(result);
        }

        private static DeviceCodeViewModel BuildViewModel(string userCode, DeviceAuthCode deviceAuthCode)
        {
            var result = new DeviceCodeViewModel
            {
                UserCode = userCode
            };

            if(deviceAuthCode != null)
            {
                result.ClientName = deviceAuthCode.Client?.ClientName;
                result.PictureUri = deviceAuthCode.Client?.LogoUri;
                result.Scopes = deviceAuthCode.Client?.Scopes.Where(s => deviceAuthCode.Scopes.Contains(s.Name)).Select(s => s.Name);
            }

            return result;
        }

        private string GetNameIdentifier()
        {
            var claimName = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier);
            return claimName.Value;
        }
    }
}
