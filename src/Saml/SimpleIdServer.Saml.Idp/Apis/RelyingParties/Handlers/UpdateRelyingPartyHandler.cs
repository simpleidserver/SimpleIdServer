// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Saml.Idp.Exceptions;
using SimpleIdServer.Saml.Idp.Extensions;
using SimpleIdServer.Saml.Idp.Persistence;
using SimpleIdServer.Saml.Idp.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Apis.RelyingParties.Handlers
{
    public interface IUpdateRelyingPartyHandler
    {
        Task<bool> Handle(string rpId, JObject content, CancellationToken cancellationToken);
    }

    public class  UpdateRelyingPartyHandler : IUpdateRelyingPartyHandler
    {
        private readonly IRelyingPartyRepository _relyingPartyRepository;
        private readonly ILogger<UpdateRelyingPartyHandler> _logger;

        public UpdateRelyingPartyHandler(IRelyingPartyRepository relyingPartyRepository, ILogger<UpdateRelyingPartyHandler> logger)
        {
            _relyingPartyRepository = relyingPartyRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(string rpId, JObject content, CancellationToken cancellationToken)
        {
            var updateParameter = content.ToUpdateRelyingPartyParameter();
            var rp = await _relyingPartyRepository.Get(rpId, cancellationToken);
            if (rp == null)
            {
                _logger.LogError($"The relying party '{rpId}' doesn't exist");
                throw new RelyingPartyNotFoundException(ErrorCodes.InvalidRequest, string.Format(Global.UnknownRelyingParty, rpId));
            }

            rp.MetadataUrl = updateParameter.MetadataUrl;
            if (updateParameter.AssertionExpirationTimeInSeconds != null)
            {
                rp.AssertionExpirationTimeInSeconds = updateParameter.AssertionExpirationTimeInSeconds.Value;
            }

            rp.ClaimMappings = updateParameter.ClaimMappings;
            rp.UpdateDateTime = DateTime.UtcNow;
            await _relyingPartyRepository.Update(rp, cancellationToken);
            await _relyingPartyRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
