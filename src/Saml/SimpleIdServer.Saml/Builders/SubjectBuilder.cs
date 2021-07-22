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
                    KeyInfo = KeyInfoBuilder.Build(certificate)
                }
            };
            AddSubjectConfirmation(subjectConfirmation);
            return this;
        }

        /// <summary>
        /// Subject of the assertion is the bearer of the assertion
        /// </summary>
        /// <param name="notBefore">A time instant before which the subject cannot be confirmed.</param>
        /// <param name="notOnAfter">A time instant at which the subject can no longer be confirmed.</param>
        /// <param name="recipient">URI specifying the entity or location to which an attesting entity can present the assertion.</param>
        /// <param name="inResponseTo">The ID of a SAML protocol message in response to which an attesting entity can present the assertion.</param>
        /// <returns></returns>
        public SubjectBuilder AddSubjectConfirmationBearer(DateTime? notBefore, DateTime? notOnAfter, string recipient = null, string inResponseTo = null)
        {
            var data = new SubjectConfirmationDataType
            {
                InResponseTo = inResponseTo,
                Recipient = recipient
            };
            if (notBefore != null)
            {
                data.NotBefore = notBefore.Value;
                data.NotBeforeSpecified = true;
            }

            if (notOnAfter != null)
            {
                data.NotOnOrAfter = notOnAfter.Value;
                data.NotOnOrAfterSpecified = true;
            }

            var subjectConfirmation = new SubjectConfirmationType
            {
                Method = Constants.ConfirmationMethodIdentifiers.Bearer,
                SubjectConfirmationData = data
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
