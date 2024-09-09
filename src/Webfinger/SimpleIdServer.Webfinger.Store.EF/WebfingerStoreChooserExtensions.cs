// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Webfinger.Models;
using SimpleIdServer.Webfinger.Store.EF;
using SimpleIdServer.Webfinger.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Webfinger;

public static class WebfingerStoreChooserExtensions
{
    public static void UseEfStore(this WebfingerStoreChooser chooser, Action<DbContextOptionsBuilder> dbCallback = null)
    {
        if (dbCallback != null) chooser.Services.AddDbContext<WebfingerDbContext>(dbCallback);
        else chooser.Services.AddDbContext<WebfingerDbContext>(o => o.UseInMemoryDatabase("webfinger"));
        chooser.Services.AddTransient<IWebfingerResourceStore, WebfingerResourceStore>();
    }

    public static void UseInMemoryEfStore(this WebfingerStoreChooser chooser, params WebfingerResource[] webfingerResources)
        => UseInMemoryEfStore(chooser, webfingerResources.ToList());

    public static void UseInMemoryEfStore(this WebfingerStoreChooser chooser, List<WebfingerResource> webfingerResources)
    {
        UseEfStore(chooser);
        using (var sp = chooser.Services.BuildServiceProvider())
        {
            using (var scope = sp.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetService<WebfingerDbContext>())
                {
                    dbContext.WebfingerResources.AddRange(webfingerResources);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
