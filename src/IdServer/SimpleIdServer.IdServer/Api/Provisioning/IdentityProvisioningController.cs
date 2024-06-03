// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Provisioning;
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

namespace SimpleIdServer.IdServer.Api.Provisioning
{
    public class IdentityProvisioningController : BaseController
    {
        private readonly IBusControl _busControl;
        private readonly IIdentityProvisioningStore _identityProvisioningStore;
        private readonly IConfiguration _configuration;
        private readonly IEnumerable<IProvisioningService> _provisioningServices;
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IMessageBusErrorStore _messageBrokerErrorStore;

        public IdentityProvisioningController(
            IBusControl busControl,
            IIdentityProvisioningStore identityProvisioningStore, 
            ITokenRepository tokenRepository, 
            IJwtBuilder jwtBuilder, 
            IConfiguration configuration, 
            IEnumerable<IProvisioningService> provisioningServices,
            ITransactionBuilder transactionBuilder,
            IMessageBusErrorStore messageBusErrorStore) : base(tokenRepository, jwtBuilder)
        {
            _busControl = busControl;
            _identityProvisioningStore = identityProvisioningStore;
            _configuration = configuration;
            _provisioningServices = provisioningServices;
            _transactionBuilder = transactionBuilder;
            _messageBrokerErrorStore = messageBusErrorStore;
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name);
                var result = await _identityProvisioningStore.Search(prefix, request, cancellationToken);
                return new OkObjectResult(new SearchResult<IdentityProvisioningResult>
                {
                    Count = result.Count,
                    Content = result.Content.Select(p => Build(p)).ToList()
                });
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
                await CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name);
                var result = await _identityProvisioningStore.Get(prefix, id, cancellationToken);
                if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioning, id));
                var optionKey = $"{result.Name}:{result.Definition.OptionsName}";
                var optionType = Type.GetType(result.Definition.OptionsFullQualifiedName);
                var section = _configuration.GetSection(optionKey);
                var configuration = section.Get(optionType);
                var provisioningService = _provisioningServices.Single(p => p.Name == result.Definition.Name);
                var externalIds = result.Processes.Select(h => h.Id).ToList();
                var messageErrors = await _messageBrokerErrorStore.GetAllByExternalId(externalIds, cancellationToken);
                var res = Build(result, configuration);
                res.Processes.ForEach(p =>
                {
                    var filteredMessageErrors = messageErrors.Where(e => e.ExternalId == p.Id);
                    if (filteredMessageErrors.Any()) 
                    {
                        p.Errors = filteredMessageErrors.Select(e => new IdentityProvisioningProcessMessageErrorResult
                        {
                            Id = e.Id,
                            Exceptions = e.Exceptions
                        }).ToList();
                    }
                });
                return new OkObjectResult(res);
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDetails([FromRoute] string prefix, string id, [FromBody] UpdateIdentityProvisioningDetailsRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name);
                    var result = await _identityProvisioningStore.Get(prefix, id, cancellationToken);
                    if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioning, id));
                    result.Description = request.Description;
                    _identityProvisioningStore.Update(result);
                    await transaction.Commit(cancellationToken);
                    return NoContent();
                }
            }
            catch(OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProperties([FromRoute] string prefix, string id, [FromBody] UpdateIdentityProvisioningPropertiesRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name);
                    var result = await _identityProvisioningStore.Get(prefix, id, cancellationToken);
                    if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioning, id));
                    if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioning, id));
                    result.UpdateDateTime = DateTime.UtcNow;
                    SyncConfiguration(result, request.Values);
                    _identityProvisioningStore.Update(result);
                    await transaction.Commit(cancellationToken);
                    return NoContent();
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
                    await CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name);
                    var result = await _identityProvisioningStore.Get(prefix, id, cancellationToken);
                    if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioning, id));
                    var mapper = result.Definition.MappingRules.SingleOrDefault(r => r.Id == mapperId);
                    if (mapper == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioningMappingRule, mapperId));
                    result.UpdateDateTime = DateTime.UtcNow;
                    result.Definition.MappingRules = result.Definition.MappingRules.Where(r => r.Id != mapperId).ToList();
                    _identityProvisioningStore.Update(result);
                    _identityProvisioningStore.Update(result.Definition);
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
        public async Task<IActionResult> AddMapper([FromRoute] string prefix, string id, [FromBody] AddIdentityProvisioningMapperRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name);
                    var result = await _identityProvisioningStore.Get(prefix, id, cancellationToken);
                    if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioning, id));
                    if (IProvisioningMappingRule.IsUnique(request.MappingRule) && result.Definition.MappingRules.Any(r => r.MapperType == request.MappingRule))
                        return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.IdProvisioningTypeUnique);

                    // TODO : Check GROUPNAME can be added when usage is GROUP.
                    // TODO : Check SUBJECT or USERATTRIBUTE can be added when usage is USER.

                    result.UpdateDateTime = DateTime.UtcNow;
                    var record = new IdentityProvisioningMappingRule
                    {
                        Id = Guid.NewGuid().ToString(),
                        From = request.From,
                        MapperType = request.MappingRule,
                        TargetUserAttribute = request.TargetUserAttribute,
                        TargetUserProperty = request.TargetUserProperty,
                        HasMultipleAttribute = request.HasMultipleAttribute,
                        Usage = request.Usage
                    };
                    result.Definition.MappingRules.Add(record);
                    _identityProvisioningStore.Update(result);
                    _identityProvisioningStore.Update(result.Definition);
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

        [HttpPut]
        public async Task<IActionResult> UpdateMapper([FromRoute] string prefix, string id, string mapperId, [FromBody] UpdateIdentityProvisioningMapperRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name);
                    var result = await _identityProvisioningStore.Get(prefix, id, cancellationToken);
                    if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioning, id));
                    var mapperRule = result.Definition.MappingRules.SingleOrDefault(r => r.Id == mapperId);
                    if (mapperRule == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioningMappingRule, id));
                    result.UpdateDateTime = DateTime.UtcNow;
                    mapperRule.From = request.From;
                    mapperRule.TargetUserAttribute = request.TargetUserAttribute;
                    mapperRule.TargetUserProperty = request.TargetUserProperty;
                    mapperRule.HasMultipleAttribute = request.HasMultipleAttribute;
                    _identityProvisioningStore.Update(result);
                    _identityProvisioningStore.Update(result.Definition);
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
        public async Task<IActionResult> GetMapper([FromRoute] string prefix, string id, string mapperId, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name);
                var result = await _identityProvisioningStore.Get(prefix, id, cancellationToken);
                if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioning, id));
                var mapperRule = result.Definition.MappingRules.SingleOrDefault(r => r.Id == mapperId);
                if (mapperRule == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioningMappingRule, id));
                return new OkObjectResult(Build(mapperRule));
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestConnection([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name);
                var result = await _identityProvisioningStore.Get(prefix, id, cancellationToken);
                if (result == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioning, id));
                var provisioningService = _provisioningServices.Single(s => s.Name == result.Definition.Name);
                var extractionResult = await provisioningService.ExtractTestData(result.Definition, cancellationToken);
                var users = new List<IdentityProvisioningExtractionResult>();
                var groups = new List<IdentityProvisioningExtractionResult>();
                var assignedGroups = new List<AssignedGroupResult>();
                if (extractionResult != null)
                {
                    users = extractionResult.Users.Select(u => new IdentityProvisioningExtractionResult
                    {
                        Id = u.Id,
                        Values = u.Values,
                        Version = u.Version
                    }).ToList();
                    groups = extractionResult.Groups.Select(u => new IdentityProvisioningExtractionResult
                    {
                        Id = u.Id,
                        Values = u.Values,
                        Version = u.Version
                    }).ToList();
                    foreach (var user in extractionResult.Users.Where(u => u.GroupIds != null && u.GroupIds.Any()))
                    {
                        assignedGroups.AddRange(user.GroupIds.Select(g => new AssignedGroupResult
                        {
                            UserId = user.Id,
                            GroupId = g
                        }));
                    }
                }

                return new OkObjectResult(new TestConnectionResult
                {
                    Users = Build(result.Definition, users, IdentityProvisioningMappingUsage.USER),
                    Groups = Build(result.Definition, groups, IdentityProvisioningMappingUsage.GROUP),
                    AssignedGroups = assignedGroups
                });
            }
            catch(OAuthException ex)
            {
                return BuildError(ex);
            }
            catch (Exception ex)
            {
                return BuildError(ex);
            }

            TestConnectionRecordsResult Build(IdentityProvisioningDefinition definition, List<IdentityProvisioningExtractionResult> values, IdentityProvisioningMappingUsage usage)
            {
                var columns = new List<string> { "Id", "Version" };
                columns.AddRange(definition.MappingRules.Where(r => r.Usage == usage).Select(r =>
                {
                    if (r.MapperType == MappingRuleTypes.USERATTRIBUTE) return r.TargetUserAttribute;
                    if (r.MapperType == MappingRuleTypes.USERPROPERTY) return r.TargetUserProperty;
                    return "Subject";
                }));
                var result = new TestConnectionRecordsResult
                {
                    Columns = columns,
                    Values = values
                };
                return result;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Extract([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name);
                    var result = await _identityProvisioningStore.Get(prefix, id, cancellationToken);
                    if (result == null)
                        return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioning, id));
                    var provisioningService = _provisioningServices.Single(s => s.Name == result.Definition.Name);
                    await provisioningService.ExtractTestData(result.Definition, cancellationToken);

                    var processId = result.Launch();
                    _identityProvisioningStore.Update(result);
                    await transaction.Commit(cancellationToken);
                    var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{ExtractUsersConsumer.Queuename}"));
                    await sendEndpoint.Send(new StartExtractUsersCommand
                    {
                        InstanceId = id,
                        Realm = prefix,
                        ProcessId = processId
                    });
                    return new ContentResult
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Content = JsonSerializer.Serialize(new IdentityProvisioningLaunchedResult { Id = processId }),
                        ContentType = "application/json"
                    };
                }
            }
            catch(OAuthException ex)
            {
                return BuildError(ex);
            }
            catch(Exception ex)
            {
                return BuildError(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Import([FromRoute] string prefix, string id, string processId, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckAccessToken(prefix, Constants.StandardScopes.Provisioning.Name);
                var result = await _identityProvisioningStore.Get(prefix, id, cancellationToken);
                if (result == null) 
                    return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioning, id));
                var process = result.GetProcess(processId);
                if (process == null) 
                    return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownIdProvisioningProcess, processId));
                if (!process.IsExported)
                    return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.IdProvisioningProcessIsNotExtracted);
                if(process.StartImportDateTime != null)
                    return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.IdProvisioningProcessStarted);

                var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{ImportUsersConsumer.Queuename}"));
                await sendEndpoint.Send(new StartImportUsersCommand
                {
                    InstanceId = id,
                    Realm = prefix,
                    ProcessId = processId
                });
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JsonSerializer.Serialize(new { id = id }),
                    ContentType = "application/json"
                };
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
            catch (Exception ex)
            {
                return BuildError(ex);
            }
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
                Processes = idProvisioning.Processes?.Select(h => new IdentityProvisioningProcessResult
                {
                    Id = h.Id,
                    IsExported = h.IsExported,
                    IsImported = h.IsImported,
                    NbExtractedPages = h.NbExtractedPages,
                    NbImportedPages = h.NbImportedPages,
                    TotalPageToExtract = h.TotalPageToExtract,
                    TotalPageToImport = h.TotalPageToImport,
                    EndExportDateTime = h.EndExportDateTime,
                    EndImportDateTime = h.EndImportDateTime,
                    NbExtractedGroups = h.NbExtractedGroups,
                    NbExtractedUsers = h.NbExtractedUsers,
                    NbFilteredRepresentations = h.NbFilteredRepresentations,
                    NbImportedGroups = h.NbImportedGroups,
                    NbImportedUsers = h.NbImportedUsers,
                    StartExportDateTime = h.StartExportDateTime,
                    StartImportDateTime = h.StartImportDateTime,
                    CreateDateTime = h.CreateDateTime
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
                TargetUserProperty = mappingRule.TargetUserProperty,
                HasMultipleAttribute = mappingRule.HasMultipleAttribute,
                Usage = mappingRule.Usage
            };
        }
    }
}
