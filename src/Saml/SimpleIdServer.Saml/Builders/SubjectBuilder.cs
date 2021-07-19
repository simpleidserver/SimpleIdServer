// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Xsd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Builders
{
    public class SubjectBuilder
    {
        private readonly SubjectType _subject;

        internal SubjectBuilder(SubjectType subject)
        {
            _subject = subject;
        }

        #region Operations

        /// <summary>
        /// Identifies the subject.
        /// </summary>
        /// <param name="format">A URI reference representing the classification of string-based identifier information. If no value is specified then the value "nameid-format-unspecified" is in effect.</param>
        /// <param name="value">Value of the name</param>
        /// <param name="spProviderId">Name Identifier established by a service provider or affiliation providers for the entity.</param>
        /// <param name="nameQualifier">The security or administrative domain that qualifies the name.</param>
        /// <param name="spNameQualifier">Further qualifies a name with the name of the service provider or affiliaton provider.</param>
        /// <returns></returns>
        public SubjectBuilder SetNameId(string format, string value, string spProviderId = null, string nameQualifier = null, string spNameQualifier = null)
        {
            var nameIdType = new NameIDType
            {
                Format = format,
                Value = value,
                SPProvidedID = spProviderId,
                NameQualifier = nameQualifier,
                SPNameQualifier = spNameQualifier
            };
            ReplaceIdentifier(nameIdType);
            return this;
        }

        public SubjectBuilder SetEncryptedId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SAML issuer uses its certificate to produce a holder-of-key SAML assertion. 
        /// The relying party consumes the assertion, confirming the attesting entity by comparing the X.509 data in the assertion with the X.509 data in its possession.
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public SubjectBuilder AddSubjectConfirmationHolderOfKey(X509Certificate2 certificate)
        {
            var subjectConfirmation = new SubjectConfirmationType
            {
                Method = Constants.ConfirmationMethodIdentifiers.HolderOfKey,
                SubjectConfirmationData = new KeyInfoConfirmationDataType
                {
                    KeyInfo = new KeyInfoType
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
                    }
                }
            };
            AddSubjectConfirmation(subjectConfirmation);
            return this;
        }

        #endregion

        private void ReplaceIdentifier(object value)
        {
            var items = _subject.Items;
            var lst = new List<object>();
            if (items != null)
            {
                lst = items.ToList();
                var rec = lst.FirstOrDefault(l => l is NameIDType || l is EncryptedElementType);
                if (rec != null)
                {
                    lst.Remove(rec);
                }
            }

            lst.Add(value);
            _subject.Items = lst.ToArray();
        }

        private void AddSubjectConfirmation(object value)
        {
            var items = _subject.Items;
            var lst = new List<object>();
            if (items != null)
            {
                lst = items.ToList();
            }

            lst.Add(value);
            _subject.Items = lst.ToArray();
        }
    }
}
