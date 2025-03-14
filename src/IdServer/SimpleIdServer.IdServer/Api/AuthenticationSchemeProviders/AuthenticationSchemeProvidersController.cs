// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.AuthenticationSchemeProviders;

public class AuthenticationSchemeProvidersController : BaseController
{
	private readonly IAuthenticationSchemeProviderRepository _authenticationSchemeProviderRepository;
	private readonly IAuthenticationSchemeProviderDefinitionRepository _authenticationSchemeProviderDefinitionRepository;
	private readonly IRealmRepository _realmRepository;
	private readonly IConfiguration _configuration;
    private readonly ITransactionBuilder _transactionBuilder;

	public AuthenticationSchemeProvidersController(
        IAuthenticationSchemeProviderRepository authenticationSchemeProviderRepository, 
        IAuthenticationSchemeProviderDefinitionRepository authenticationSchemeProviderDefinitionRepository, 
        IRealmRepository realmRepository,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder, 
        IConfiguration configuration,
        ITransactionBuilder transactionBuilder) : base(tokenRepository, jwtBuilder)
	{
		_authenticationSchemeProviderRepository = authenticationSchemeProviderRepository;
		_authenticationSchemeProviderDefinitionRepository = authenticationSchemeProviderDefinitionRepository;
		_realmRepository = realmRepository;
		_configuration = configuration;
        _transactionBuilder = transactionBuilder;
	}

