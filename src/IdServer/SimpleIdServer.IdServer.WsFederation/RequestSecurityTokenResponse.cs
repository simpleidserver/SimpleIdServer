// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml;
using Microsoft.IdentityModel.Xml;
using System.Globalization;
using System.Text;
using System.Xml;

namespace SimpleIdServer.IdServer.WsFederation;

public class RequestSecurityTokenResponse
{
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string AppliesTo { get; set; }
    public string Context { get; set; }
    public string ReplyTo { get; set; }
    public SecurityToken RequestedSecurityToken { get; set; }
    public SecurityTokenHandler SecurityTokenHandler { get; set; }

    public string Serialize()
    {
        using var ms = new MemoryStream();
        using var writer = XmlDictionaryWriter.CreateTextWriter(ms, Encoding.UTF8, false);
        // <t:RequestSecurityTokenResponseCollection>
        writer.WriteStartElement(WsTrustConstants_1_3.PreferredPrefix, Microsoft.IdentityModel.Xml.WsTrustConstants.Elements.RequestSecurityTokenResponseCollection, WsTrustConstants.Namespaces.WsTrust1_3);
        // <t:RequestSecurityTokenResponse>
        writer.WriteStartElement(WsTrustConstants_1_3.PreferredPrefix, Microsoft.IdentityModel.Xml.WsTrustConstants.Elements.RequestSecurityTokenResponse, WsTrustConstants.Namespaces.WsTrust1_3);
        // @Context
        writer.WriteAttributeString("Context", Context);

        // <t:Lifetime>
        writer.WriteStartElement(WsTrustConstants.Elements.Lifetime, WsTrustConstants.Namespaces.WsTrust1_3);

        // <wsu:Created></wsu:Created>
        writer.WriteElementString(WsUtility.PreferredPrefix, WsUtility.Elements.Created, WsUtility.Namespace, CreatedAt.ToString(SamlConstants.GeneratedDateTimeFormat, DateTimeFormatInfo.InvariantInfo));
        // <wsu:Expires></wsu:Expires>
        writer.WriteElementString(WsUtility.PreferredPrefix, WsUtility.Elements.Expires, WsUtility.Namespace, ExpiresAt.ToString(SamlConstants.GeneratedDateTimeFormat, DateTimeFormatInfo.InvariantInfo));

        // </t:Lifetime>
        writer.WriteEndElement();

        // <wsp:AppliesTo>
        writer.WriteStartElement(WsPolicy.PreferredPrefix, WsPolicy.Elements.AppliesTo, WsPolicy.Namespace);

        // <wsa:EndpointReference>
        writer.WriteStartElement(WsAddressing.PreferredPrefix, WsAddressing.Elements.EndpointReference, WsAddressing.Namespace);

        // <wsa:Address></wsa:Address>
        writer.WriteElementString(WsAddressing.PreferredPrefix, WsAddressing.Elements.Address, WsAddressing.Namespace, AppliesTo);

        writer.WriteEndElement();
        // </wsa:EndpointReference>

        writer.WriteEndElement();
        // </wsp:AppliesTo>

        // <t:RequestedSecurityToken>
        writer.WriteStartElement(WsTrustConstants_1_3.PreferredPrefix, WsTrustConstants.Elements.RequestedSecurityToken, WsTrustConstants.Namespaces.WsTrust1_3);

        // write assertion
        SecurityTokenHandler.WriteToken(writer, RequestedSecurityToken);

        // </t:RequestedSecurityToken>
        writer.WriteEndElement();

        // </t:RequestSecurityTokenResponse>
        writer.WriteEndElement();

        // <t:RequestSecurityTokenResponseCollection>
        writer.WriteEndElement();

        writer.Flush();
        return Encoding.UTF8.GetString(ms.ToArray());
    }
}
