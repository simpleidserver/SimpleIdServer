// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Webfinger.Models;

public class WebfingerResource
{
    public string Id { get; set; }
    /// <summary>
    ///  The value of the "subject" member is a URI that identifies the entity that the JRD describes.
    /// </summary>
    public string Subject { get; set; }
    /// <summary>
    /// it could be an "acct" URI [18], an "http" or "https" URI, a "mailto" URI[19], or some other scheme.
    /// </summary>
    public string Scheme { get; set; }
    /// <summary>
    /// The URI or registered relation type identifies the type of the link relation.
    /// </summary>
    public string Rel { get; set; }
    /// <summary>
    ///  The value of the "href" member is a string that contains a URI pointing to the target resource.
    /// </summary>
    public string Href { get; set; }
}