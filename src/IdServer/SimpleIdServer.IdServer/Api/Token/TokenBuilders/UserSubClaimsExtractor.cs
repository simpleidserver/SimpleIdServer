// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.SubjectTypeBuilders;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.TokenBuilders
{
    public class UserSubClaimsExtractor : BaseAttributeClaimsExtractor, IMapperClaimsExtractor
    {
        private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;

        public UserSubClaimsExtractor(IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders)
        {
            _subjectTypeBuilders = subjectTypeBuilders;
        }

        public ScopeClaimMapperTypes Type => ScopeClaimMapperTypes.SUBJECT;

        public async Task<KeyValuePair<string, object>?> Extract(ClaimsExtractionParameter parameter, ScopeClaimMapper mapper)
        {
            var client = parameter.Context.Client;
            var subjectTypeBuilder = _subjectTypeBuilders.First(f => f.SubjectType == (string.IsNullOrWhiteSpace(client?.SubjectType) ? PublicSubjectTypeBuilder.SUBJECT_TYPE : client.SubjectType));
            var subject = await subjectTypeBuilder.Build(parameter.Context, CancellationToken.None);
            return new KeyValuePair<string, object>(JwtRegisteredClaimNames.Sub, subject);
        }
    }
}
