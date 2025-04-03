// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Website.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.Website;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Middlewares;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseIdserverAdmin(this WebApplication builder)
    {
        var options = builder.Services.GetRequiredService<IOptions<IdServerWebsiteOptions>>().Value;
        builder.UseStaticFiles();
        builder.UseMiddleware<RealmMiddleware>();
        if (options.ForceHttps)
        {
            builder.UseMiddleware<HttpsMiddleware>();
        }

        UseRequestLocalization(builder);
        builder.UseRouting();
        builder.UseCookiePolicy();
        builder.UseAuthentication();
        builder.UseAuthorization();
        builder.MapBlazorHub();
        builder.MapFallbackToPage("/_Host");
        builder.MapControllers();
        return builder;
    }

    private static async void UseRequestLocalization(this WebApplication webApplication)
    {
        var factory = webApplication.Services.GetRequiredService<IWebsiteHttpClientFactory>();
        using (var httpClient = await factory.Build())
        {
            var url = $"{options.Issuer}/languages";
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            if (!httpResult.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Cannot retrieve the languages from {url}");
            }

            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync();
            var languages = SidJsonSerializer.Deserialize<List<Language>>(json);
            var languageCodes = languages.Select(l => l.Code).ToArray();
            webApplication.UseRequestLocalization(e =>
            {
                e.SetDefaultCulture(options.DefaultLanguage);
                e.AddSupportedCultures(languageCodes);
                e.AddSupportedUICultures(languageCodes);
            });
        }
    }
}
