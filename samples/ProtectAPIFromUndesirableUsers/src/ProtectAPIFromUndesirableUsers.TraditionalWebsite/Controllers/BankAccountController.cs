// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtectAPIFromUndesirableUsers.TraditionalWebsite.ViewModels;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProtectAPIFromUndesirableUsers.TraditionalWebsite.Controllers
{
    [Authorize("Authenticated")]
    public class BankAccountController : Controller
    {
        public async Task<IActionResult> Index()
        {
            string idToken = await HttpContext.GetTokenAsync("id_token");
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new System.Uri($"{TraditionalWebsiteConstants.BASE_API_URI}/bankaccounts/.me")
                };
                request.Headers.Add("Authorization", $"Bearer {idToken}");
                var httpResult = await httpClient.SendAsync(request);
                var json = await httpResult.Content.ReadAsStringAsync();
                return View(new BankAccountViewModel { Content = json });
            }
        }
    }
}
