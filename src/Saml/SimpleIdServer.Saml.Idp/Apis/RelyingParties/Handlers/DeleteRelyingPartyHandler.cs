// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using SimpleIdServer.Saml.Idp.Exceptions;
using SimpleIdServer.Saml.Idp.Persistence;
using SimpleIdServer.Saml.Idp.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Apis.RelyingParties.Handlers
{
    public interface IDeleteRelyingPartyHandler
    {
        Task<bool> Handle(string id, CancellationToken cancellationToken);
    }

    public class DeleteRelyingPartyHandler : IDeleteRelyingPartyHandler
    {
        private readonly IRelyingPartyRepository _relyingPartyRepository;
        private readonly ILogger<DeleteRelyingPartyHandler> _logger;

        public DeleteRelyingPartyHandler(
            IRelyingPartyRepository relyingPartyRepository,
            ILogger<DeleteRelyingPartyHandler> logger)
        {
            _relyingPartyRepository = relyingPartyRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(string id, CancellationToken cancellationToken)
        {
            var rp = await _relyingPartyRepository.Get(id, cancellationToken);
            if (rp == null)
            {
                _logger.LogError($"The relying party '{id}' doesn't exist");
                throw new RelyingPartyNotFoundException(ErrorCodes.InvalidRequest, string.Format(Global.UnknownRelyingParty, id));
            }

            await _relyingPartyRepository.Delete(rp, cancellationToken);
            await _relyingPartyRepository.SaveChanges(cancellationToken);
            _logger.LogInformation($"Relying party {id} has been removed");
            return true;
        }
    }
}
