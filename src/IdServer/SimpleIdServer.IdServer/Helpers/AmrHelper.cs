// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Repositories;
using FormBuilder.Stores;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers;

public interface IAmrHelper
{
    Task<AcrResult> FetchDefaultAcr(string realm, IEnumerable<string> requestedAcrValues, IEnumerable<AuthorizedClaim> requestedClaims, Client client, CancellationToken cancellationToken);
    Task<AcrResult> GetSupportedAcr(string realm, IEnumerable<string> requestedAcrValues, CancellationToken cancellationToken);
    string FetchNextAmr(AcrResult acr, string currentAmr);
}

public class AcrResult
{
    public AcrResult(AuthenticationContextClassReference acr, List<string> allAmrs)
    {
        Acr = acr;
        AllAmrs = allAmrs;
    }

    public AuthenticationContextClassReference Acr { get; private set; }
    public List<string> AllAmrs { get; set; }
}

public class AmrHelper : IAmrHelper
{
    private readonly IAuthenticationContextClassReferenceRepository _authenticationContextClassReferenceRepository;
    private readonly IWorkflowStore _workflowStore;
    private readonly IFormStore _formStore;
    private readonly IdServerHostOptions _options;

    public AmrHelper(IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository, IWorkflowStore workflowStore, IFormStore formStore, IOptions<IdServerHostOptions> options)
    {
        _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
        _workflowStore = workflowStore;
        _formStore = formStore;
        _options = options.Value;
    }

    public async Task<AcrResult> FetchDefaultAcr(
        string realm, 
        IEnumerable<string> requestedAcrValues, 
        IEnumerable<AuthorizedClaim> requestedClaims, 
        Client client, 
        CancellationToken cancellationToken)
    {
        var defaultAcr = await GetSupportedAcr(realm, requestedAcrValues, cancellationToken);
        if (defaultAcr == null)
        {
            var acrClaim = requestedClaims?.FirstOrDefault(r => r.Name == JwtRegisteredClaimNames.Acr);
            if (acrClaim != null)
            {
                defaultAcr = await GetSupportedAcr(realm, acrClaim.Values, cancellationToken);
                if (defaultAcr == null && acrClaim.IsEssential)
                    throw new OAuthException(ErrorCodes.ACCESS_DENIED, Global.NoEssentialAcrIsSupported);
            }

            if (defaultAcr == null)
            {
                var acrs = new List<string>();
                if(client != null && client.DefaultAcrValues != null)
                    acrs.AddRange(client.DefaultAcrValues);
                acrs.Add(_options.DefaultAcrValue);
                defaultAcr = await GetSupportedAcr(realm, acrs, cancellationToken);
            }
        }

        return defaultAcr;
    }

    public async Task<AcrResult> GetSupportedAcr(string realm, IEnumerable<string> requestedAcrValues, CancellationToken cancellationToken)
    {
        var acrs = await _authenticationContextClassReferenceRepository.GetByNames(realm, requestedAcrValues.ToList(), cancellationToken);
        foreach (var acrValue in requestedAcrValues)
        {
            var acr = acrs.FirstOrDefault(a => a.Name == acrValue);
            if (acr != null)
            {
                var workflow = await _workflowStore.Get(acr.AuthenticationWorkflow, cancellationToken);
                var allSteps = (await _formStore.GetAll(cancellationToken)).Where(f => f.ActAsStep).Select(f => f.Name);
                var workflowAmrs = allSteps.Where(stepName => workflow.Steps.Any(s => s.FormRecordName == stepName)).ToList();
                return new AcrResult(acr, workflowAmrs);
            }
        }

        return null;
    }

    public string FetchNextAmr(AcrResult acr, string currentAmr)
    {
        var index = acr.AllAmrs.IndexOf(currentAmr);
        if (index == -1 || (index + 1) >= acr.AllAmrs.Count())
        {
            return null;
        }

        return acr.AllAmrs.ElementAt(index + 1);
    }
}
