// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
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
    Task<AcrResult> Get(string realm, List<string> names, CancellationToken cancellationToken);
    string FetchNextAmr(AcrResult acr, string currentAmr);
}

public class AcrResult
{
    public AcrResult(AuthenticationContextClassReference acr, List<string> allAmrs, WorkflowRecord workflow, List<FormRecord> forms)
    {
        Acr = acr;
        AllAmrs = allAmrs;
        Workflow = workflow;
        Forms = forms;
    }

    public AuthenticationContextClassReference Acr { get; private set; }
    public List<string> AllAmrs { get; set; }
    public WorkflowRecord Workflow { get; set; }
    public List<FormRecord> Forms { get; set; }
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

    public async Task<AcrResult> FetchDefaultAcr(string realm, IEnumerable<string> requestedAcrValues, IEnumerable<AuthorizedClaim> requestedClaims, Client client, CancellationToken cancellationToken)
    {
        // 1. Fetch default ACR from the authorization request.
        var defaultAcr = await Get(realm, requestedAcrValues.ToList(), cancellationToken);
        if (defaultAcr != null) return defaultAcr;
        // 2. Fetch default ACR from the requested claims.
        var acrClaim = requestedClaims?.FirstOrDefault(r => r.Name == JwtRegisteredClaimNames.Acr);
        if (acrClaim != null)
        {
            defaultAcr = await Get(realm, acrClaim.Values.ToList(), cancellationToken);
            if (defaultAcr == null && acrClaim.IsEssential) throw new OAuthException(ErrorCodes.ACCESS_DENIED, Global.NoEssentialAcrIsSupported);
        }

        if (defaultAcr != null) return defaultAcr;
        // 3. Fetch default ACR from the client.
        var acrs = new List<string>();
        if(client != null && client.DefaultAcrValues != null)
            acrs.AddRange(client.DefaultAcrValues);
        acrs.Add(_options.DefaultAcrValue);
        return await Get(realm, acrs, cancellationToken);
    }

    public async Task<AcrResult> Get(string realm, List<string> names, CancellationToken cancellationToken)
    {
        var acrs = await _authenticationContextClassReferenceRepository.GetByNames(realm, names, cancellationToken);
        foreach (var name in names)
        {
            var acr = acrs.FirstOrDefault(a => a.Name == name);
            if (acr == null) continue;
            var workflow = await _workflowStore.Get(acr.AuthenticationWorkflow, cancellationToken);
            var forms = await _formStore.GetAll(cancellationToken);
            var amrs = WorkflowHelper.ExtractAmrs(workflow, forms);
            return new AcrResult(acr, amrs, workflow, forms);
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
