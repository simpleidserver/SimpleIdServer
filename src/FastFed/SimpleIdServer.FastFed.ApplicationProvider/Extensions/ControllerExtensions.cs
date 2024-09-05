// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Microsoft.AspNetCore.Mvc;

public static class ControllerExtensions
{
    public static string GetAbsoluteUriWithVirtualPath(this Controller controller)
    {
        var requestMessage = controller.Request;
        var host = requestMessage.Host.Value;
        var http = "http://";
        if (requestMessage.IsHttps) http = "https://";
        var relativePath = requestMessage.PathBase.Value;
        return http + host + relativePath;
    }
}