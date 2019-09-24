// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;

namespace SimpleIdServer.OAuth.Infrastructures
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseModule(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseAuthentication();
            applicationBuilder.UseStaticFiles();
            applicationBuilder.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "AreaRoute",
                  template: "{area}/{controller}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "DefaultRoute",
                    template: "{controller}/{action}/{id?}");
            });
            return applicationBuilder;
        }
    }
}