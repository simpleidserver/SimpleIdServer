// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Html;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class ClientHTMLHelpers
    {
        public static IHtmlContent ClientPicture(this IHtmlHelper htmlHelper, string logoUri)
        {
            var src = logoUri;
            if (string.IsNullOrWhiteSpace(logoUri))
                src = "/images/DefaultClient.png";

            return new HtmlString($"<img class='img-thumbnail' src='{src}' />");
        }
    }
}
