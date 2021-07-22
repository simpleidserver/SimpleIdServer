// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Xsd;

namespace SimpleIdServer.Saml.Builders
{
    public class ConditionsBuilder
    {
        private readonly ConditionsType _conditions;

        internal ConditionsBuilder(ConditionsType conditions)
        {
            _conditions = conditions;
        }

        #region Actions

        /// <summary>
        /// Assertion is addressed to one or more specific audiences.
        /// </summary>
        /// <param name="audiences">URI reference that identifies an intended audience.</param>
        /// <returns></returns>
        public ConditionsBuilder AddAudienceRestriction(params string[] audiences)
        {
            var audienceRestriction = new AudienceRestrictionType
            {
                Audience = audiences
            };
            _conditions.Items = _conditions.Items.Add(audienceRestriction);
            return this;
        }

        #endregion
    }
}
