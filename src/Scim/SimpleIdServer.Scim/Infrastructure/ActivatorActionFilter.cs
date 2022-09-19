// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Infrastructure
{
    public class ActivatorActionFilter : ActionFilterAttribute
    {
        private readonly bool _isEnabled = true;

        public ActivatorActionFilter(IOptions<SCIMHostOptions> options, string propertyName)
        {
            var propertyInfo = typeof(SCIMHostOptions).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if(propertyInfo != null)
            {
                var val = propertyInfo.GetValue(options.Value);
                if (val != null && val.GetType() == typeof(bool)) _isEnabled = (bool)val;
            }
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!_isEnabled)
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.NotFound);
                return;
            }

            await next();
        }
    }
}
