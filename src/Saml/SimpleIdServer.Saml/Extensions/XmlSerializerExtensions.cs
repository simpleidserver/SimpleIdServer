// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SimpleIdServer.Saml.Extensions
{
    public static class XmlSerializerExtensions
    {
        public static XmlElement SerializeToXmlElement(this object obj)
        {
            var doc = new XmlDocument();
            using (var writer = doc.CreateNavigator().AppendChild())
            {
                new XmlSerializer(obj.GetType()).Serialize(writer, obj);
            }

            return doc.DocumentElement;
        }
        public static XmlDocument SerializeToXmlDocument(this object obj)
        {
            var doc = new XmlDocument();
            using (var writer = doc.CreateNavigator().AppendChild())
            {
                new XmlSerializer(obj.GetType()).Serialize(writer, obj);
            }

            return doc;
        }

        public static T DeserializeXml<T>(this string xml) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(xml))
            {
                return serializer.Deserialize(reader) as T;
            }
        }
    }
}
