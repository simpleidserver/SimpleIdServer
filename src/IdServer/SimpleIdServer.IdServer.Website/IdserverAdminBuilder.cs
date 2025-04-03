// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.IdServer.Website;

public class IdserverAdminBuilder
{
    private readonly IServiceCollection _services;
    private readonly IDataProtectionBuilder _dataprotectionBuilder;
    private readonly AdminOpenidAuth _adminOpenidAuth;
    private readonly AdminCookieAuth _adminCookieAuth;
    private readonly AdminAuthz _adminAuthz;

    internal IdserverAdminBuilder(IServiceCollection services, IDataProtectionBuilder dataprotectionBuilder, AdminOpenidAuth adminOpenidAuth, AdminCookieAuth adminCookieAuth, AdminAuthz adminAuthz)
    {
        _services = services;
        _dataprotectionBuilder = dataprotectionBuilder;
        _adminOpenidAuth = adminOpenidAuth;
        _adminCookieAuth = adminCookieAuth;
        _adminAuthz = adminAuthz;
    }

    public IdserverAdminBuilder PersistDataprotection(string directoryPath)
    {
        _dataprotectionBuilder.PersistKeysToFileSystem(new DirectoryInfo(directoryPath));
        return this;
    }

    public IdserverAdminBuilder ForceHttps()
    {
        _services.Configure<IdServerWebsiteOptions>(o =>
        {
            o.ForceHttps = true;
        });
        return this;
    }

    public IdserverAdminBuilder EnableRealm()
    {
        _adminOpenidAuth.UseRealm = true;
        _services.Configure<IdServerWebsiteOptions>(o =>
        {
            o.IsReamEnabled = true;
        });
        return this;
    }
}
