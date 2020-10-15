// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Infrastructure.ValueProviders;

namespace Microsoft.AspNetCore.Mvc
{
    public static class MvcOptionsExtensions
    {
        public static MvcOptions AddSCIMValueProviders(this MvcOptions mvcOptions)
        {
            mvcOptions.ValueProviderFactories.Insert(0, new SeparatedQueryStringValueProviderFactory(","));
            return mvcOptions;
        }
    }
}
