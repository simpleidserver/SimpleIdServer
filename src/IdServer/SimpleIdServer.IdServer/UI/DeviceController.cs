// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    [Authorize(Constants.Policies.Authenticated)]
    public class DeviceController : Controller
    {
        private readonly IDeviceAuthCodeRepository _deviceAuthCodeRepository;

        public DeviceController(IDeviceAuthCodeRepository deviceAuthCodeRepository)
        {
            _deviceAuthCodeRepository = deviceAuthCodeRepository;
        }

        public async Task<IActionResult> Index([FromRoute] string prefix, string userCode, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(userCode))
            {
                var deviceAuthCode = await _deviceAuthCodeRepository.Query().Include(c => c.Client).ThenInclude(c => c.Scopes).SingleOrDefaultAsync(a => a.UserCode == userCode, cancellationToken);
                if (deviceAuthCode != null)
                    return View(new DeviceCodeViewModel
                    {
                        ClientName = deviceAuthCode.Client.ClientName,
                        PictureUri = deviceAuthCode.Client.LogoUri,
                        Scopes = deviceAuthCode.Client.Scopes.Where(s => deviceAuthCode.Scopes.Contains(s.Name)).Select(s => s.Name),
                        UserCode = userCode
                    });
            }

            return View(new DeviceCodeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromRoute] string prefix, ConfirmDeviceCodeViewModel viewModel, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            return null;
        }
    }
}
