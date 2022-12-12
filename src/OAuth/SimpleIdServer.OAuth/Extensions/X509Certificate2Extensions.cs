// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Security.Cryptography.X509Certificates
{
    public static class X509Certificate2Extensions
    {
        public static List<KeyValuePair<SubjectAlternativeNameTypes, string>> GetSubjectAlternativeName(this X509Certificate2 certificate)
        {
            var lst = new List<KeyValuePair<SubjectAlternativeNameTypes, string>>();
            var result = certificate.GetExtension(Constants.CertificateOIDS.SubjectAlternativeName);
            if (result == null)
            {
                return lst;
            }

            var content = new AsnEncodedData(Constants.CertificateOIDS.SubjectAlternativeName, result).Format(false);
            var splitted = content.Split(',');
            foreach(var str in splitted)
            {
                var values = str.Split('=');
                if (!values.Any())
                {
                    values = str.Split(':');
                    var newValues = new List<string>
                    {
                        values.First()
                    };
                    newValues.AddRange(values.Skip(1).Take(values.Count() - 1));
                    values = newValues.ToArray();
                }

                var name = values.First();
                if (name.IndexOf("DNS", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    lst.Add(new KeyValuePair<SubjectAlternativeNameTypes, string>(SubjectAlternativeNameTypes.DNSNAME, values.Last()));
                }
                else if (name.IndexOf("IP", StringComparison.InvariantCultureIgnoreCase) !=  -1)
                {
                    lst.Add(new KeyValuePair<SubjectAlternativeNameTypes, string>(SubjectAlternativeNameTypes.IPADDRESS, values.Last()));
                }
                else if (name.IndexOf("RFC822", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    lst.Add(new KeyValuePair<SubjectAlternativeNameTypes, string>(SubjectAlternativeNameTypes.EMAIL, values.Last()));
                }
            }

            return lst;
        }

        public static bool IsSelfSigned(this X509Certificate2 certificate)
        {
            return certificate.SubjectName.RawData.SequenceEqual(certificate.IssuerName.RawData);
        }

        public static bool IsValid(this X509Certificate2 certificate)
        {
            // Externalize this options.
            X509Chain chain = new X509Chain();
            X509ChainPolicy chainPolicy = new X509ChainPolicy()
            {
                RevocationMode = X509RevocationMode.NoCheck,
                RevocationFlag = X509RevocationFlag.EntireChain
            };
            chain.ChainPolicy = chainPolicy;
            return chain.Build(certificate);
        }

        public static byte[] GetExtension(this X509Certificate2 certificate, string oid)
        {
            foreach(var extension in certificate.Extensions)
            {
                if (extension.Oid.Value == oid)
                {
                    return extension.RawData;
                }
            }

            return null;
        }
    }
}