	[HttpPost]
	public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchRequest request, CancellationToken cancellationToken)
	{
		prefix = prefix ?? Constants.DefaultRealm;
		try
		{
            await CheckAccessToken(prefix, Constants.DefaultScopes.AuthenticationSchemeProviders.Name);
            var result = await _authenticationSchemeProviderRepository.Search(prefix, request, cancellationToken);
			return new OkObjectResult(new SearchResult<AuthenticationSchemeProviderResult>
			{
				Count = result.Count,
				Content = result.Content.Select(p => Build(p)).ToList()
			});
		}
		catch(OAuthException ex)
		{
			return BuildError(ex);
		}
    }

    [HttpGet]
    public async Task<IActionResult> GetDefinitions([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.DefaultScopes.AuthenticationSchemeProviders.Name);
            var result = await _authenticationSchemeProviderDefinitionRepository.GetAll(cancellationToken);
            return new OkObjectResult(result);
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    [HttpDelete]
	public async Task<IActionResult> Remove([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Constants.DefaultScopes.AuthenticationSchemeProviders.Name);
                var result = await _authenticationSchemeProviderRepository.Get(prefix, id, cancellationToken);
                if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownAuthSchemeProvider, id));
                _authenticationSchemeProviderRepository.Remove(result);
                await transaction.Commit(cancellationToken);
                return NoContent();
            }
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.DefaultScopes.AuthenticationSchemeProviders.Name);
            var result = await _authenticationSchemeProviderRepository.Get(prefix, id, cancellationToken);
            if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownAuthSchemeProvider, id));
            var optionKey = $"{result.Name}:{result.AuthSchemeProviderDefinition.OptionsName}";
            var optionType = Assembly.GetEntryAssembly().GetType(result.AuthSchemeProviderDefinition.OptionsFullQualifiedName);
            var section = _configuration.GetSection(optionKey);
            var configuration = section.Get(optionType);
            return new OkObjectResult(Build(result, configuration));
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

	[HttpPost]
	public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] AddAuthenticationSchemeProviderRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Constants.DefaultScopes.AuthenticationSchemeProviders.Name);
                Validate();
                var instance = await _authenticationSchemeProviderRepository.Get(prefix, request.Name, cancellationToken);
                if (instance != null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.AuthSchemeProviderWithSameNameExists);
                var idProviderDef = await _authenticationSchemeProviderDefinitionRepository.Get(request.DefinitionName, cancellationToken);
                var realm = await _realmRepository.Get(prefix, cancellationToken);
                var result = new AuthenticationSchemeProvider
                {
                    Id = Guid.NewGuid().ToString(),
                    AuthSchemeProviderDefinition = idProviderDef,
                    CreateDateTime = DateTime.UtcNow,
                    Description = request.Description,
                    DisplayName = request.DisplayName,
                    Mappers = Constants.GetDefaultIdProviderMappers(),
                    Name = request.Name,
                    UpdateDateTime = DateTime.UtcNow
                };
                result.Realms.Add(realm);
                SyncConfiguration(result, request.Values);
                _authenticationSchemeProviderRepository.Add(result);
                await transaction.Commit(cancellationToken);
                return NoContent();
            }
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }

        void Validate()
        {
            if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidRequestParameter);
            if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, nameof(AuthenticationSchemeProviderNames.Name)));
        }
    }

	[HttpPut]
	public async Task<IActionResult> UpdateDetails([FromRoute] string prefix, string id, [FromBody] UpdateAuthenticationSchemeProviderDetailsRequest request, CancellationToken cancellationToken)
	{
		prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Constants.DefaultScopes.AuthenticationSchemeProviders.Name);
                var instance = await _authenticationSchemeProviderRepository.Get(prefix, id, cancellationToken);
                if (instance == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownAuthSchemeProvider, id));
                instance.UpdateDateTime = DateTime.UtcNow;
                instance.Description = request.Description;
                instance.DisplayName = request.DisplayName;
                _authenticationSchemeProviderRepository.Update(instance);
                await transaction.Commit(cancellationToken);
                return NoContent();
            }
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateValues([FromRoute] string prefix, string id, [FromBody] UpdateAuthenticationSchemeProviderValuesRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Constants.DefaultScopes.AuthenticationSchemeProviders.Name);
                var instance = await _authenticationSchemeProviderRepository.Get(prefix, id, cancellationToken);
                if (instance == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownAuthSchemeProvider, id));
                instance.UpdateDateTime = DateTime.UtcNow;
                SyncConfiguration(instance, request.Values);
                _authenticationSchemeProviderRepository.Update(instance);
                await transaction.Commit(cancellationToken);
                return NoContent();
            }
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

	[HttpPost]
	public async Task<IActionResult> AddMapper([FromRoute] string prefix, string id, [FromBody] AddAuthenticationSchemeProviderMapperRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Constants.DefaultScopes.AuthenticationSchemeProviders.Name);
                var instance = await _authenticationSchemeProviderRepository.Get(prefix, id, cancellationToken);
                if (instance == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownAuthSchemeProvider, id));
                instance.UpdateDateTime = DateTime.UtcNow;
                var record = new AuthenticationSchemeProviderMapper
                {
                    Id = Guid.NewGuid().ToString(),
                    MapperType = request.MapperType,
                    Name = request.Name,
                    SourceClaimName = request.SourceClaimName,
                    TargetUserAttribute = request.TargetUserAttribute,
                    TargetUserProperty = request.TargetUserProperty
                };
                instance.Mappers.Add(record);
                _authenticationSchemeProviderRepository.Update(instance);
                await transaction.Commit(cancellationToken);
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JsonSerializer.Serialize(Build(record)),
                    ContentType = "application/json"
                };
            }
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveMapper([FromRoute] string prefix, string id, string mapperId, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Constants.DefaultScopes.AuthenticationSchemeProviders.Name);
                var result = await _authenticationSchemeProviderRepository.Get(prefix, id, cancellationToken);
                if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownAuthSchemeProvider, id));
                result.Mappers = result.Mappers.Where(m => m.Id != mapperId).ToList();
                _authenticationSchemeProviderRepository.Update(result);
                await transaction.Commit(cancellationToken);
                return NoContent();
            }
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMapper([FromRoute] string prefix, string id, string mapperId, [FromBody] UpdateAuthenticationSchemeProviderMapperRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Constants.DefaultScopes.AuthenticationSchemeProviders.Name);
                var instance = await _authenticationSchemeProviderRepository.Get(prefix, id, cancellationToken);
                if (instance == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownAuthSchemeProvider, id));
                instance.UpdateDateTime = DateTime.UtcNow;
                var mapper = instance.Mappers.Single(m => m.Id == mapperId);
                mapper.Name = request.Name;
                mapper.SourceClaimName = request.SourceClaimName;
                mapper.TargetUserAttribute = request.TargetUserAttribute;
                mapper.TargetUserProperty = request.TargetUserProperty;
                _authenticationSchemeProviderRepository.Update(instance);
                await transaction.Commit(cancellationToken);
                return NoContent();
            }
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    private void SyncConfiguration(AuthenticationSchemeProvider authenticationSchemeProvider, Dictionary<string, string> values)
	{
		var optionKey = $"{authenticationSchemeProvider.Name}:{authenticationSchemeProvider.AuthSchemeProviderDefinition.OptionsName}";
		foreach(var kvp in values)
			_configuration[$"{optionKey}:{kvp.Key}"] = kvp.Value;
	}

	private static AuthenticationSchemeProviderResult Build(AuthenticationSchemeProvider authenticationSchemeProvider, object obj = null)
    {
        var values = new Dictionary<string, string>();
		if(obj != null)
        {
            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                var value = property.GetValue(obj)?.ToString();
                if (!string.IsNullOrWhiteSpace(value)) values.Add(property.Name, value);
            }
        }

		return new AuthenticationSchemeProviderResult
		{
			CreateDateTime = authenticationSchemeProvider.CreateDateTime,
			Description = authenticationSchemeProvider.Description,
			DisplayName = authenticationSchemeProvider.DisplayName,
			Id = authenticationSchemeProvider.Id,
			Name = authenticationSchemeProvider.Name,
			UpdateDateTime = authenticationSchemeProvider.UpdateDateTime,
			Mappers = authenticationSchemeProvider.Mappers == null ? new List<AuthenticationSchemeProviderMapperResult>() : authenticationSchemeProvider.Mappers.Select(m => Build(m)).ToList(),
			Definition = authenticationSchemeProvider.AuthSchemeProviderDefinition == null ? null : new AuthenticationSchemeProviderDefinitionResult
            {
                Name = authenticationSchemeProvider.AuthSchemeProviderDefinition.Name,
                Description = authenticationSchemeProvider.AuthSchemeProviderDefinition.Description,
                Image = authenticationSchemeProvider.AuthSchemeProviderDefinition.Image,
                HandlerFullQualifiedName = authenticationSchemeProvider.AuthSchemeProviderDefinition.HandlerFullQualifiedName,
                OptionsFullQualifiedName = authenticationSchemeProvider.AuthSchemeProviderDefinition.OptionsFullQualifiedName,
                OptionsName = authenticationSchemeProvider.AuthSchemeProviderDefinition.OptionsName
            },
			Values = values
		};
	}

    private static AuthenticationSchemeProviderMapperResult Build(AuthenticationSchemeProviderMapper mapper)
    {
        return new AuthenticationSchemeProviderMapperResult
        {
            Id = mapper.Id,
            MapperType = mapper.MapperType,
            Name = mapper.Name,
            SourceClaimName = mapper.SourceClaimName,
            TargetUserAttribute = mapper.TargetUserAttribute,
            TargetUserProperty = mapper.TargetUserProperty
        };
    }
}
