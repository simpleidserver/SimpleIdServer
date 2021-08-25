// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Saml.Idp.Exceptions;
using SimpleIdServer.Saml.Idp.Extensions;
using SimpleIdServer.Saml.Idp.Persistence;
using SimpleIdServer.Saml.Idp.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Apis.RelyingParties.Handlers
{
    public interface IGetRelyingPartyHandler
    {
        Task<JObject> Handle(string id, CancellationToken cancellationToken);
    }

    public class GetRelyingPartyHandler : IGetRelyingPartyHandler
    {
        private readonly IRelyingPartyRepository _relyingPartyRepository;

        public GetRelyingPartyHandler(IRelyingPartyRepository relyingPartyRepository)
        {
            _relyingPartyRepository = relyingPartyRepository;
        }

        public async Task<JObject> Handle(string id, CancellationToken cancellationToken)
        {
            var result = await _relyingPartyRepository.Get(id, cancellationToken);
            if (result == null)
            {
                throw new RelyingPartyNotFoundException(ErrorCodes.InvalidRequest, string.Format(Global.UnknownRelyingParty, id));
            }

            return result.ToDto();
        }
    }
}
