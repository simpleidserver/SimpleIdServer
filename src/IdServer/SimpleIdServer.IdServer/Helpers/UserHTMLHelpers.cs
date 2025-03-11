// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Html;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class UserHTMLHelpers
    {
        public static IHtmlContent UserPicture(this IHtmlHelper htmlHelper, ClaimsPrincipal claimsPrincipal, string picture = null, bool isEditModeEnabled = false)
        {
            var src = "/images/DefaultUser.png";
            if (!string.IsNullOrWhiteSpace(picture))
                src = picture;
            else
            {
                var cl = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == SimpleIdServer.IdServer.Constants.UserClaims.Picture);
                if (cl != null)
                    src = cl.Value;
            }

            if(!isEditModeEnabled)
                return new HtmlString($"<div class='profile-content'><img id='user-picture' class='img-thumbnail' src='{src}' /></div>");

            return new HtmlString($"<div class='profile-content'><button class='btn edit' id='edit-profile'><i class='fa-solid fa-pen-to-square'></i></button><img id='user-picture' class='img-thumbnail' src='{src}' /></div>");
        }
    }
}
