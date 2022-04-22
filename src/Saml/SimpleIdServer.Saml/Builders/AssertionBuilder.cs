// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Xsd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Saml.Builders
{
    public class AssertionBuilder
    {
        private AssertionType _assertion;

        internal AssertionBuilder(AssertionType assertion)
        {
            _assertion = assertion;
        }


        #region Actions

        /// <summary>
        /// Set the issuer.
        /// </summary>
        /// <param name="format">A URI reference representing the classification of string-based identifier information. If no value is specified then the value "nameid-format-unspecified" is in effect.</param>
        /// <param name="value">Value of the name</param>
        /// <param name="spProviderId">Name Identifier established by a service provider or affiliation providers for the entity.</param>
        /// <param name="nameQualifier">The security or administrative domain that qualifies the name.</param>
        /// <param name="spNameQualifier">Further qualifies a name with the name of the service provider or affiliaton provider.</param>
        /// <returns></returns>
        public AssertionBuilder SetIssuer(string format, string value, string spProviderId = null, string nameQualifier = null, string spNameQualifier = null)
        {
            _assertion.Issuer = new NameIDType
            {
                Format = format,
                Value = value,
                SPProvidedID = spProviderId,
                NameQualifier = nameQualifier,
                SPNameQualifier = spNameQualifier
            };
            return this;
        }

        public AssertionBuilder SetAuthnStatement(string Index, string ClassRef )
        {
            _assertion.AuthnStatement = new AuthnStatementType()
            {
                AuthnContext = new AuthnContextType()
                {
                    AuthnContextClassRef = ClassRef
                },
                SessionIndex = Index
            };
            return this;
        }

        /// <summary>
        /// Set the subject.
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public AssertionBuilder SetSubject(Action<SubjectBuilder> callback)
        {
            var subject = new SubjectType();
            var builder = new SubjectBuilder(subject);
            callback(builder);
            _assertion.Subject = subject;
            return this;
        }

        /// <summary>
        /// Must be evaluated when assessing the validity of and/or when using the assertion.
        /// </summary>
        /// <param name="notBefore">Specifies the earliest time instant at which the assertion is valid.</param>
        /// <param name="notOnOrAfter">Specifies the time instance at which the assertion has expired.</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public AssertionBuilder SetConditions(DateTime? notBefore, DateTime? notOnOrAfter, Action<ConditionsBuilder> callback)
        {
            var conditions = new ConditionsType();
            if (notBefore != null)
            {
                conditions.NotBeforeSpecified = true;
                conditions.NotBefore = notBefore.Value;
            }

            if (notOnOrAfter != null)
            {
                conditions.NotOnOrAfterSpecified = true;
                conditions.NotOnOrAfter = notOnOrAfter.Value;
            }

            var builder = new ConditionsBuilder(conditions);
            callback(builder);
            _assertion.Conditions = conditions;
            return this;
            // AttributeStatement
        }

        /// <summary>
        /// Express particular attributes and values associates with an assertion subject.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="nameFormat">Classification of the attribute name.</param>
        /// <param name="valueType">Data type</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public AssertionBuilder AddAttributeStatementAttribute(string name, string nameFormat, string valueType, string value)
        {
            if (_assertion.Items == null)
            {
                _assertion.Items = new StatementAbstractType[0];
            }

            var attributeStatement = _assertion.Items.FirstOrDefault(i => i is AttributeStatementType) as AttributeStatementType;
            if (attributeStatement == null)
            {
                attributeStatement = new AttributeStatementType();
                _assertion.Items = _assertion.Items.Add(attributeStatement);
            }

            var attributes = new List<object>();
            if (attributeStatement.Items != null)
            {
                attributes = attributeStatement.Items.ToList();
            }

            var attribute = attributes.FirstOrDefault(a => a is AttributeType && ((AttributeType)a).Name == name) as AttributeType;
            if (attribute == null)
            {
                attribute = new AttributeType { Name = name, NameFormat = nameFormat };
                attributes.Add(attribute);
            }

            var values = new ArrayList();
            if (attribute.AttributeValue != null)
            {
                values = attribute.AttributeValue;
            }

            values.Add(value);
            attribute.AttributeValue = values;
            attributeStatement.Items = attributes.ToArray();
            return this;
        }

        #endregion
    }
}
