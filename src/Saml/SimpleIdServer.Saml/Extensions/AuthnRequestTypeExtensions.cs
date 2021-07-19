// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Xsd;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SimpleIdServer.Saml.Extensions
{
    public static class AuthnRequestTypeExtensions
    {
        public static string SerializeXml(this AuthnRequestType request)
        {
            var namespaces = BuildXmlSerializerNamespaces();
            var serializer = new XmlSerializer(typeof(AuthnRequestType));
            using (var reader = new MemoryStream())
            {
                serializer.Serialize(reader, request, namespaces);
                var payload = reader.ToArray();
                return Encoding.UTF8.GetString(payload);
            }
        }

        public static AuthnRequestType DeserializeAuthnRequestFromXml(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            var serializer = new XmlSerializer(typeof(AuthnRequestType));
            using (var reader = new StringReader(str))
            {
                return serializer.Deserialize(reader) as AuthnRequestType;
            }
        }

        private static XmlSerializerNamespaces BuildXmlSerializerNamespaces()
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
            return namespaces;
        }
    }
}
