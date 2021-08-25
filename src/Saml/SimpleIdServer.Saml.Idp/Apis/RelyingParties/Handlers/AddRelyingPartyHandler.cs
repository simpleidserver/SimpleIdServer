// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.Extensions;
using SimpleIdServer.Saml.Idp.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Apis.RelyingParties.Handlers
{
    public interface IAddRelyingPartyHandler
    {
        Task<string> Handle(JObject jObj, CancellationToken cancellationToken);
    }

    public class AddRelyingPartyHandler : IAddRelyingPartyHandler
    {
        private readonly IRelyingPartyRepository _relyingPartyRepository;
        private readonly ILogger<AddRelyingPartyHandler> _logger;

        public AddRelyingPartyHandler(
            IRelyingPartyRepository relyingPartyRepository,
            ILogger<AddRelyingPartyHandler> logger)
        {
            _relyingPartyRepository = relyingPartyRepository;
            _logger = logger;
        }

        public async Task<string> Handle(JObject jObj, CancellationToken cancellationToken)
        {
            var parameter = jObj.ToCreateRelyingPartyParameter();
            var relyingParty = RelyingPartyAggregate.Create(parameter.MetadataUrl);
            await _relyingPartyRepository.Add(relyingParty, cancellationToken);
            await _relyingPartyRepository.SaveChanges(cancellationToken);
            _logger.LogInformation("Relying party has been added");
            return relyingParty.Id;
        }
    }
}
