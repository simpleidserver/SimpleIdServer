// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml.Infrastructures;

public class DynamicSamlAuthenticationHandlerProvider : IAuthenticationHandlerProvider
{
    private readonly Dictionary<string, IAuthenticationHandler> _handlerMap = new Dictionary<string, IAuthenticationHandler>(StringComparer.Ordinal);
    private readonly ISamlAuthenticationSchemeProvider _authenticationSchemeProvider;

    public DynamicSamlAuthenticationHandlerProvider(ISamlAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    public async Task<IAuthenticationHandler> GetHandlerAsync(HttpContext context, string authenticationScheme)
    {
        if (_handlerMap.TryGetValue(authenticationScheme, out var value))
            return value;

        var scheme = await _authenticationSchemeProvider.GetSamlSchemeAsync(authenticationScheme);
        if (scheme == null) return null;
        var handlerType = scheme.AuthScheme.HandlerType;
        var handler = context.RequestServices.GetService(handlerType) as IAuthenticationHandler;
        if (handler == null)
        {
            var ctr = handlerType.GetConstructors().First();
            var args = new List<object>();
            foreach (var par in ctr.GetParameters())
            {
                if (par.ParameterType.IsGenericType && par.ParameterType.GetGenericTypeDefinition() == typeof(IOptionsMonitor<>).GetGenericTypeDefinition())
                    args.Add(scheme.SamlSpOptions);
                else
                    args.Add(context.RequestServices.GetService(par.ParameterType));
            }

            handler = Activator.CreateInstance(handlerType, args.ToArray()) as IAuthenticationHandler;
        }

        if (handler != null)
        {
            await handler.InitializeAsync(scheme.AuthScheme, context);
            _handlerMap[authenticationScheme] = handler;
        }

        return handler;
    }
}
