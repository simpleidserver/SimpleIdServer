// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using System.Xml.Linq;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class SqlSugarXmlRepository : IXmlRepository
{
    private readonly IServiceProvider _serviceProvider;

    public SqlSugarXmlRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IReadOnlyCollection<XElement> GetAllElements()
    {
        return GetAllElementsCore().ToList().AsReadOnly();
        IEnumerable<XElement> GetAllElementsCore()
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            var requiredService = scope.ServiceProvider.GetRequiredService<DbContext>();
            foreach (SugarDataProtection item in requiredService.Client.Queryable<SugarDataProtection>().ToList())
            {
                if (!string.IsNullOrEmpty(item.Xml))
                {
                    yield return XElement.Parse(item.Xml);
                }
            }
        }
    }

    public void StoreElement(XElement element, string friendlyName)
    {
        using IServiceScope serviceScope = _serviceProvider.CreateScope();
        var requiredService = serviceScope.ServiceProvider.GetRequiredService<DbContext>();
        SugarDataProtection entity = new SugarDataProtection
        {
            FriendlyName = friendlyName,
            Xml = element.ToString(SaveOptions.DisableFormatting)
        };
        requiredService.Client.Insertable(entity).ExecuteCommand();
    }
}