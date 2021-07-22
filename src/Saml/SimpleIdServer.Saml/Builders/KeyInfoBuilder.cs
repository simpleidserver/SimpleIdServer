// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Xsd;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Builders
{
    public static class KeyInfoBuilder
    {
        public static KeyInfoType Build(X509Certificate2 certificate)
        {
            return new KeyInfoType
            {
                Items = new object[]
                {
                    new X509DataType
                    {
                        Items = new object[]
                        {
                            certificate.RawData,
                            certificate.SubjectName.Name,
                            new X509IssuerSerialType
                            {
                                X509IssuerName = certificate.IssuerName.Name,
                                X509SerialNumber = certificate.SerialNumber
                            }
                        },
                        ItemsElementName = new ItemsChoiceType[]
                        {
                            ItemsChoiceType.X509Certificate,
                            ItemsChoiceType.X509SubjectName,
                            ItemsChoiceType.X509IssuerSerial
                        }
                    },
                },
                ItemsElementName = new ItemsChoiceType2[]
                {
                    ItemsChoiceType2.X509Data
                }
            };
        }
    }
}
