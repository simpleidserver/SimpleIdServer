// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Provisioning
{
    public class IdentityProvisioningController : BaseController
    {
        private readonly IIdentityProvisioningStore _identityProvisioningStore;
        private readonly IServiceProvider _serviceProvider;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IConfiguration _configuration;

        public IdentityProvisioningController(IIdentityProvisioningStore identityProvisioningStore, IServiceProvider serviceProvider, IJwtBuilder jwtBuilder, IConfiguration configuration)
        {
            _identityProvisioningStore = identityProvisioningStore;
            _serviceProvider = serviceProvider;
            _jwtBuilder = jwtBuilder;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchRequest request)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name, _jwtBuilder);
                IQueryable<IdentityProvisioning> query = _identityProvisioningStore.Query()
                    .Include(p => p.Realms)
                    .Where(p => p.Realms.Any(r => r.Name == prefix))
                    .AsNoTracking();
                if (!string.IsNullOrWhiteSpace(request.Filter))
                    query = query.Where(request.Filter);

                if (!string.IsNullOrWhiteSpace(request.OrderBy))
                    query = query.OrderBy(request.OrderBy);

                var nb = query.Count();
                var idProviders = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
                return new OkObjectResult(new SearchResult<IdentityProvisioningResult>
                {
                    Count = nb,
                    Content = idProviders.Select(p => Build(p)).ToList()
                });
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Remove([FromRoute] string prefix, string id)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name, _jwtBuilder);
                var result = await _identityProvisioningStore.Query()
                    .Include(p => p.Realms)
                    .Where(p => p.Realms.Any(r => r.Name == prefix))
                    .SingleOrDefaultAsync(p => p.Id == id);
                if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_IDPROVISIONING, id));
                _identityProvisioningStore.Remove(result);
                await _identityProvisioningStore.SaveChanges(CancellationToken.None);
                return NoContent();
            }
            catch(OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] string prefix, string id)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name, _jwtBuilder);
                var result = await _identityProvisioningStore.Query()
                    .Include(p => p.Realms)
                    .Include(p => p.Histories)
                    .Include(p => p.Definition).ThenInclude(d => d.MappingRules)
                    .Where(p => p.Realms.Any(r => r.Name == prefix))
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.Id == id);
                if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_IDPROVISIONING, id));
                var optionKey = $"{result.Name}:{result.Definition.OptionsName}";
                var optionType = Type.GetType(result.Definition.OptionsFullQualifiedName);
                var section = _configuration.GetSection(optionKey);
                var configuration = section.Get(optionType);
                return new OkObjectResult(Build(result, configuration));
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDetails([FromRoute] string prefix, string id, [FromBody] UpdateIdentityProvisioningDetailsRequest request)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name, _jwtBuilder);
                var result = await _identityProvisioningStore.Query()
                    .Include(p => p.Realms)
                    .Where(p => p.Realms.Any(r => r.Name == prefix))
                    .SingleOrDefaultAsync(p => p.Id == id);
                if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_IDPROVISIONING, id));
                result.Description = request.Description;
                await _identityProvisioningStore.SaveChanges(CancellationToken.None);
                return NoContent();
            }
            catch(OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProperties([FromRoute] string prefix, string id, [FromBody] UpdateIdentityProvisioningPropertiesRequest request)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name, _jwtBuilder);
                var result = await _identityProvisioningStore.Query()
                    .Include(p => p.Realms)
                    .Include(p => p.Definition)
                    .Where(p => p.Realms.Any(r => r.Name == prefix))
                    .SingleOrDefaultAsync(p => p.Id == id);
                if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_IDPROVISIONING, id));
                result.UpdateDateTime = DateTime.UtcNow;
                SyncConfiguration(result, request.Values);
                await _identityProvisioningStore.SaveChanges(CancellationToken.None);
                return NoContent();
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveMapper([FromRoute] string prefix, string id, string mapperId)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name, _jwtBuilder);
                var result = await _identityProvisioningStore.Query()
                    .Include(p => p.Realms)
                    .Include(p => p.Definition).ThenInclude(d => d.MappingRules)
                    .Where(p => p.Realms.Any(r => r.Name == prefix))
                    .SingleOrDefaultAsync(p => p.Id == id);
                if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_IDPROVISIONING, id));
                var mapper = result.Definition.MappingRules.SingleOrDefault(r => r.Id == mapperId);
                if (mapper == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_IDPROVISIONING_MAPPINGRULE, mapperId));
                result.UpdateDateTime = DateTime.UtcNow;
                result.Definition.MappingRules = result.Definition.MappingRules.Where(r => r.Id != mapperId).ToList();
                await _identityProvisioningStore.SaveChanges(CancellationToken.None);
                return NoContent();
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMapper([FromRoute] string prefix, string id, [FromBody] AddIdentityProvisioningMapperRequest request)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name, _jwtBuilder);
                var result = await _identityProvisioningStore.Query()
                    .Include(p => p.Realms)
                    .Include(p => p.Definition).ThenInclude(d => d.MappingRules)
                    .Where(p => p.Realms.Any(r => r.Name == prefix))
                    .SingleOrDefaultAsync(p => p.Id == id);
                if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_IDPROVISIONING, id));
                result.UpdateDateTime = DateTime.UtcNow;
                var record = new IdentityProvisioningMappingRule
                {
                    Id = Guid.NewGuid().ToString(),
                    From = request.From,
                    MapperType = request.MappingRule,
                    TargetUserAttribute = request.TargetUserAttribute,
                    TargetUserProperty = request.TargetUserProperty
                };
                result.Definition.MappingRules.Add(record);
                await _identityProvisioningStore.SaveChanges(CancellationToken.None);
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JsonSerializer.Serialize(Build(record)),
                    ContentType = "application/json"
                };
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpGet]
        public IActionResult Enqueue([FromRoute] string prefix, string name, string id)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name, _jwtBuilder);
            var jobs = _serviceProvider.GetRequiredService<IEnumerable<IRepresentationExtractionJob>>();
            var job = jobs.SingleOrDefault(j => j.Name == name);
            BackgroundJob.Enqueue(() => job.Execute(id, prefix));
            return new NoContentResult();
        }

        [HttpGet]
        public IActionResult Import([FromRoute] string prefix)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name, _jwtBuilder);
            string id = Guid.NewGuid().ToString();
            BackgroundJob.Enqueue<IImportRepresentationJob>((j) => j.Execute(prefix, id));
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JsonSerializer.Serialize(new { id = id }),
                ContentType = "application/json"
            };
        }

        private void SyncConfiguration(IdentityProvisioning identityProvisioning, Dictionary<string, string> values)
        {
            var optionKey = $"{identityProvisioning.Name}:{identityProvisioning.Definition.OptionsName}";
            foreach (var kvp in values)
                _configuration[$"{optionKey}:{kvp.Key}"] = kvp.Value;
        }

        private static IdentityProvisioningResult Build(IdentityProvisioning idProvisioning, object configuration = null)
        {
            var values = new Dictionary<string, string>();
            if (configuration != null)
            {
                var type = configuration.GetType();
                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var property in properties)
                {
                    var value = property.GetValue(configuration)?.ToString();
                    if (!string.IsNullOrWhiteSpace(value)) values.Add(property.Name, value);
                }
            }

            return new IdentityProvisioningResult
            {
                Id = idProvisioning.Id,
                CreateDateTime = idProvisioning.CreateDateTime,
                IsEnabled = idProvisioning.IsEnabled,
                Description = idProvisioning.Description,
                Histories = idProvisioning.Histories?.Select(h => new IdentityProvisioningHistoryResult
                {
                    EndDateTime = h.EndDateTime,
                    ErrorMessage = h.ErrorMessage,
                    FolderName = h.FolderName,
                    NbRepresentations = h.NbRepresentations,
                    StartDateTime = h.StartDateTime,
                    Status = h.Status
                }).ToList(),
                Name = idProvisioning.Name,
                UpdateDateTime = idProvisioning.UpdateDateTime,
                Definition = idProvisioning.Definition == null ? null : new IdentityProvisioningDefinitionResult
                {
                    OptionsName = idProvisioning.Definition.OptionsName,
                    CreateDateTime = idProvisioning.Definition.CreateDateTime,
                    Description = idProvisioning.Definition.Description,
                    Name = idProvisioning.Definition.Name,
                    UpdateDateTime = idProvisioning.Definition.UpdateDateTime,
                    MappingRules = idProvisioning.Definition.MappingRules == null ? new List<IdentityProvisioningMappingRuleResult>() : idProvisioning.Definition.MappingRules.Select(m => Build(m)).ToList()
                },
                Values = values
            };
        }

        private static IdentityProvisioningMappingRuleResult Build(IdentityProvisioningMappingRule mappingRule)
        {
            return new IdentityProvisioningMappingRuleResult
            {
                From = mappingRule.From,
                Id = mappingRule.Id,
                MapperType = mappingRule.MapperType,
                TargetUserAttribute = mappingRule.TargetUserAttribute,
                TargetUserProperty = mappingRule.TargetUserProperty
            };
        }
    }
}
