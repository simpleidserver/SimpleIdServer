// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.SubjectTypeBuilders;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SimpleIdServer.IdServer.Extractors
{
    public class SubClaimExtractor : BaseClaimExtractor, IClaimExtractor
    {
        private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;

        public SubClaimExtractor(IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders)
        {
            _subjectTypeBuilders = subjectTypeBuilders;
        }

        public MappingRuleTypes MappingRuleType => MappingRuleTypes.SUBJECT;

        public object Extract(HandlerContext context, IClaimMappingRule mappingRule)
        {
            var client = context.Client;
            var subjectTypeBuilder = _subjectTypeBuilders.First(f => f.SubjectType == (string.IsNullOrWhiteSpace(client?.SubjectType) ? PublicSubjectTypeBuilder.SUBJECT_TYPE : client.SubjectType));
            var subject = subjectTypeBuilder.Build(context, CancellationToken.None).Result;
            return subject;
        }
    }
}
