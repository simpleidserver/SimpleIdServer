// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Html;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class UserHTMLHelpers
    {
        public static IHtmlContent UserPicture(this IHtmlHelper htmlHelper, ClaimsPrincipal claimsPrincipal)
        {
            var src = "_content/SimpleIdServer.IdServer.Website/images/DefaultUser.png";
            var cl = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == SimpleIdServer.IdServer.Constants.UserClaims.Profile);
            if (cl != null)
                src = cl.Value;
            return new HtmlString($"<img class='img-thumbnail' src='{src}' />");
        }
    }
}
