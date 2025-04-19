// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Website;
using SimpleIdServer.IdServer.Website.Infrastructures;
using SimpleIdServer.IdServer.Website.Middlewares;
using System.Globalization;

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

        UseRequestLocalization(builder, options);
        builder.UseCookiePolicy();
        builder.UseAuthentication();
        builder.UseAuthorization();
        builder.MapControllers();
        return builder;
    }

    private static async void UseRequestLocalization(this WebApplication webApplication, IdServerWebsiteOptions options)
    {
        var locOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(SimpleIdServer.IdServer.Constants.DefaultLanguage),
            SupportedCultures = new[] { new CultureInfo(SimpleIdServer.IdServer.Constants.DefaultLanguage) },
            SupportedUICultures = new[] { new CultureInfo(SimpleIdServer.IdServer.Constants.DefaultLanguage) }
        };
        var store = webApplication.Services.GetRequiredService<ILanguageStore>(); 
        store.LanguagesUpdated += codes =>
        {
            var cultures = codes
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => new CultureInfo(c.Trim()))
                .ToList();

            if (cultures.Any())
            {
                locOptions.SupportedCultures = cultures;
                locOptions.SupportedUICultures = cultures;
            }
        };

        await store.InitialLoadAsync();
        webApplication.UseRequestLocalization(locOptions);
    }
}
