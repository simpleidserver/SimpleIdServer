// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdServerBuilderExtensions
    {
        public static IdServerBuilder AddOpenIdProtocol(this IdServerBuilder serverBuilder)
        {
            return serverBuilder;
        }
    }
}
