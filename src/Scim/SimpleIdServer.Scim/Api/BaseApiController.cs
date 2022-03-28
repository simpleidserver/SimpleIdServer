// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Commands;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.ExternalEvents;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Parser;
using SimpleIdServer.Scim.Parser.Exceptions;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    public abstract class BaseApiController : Controller
    {
        private readonly string _resourceType;
        private readonly IAddRepresentationCommandHandler _addRepresentationCommandHandler;
        private readonly IDeleteRepresentationCommandHandler _deleteRepresentationCommandHandler;
        private readonly IReplaceRepresentationCommandHandler _replaceRepresentationCommandHandler;
        private readonly IPatchRepresentationCommandHandler _patchRepresentationCommandHandler;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly IAttributeReferenceEnricher _attributeReferenceEnricher;
        private readonly SCIMHostOptions _options;
        private readonly ILogger _logger;
        private readonly IBusControl _busControl;
        private readonly IResourceTypeResolver _resourceTypeResolver;

        public BaseApiController(string resourceType, IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, IReplaceRepresentationCommandHandler replaceRepresentationCommandHandler, IPatchRepresentationCommandHandler patchRepresentationCommandHandler, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, ISCIMSchemaQueryRepository scimSchemaQueryRepository, IAttributeReferenceEnricher attributeReferenceEnricher, IOptionsMonitor<SCIMHostOptions> options, ILogger logger, IBusControl busControl, IResourceTypeResolver resourceTypeResolver)
        {
            _resourceType = resourceType;
            _addRepresentationCommandHandler = addRepresentationCommandHandler;
            _deleteRepresentationCommandHandler = deleteRepresentationCommandHandler;
            _replaceRepresentationCommandHandler = replaceRepresentationCommandHandler;
            _patchRepresentationCommandHandler = patchRepresentationCommandHandler;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _attributeReferenceEnricher = attributeReferenceEnricher;
            _options = options.CurrentValue;
            _logger = logger;
            _busControl = busControl;
            _resourceTypeResolver = resourceTypeResolver;
        }

        public string ResourceType => _resourceType;

        [HttpGet]
        [Authorize("QueryScimResource")]
        public virtual Task<IActionResult> Get([FromQuery] SearchSCIMResourceParameter searchRequest)
        {
            return InternalSearch(searchRequest);
        }


        [HttpPost(".search")]
        [Authorize("QueryScimResource")]
        public virtual Task<IActionResult> Search([FromBody] SearchSCIMResourceParameter searchRequest)
        {
            return InternalSearch(searchRequest);
        }

        [HttpGet("{id}")]
        [Authorize("QueryScimResource")]
        public virtual Task<IActionResult> Get(string id)
        {
            return InternalGet(id);
        }

        [HttpGet("Me")]
        [Authorize("UserAuthenticated")]
        public virtual Task<IActionResult> GetMe(string id)
        {
            return ExecuteActionIfAuthenticated(() =>
            {
                return InternalGet(id);
            });
        }

        [HttpPost]
        [Authorize("AddScimResource")]
        public virtual Task<IActionResult> Add([FromBody] RepresentationParameter param)
        {
            return InternalAdd(param);
        }

        [HttpPost("Me")]
        [Authorize("UserAuthenticated")]
        public virtual Task<IActionResult> AddMe([FromBody] RepresentationParameter param)
        {
            return ExecuteActionIfAuthenticated(() =>
            {
                return InternalAdd(param);
            });
        }

        [HttpDelete("{id}")]
        [Authorize("DeleteScimResource")]
        public virtual Task<IActionResult> Delete(string id)
        {
            return InternalDelete(id);
        }

        [HttpDelete("Me/{id}")]
        [Authorize("UserAuthenticated")]
        public virtual Task<IActionResult> DeleteMe(string id)
        {
            return ExecuteActionIfAuthenticated(() =>
            {
                return InternalDelete(id);
            });
        }

        [HttpPut("{id}")]
        [Authorize("UpdateScimResource")]
        public virtual Task<IActionResult> Update(string id, [FromBody] RepresentationParameter param)
        {
            return InternalUpdate(id, param);
        }

        [HttpPut("Me/{id}")]
        [Authorize("UserAuthenticated")]
        public virtual Task<IActionResult> UpdateMe(string id, [FromBody] RepresentationParameter param)
        {
            return ExecuteActionIfAuthenticated(() =>
            {
                return InternalUpdate(id, param);
            });
        }

        [HttpPatch("{id}")]
        [Authorize("UpdateScimResource")]
        public virtual Task<IActionResult> Patch(string id, [FromBody] PatchRepresentationParameter patchRepresentation)
        {
            return InternalPatch(id, patchRepresentation);
        }

        [HttpPatch("Me/{id}")]
        [Authorize("UserAuthenticated")]
        public virtual Task<IActionResult> PatchMe(string id, [FromBody] PatchRepresentationParameter patchRepresentation)
        {
            return ExecuteActionIfAuthenticated(() =>
            {
                return InternalPatch(id, patchRepresentation);
            });
        }

        protected async Task<IActionResult> InternalSearch(SearchSCIMResourceParameter searchRequest)
        {
            _logger.LogInformation(Global.StartGetResources);
            try
            {
                if (searchRequest.Count > _options.MaxResults || searchRequest.Count == null)
                {
                    searchRequest.Count = _options.MaxResults;
                }

                var schema = await _scimSchemaQueryRepository.FindRootSCIMSchemaByResourceType(_resourceType);
                var schemaIds = new List<string> { schema.Id };
                schemaIds.AddRange(schema.SchemaExtensions.Select(s => s.Schema));
                var schemas = (await _scimSchemaQueryRepository.FindSCIMSchemaByIdentifiers(schemaIds)).ToList();
                var sortByFilter = SCIMFilterParser.Parse(searchRequest.SortBy, schemas);
                if (searchRequest.StartIndex <= 0)
                {
                    _logger.LogError(Global.StartIndexMustBeSuperiorOrEqualTo1);
                    return this.BuildError(HttpStatusCode.BadRequest, Global.StartIndexMustBeSuperiorOrEqualTo1);
                }

                var standardSchemas = new List<SCIMSchema>
                {
                    StandardSchemas.StandardResponseSchemas
                };
                standardSchemas.AddRange(schemas);
                var includedAttributes = searchRequest.Attributes == null ? new List<SCIMAttributeExpression>() : searchRequest.Attributes.Select(a => SCIMFilterParser.Parse(a, standardSchemas)).Cast<SCIMAttributeExpression>().ToList();
                var excludedAttributes = searchRequest.ExcludedAttributes == null ? new List<SCIMAttributeExpression>() : searchRequest.ExcludedAttributes.Select(a => SCIMFilterParser.Parse(a, standardSchemas)).Cast<SCIMAttributeExpression>().ToList();
                var result = await _scimRepresentationQueryRepository.FindSCIMRepresentations(new SearchSCIMRepresentationsParameter(_resourceType, searchRequest.StartIndex, searchRequest.Count.Value, sortByFilter, searchRequest.SortOrder, SCIMFilterParser.Parse(searchRequest.Filter, schemas), includedAttributes, excludedAttributes));
                var jObj = new JObject
                {
                    { StandardSCIMRepresentationAttributes.Schemas, new JArray(new [] { StandardSchemas.ListResponseSchemas.Id } ) },
                    { StandardSCIMRepresentationAttributes.TotalResults, result.TotalResults },
                    { StandardSCIMRepresentationAttributes.ItemsPerPage, searchRequest.Count },
                    { StandardSCIMRepresentationAttributes.StartIndex, searchRequest.StartIndex }
                };
                var resources = new JArray();
                var baseUrl = Request.GetAbsoluteUriWithVirtualPath();
                var representations = result.Content.ToList();
                foreach(var representation in representations)
                {
                    representation.Schemas = schemas;
                }

                await _attributeReferenceEnricher.Enrich(_resourceType, representations, baseUrl);
                foreach (var record in representations)
                {
                    JObject newJObj = null;
                    var location = $"{baseUrl}/{_resourceTypeResolver.ResolveByResourceType(_resourceType).ControllerName}/{record.Id}";
                    bool includeStandardRequest = true;
                    if(searchRequest.Attributes.Any())
                    {
                        record.AddStandardAttributes(location, searchRequest.Attributes, true, false);
                        includeStandardRequest = false;
                    }
                    else if (searchRequest.ExcludedAttributes.Any())
                    {
                        record.AddStandardAttributes(location, searchRequest.ExcludedAttributes, false, false);
                        includeStandardRequest = false;
                    }
                    else
                    {
                        record.ApplyEmptyArray();
                    }

                    newJObj = record.ToResponse(location, true, includeStandardRequest);
                    resources.Add(newJObj);
                }

                jObj.Add(StandardSCIMRepresentationAttributes.Resources, resources);
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = jObj.ToString(),
                    ContentType = SCIMConstants.STANDARD_SCIM_CONTENT_TYPE
                };
            }
            catch (SCIMFilterException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidFilter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.InternalServerError, ex.ToString(), SCIMConstants.ErrorSCIMTypes.InternalServerError);
            }
        }

        protected async Task<IActionResult> InternalGet(string id)
        {
            _logger.LogInformation(string.Format(Global.StartGetResource, id));
            var representation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(id, _resourceType);
            if (representation == null)
            {
                _logger.LogError(string.Format(Global.ResourceNotFound, id));
                return this.BuildError(HttpStatusCode.NotFound, string.Format(Global.ResourceNotFound, id));
            }

            representation.ApplyEmptyArray();
            await _attributeReferenceEnricher.Enrich(_resourceType, new List<SCIMRepresentation> { representation }, Request.GetAbsoluteUriWithVirtualPath());
            return BuildHTTPResult(representation, HttpStatusCode.OK, true);
        }

        protected async Task<IActionResult> InternalAdd(RepresentationParameter jobj)
        {
            if (jobj == null)
            {
                return this.BuildError(HttpStatusCode.BadRequest, Global.HttpPostNotWellFormatted, SCIMConstants.ErrorSCIMTypes.InvalidSyntax);
            }

            _logger.LogInformation(Global.AddResource);
            try
            {
                var command = new AddRepresentationCommand(_resourceType, jobj, Request.GetAbsoluteUriWithVirtualPath());
                var scimRepresentation = await _addRepresentationCommandHandler.Handle(command);
                var location = GetLocation(scimRepresentation);
                var content = scimRepresentation.ToResponse(location, false);
                if (SCIMConstants.MappingScimResourceTypeToCommonType.ContainsKey(scimRepresentation.ResourceType))
                {
                    await _busControl.Publish(new RepresentationAddedEvent(scimRepresentation.Id, scimRepresentation.Version, SCIMConstants.MappingScimResourceTypeToCommonType[scimRepresentation.ResourceType], content));
                }

                return BuildHTTPResult(HttpStatusCode.Created, location, scimRepresentation.Version, content);
            }
            catch(SCIMSchemaViolatedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.SchemaViolated);
            }
            catch (SCIMBadSyntaxException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidSyntax);
            }
            catch (SCIMUniquenessAttributeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.Conflict, ex.Message, SCIMConstants.ErrorSCIMTypes.Uniqueness);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.InternalServerError, ex.ToString(), SCIMConstants.ErrorSCIMTypes.InternalServerError);
            }
        }

        protected async Task<IActionResult> InternalDelete(string id)
        {
            _logger.LogInformation(string.Format(Global.DeleteResource, id));
            try
            {
                var representation = await _deleteRepresentationCommandHandler.Handle(new DeleteRepresentationCommand(id, _resourceType, Request.GetAbsoluteUriWithVirtualPath()));
                if (SCIMConstants.MappingScimResourceTypeToCommonType.ContainsKey(_resourceType))
                {
                    await _busControl.Publish(new RepresentationRemovedEvent(id, representation.Version, SCIMConstants.MappingScimResourceTypeToCommonType[_resourceType]));
                }

                return new StatusCodeResult((int)HttpStatusCode.NoContent);
            }
            catch (SCIMNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.NotFound, ex.Message, SCIMConstants.ErrorSCIMTypes.Unknown);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.InternalServerError, ex.ToString(), SCIMConstants.ErrorSCIMTypes.InternalServerError);
            }
        }

        protected async Task<IActionResult> InternalUpdate(string id, RepresentationParameter representationParameter)
        {
            if (representationParameter == null)
            {
                return this.BuildError(HttpStatusCode.BadRequest, Global.HttpPutNotWellFormatted, SCIMConstants.ErrorSCIMTypes.InvalidSyntax);
            }

            _logger.LogInformation(Global.UpdateResource, id);
            try
            {
                var newRepresentation = await _replaceRepresentationCommandHandler.Handle(new ReplaceRepresentationCommand(id, _resourceType, representationParameter, Request.GetAbsoluteUriWithVirtualPath()));
                var location = GetLocation(newRepresentation);
                var content = newRepresentation.ToResponse(location, false);
                if (SCIMConstants.MappingScimResourceTypeToCommonType.ContainsKey(_resourceType))
                {
                    await _busControl.Publish(new RepresentationUpdatedEvent(newRepresentation.Id, newRepresentation.Version, SCIMConstants.MappingScimResourceTypeToCommonType[_resourceType], content));
                }

                return BuildHTTPResult(HttpStatusCode.OK, location, newRepresentation.Version, content);
            }
            catch (SCIMUniquenessAttributeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.Conflict, ex.Message, SCIMConstants.ErrorSCIMTypes.Uniqueness);
            }
            catch (SCIMSchemaViolatedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.SchemaViolated);
            }
            catch (SCIMBadSyntaxException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidSyntax);
            }
            catch(SCIMImmutableAttributeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.Mutability);
            }
            catch(SCIMNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.NotFound, ex.Message, SCIMConstants.ErrorSCIMTypes.Unknown);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.InternalServerError, ex.ToString(), SCIMConstants.ErrorSCIMTypes.InternalServerError);
            }
        }

        protected async Task<IActionResult> InternalPatch(string id, PatchRepresentationParameter patchRepresentation)
        {
            _logger.LogInformation(string.Format(Global.PatchResource, id, patchRepresentation == null ? string.Empty : JsonConvert.SerializeObject(patchRepresentation)));
            try
            {
                var newRepresentation = await _patchRepresentationCommandHandler.Handle(new PatchRepresentationCommand(id, ResourceType, patchRepresentation, Request.GetAbsoluteUriWithVirtualPath()));
                var location = GetLocation(newRepresentation);
                var content = newRepresentation.ToResponse(location, false);
                if (SCIMConstants.MappingScimResourceTypeToCommonType.ContainsKey(_resourceType))
                {
                    await _busControl.Publish(new RepresentationUpdatedEvent(newRepresentation.Id, newRepresentation.Version, SCIMConstants.MappingScimResourceTypeToCommonType[_resourceType], content));
                }

                return BuildHTTPResult(HttpStatusCode.OK, location, newRepresentation.Version, content);
            }
            catch (SCIMDuplicateAttributeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.NoContent, ex.Message, SCIMConstants.ErrorSCIMTypes.Uniqueness);
            }
            catch (SCIMUniquenessAttributeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.Conflict, ex.Message, SCIMConstants.ErrorSCIMTypes.Uniqueness);
            }
            catch (SCIMFilterException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidFilter);
            }
            catch (SCIMBadSyntaxException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidSyntax);
            }
            catch (SCIMNoTargetException ex)
            {
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.NoTarget);
            }
            catch (SCIMNotFoundException ex)
            {
                return this.BuildError(HttpStatusCode.NotFound, ex.Message, SCIMConstants.ErrorSCIMTypes.Unknown);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.InternalServerError, ex.ToString(), SCIMConstants.ErrorSCIMTypes.InternalServerError);
            }
        }

        protected async Task<IActionResult> ExecuteActionIfAuthenticated(Func<Task<IActionResult>> callback)
        {
            var user = User.Claims.FirstOrDefault(c => c.Type == _options.SCIMIdClaimName);
            if (user == null)
            {
                return this.BuildError(HttpStatusCode.BadRequest, $"{_options.SCIMIdClaimName} claim cannot be retrieved", SCIMConstants.ErrorSCIMTypes.InvalidSyntax);
            }

            return await callback();
        }

        protected IActionResult BuildHTTPResult(SCIMRepresentation representation, HttpStatusCode status, bool isGetRequest)
        {
            var location = GetLocation(representation);
            var content = representation.ToResponse(location, isGetRequest);
            return BuildHTTPResult(status, location, representation.Version, content);
        }

        protected IActionResult BuildHTTPResult(HttpStatusCode status, string location, int version, JObject content)
        {
            HttpContext.Response.Headers.Add("Location", location);
            HttpContext.Response.Headers.Add("ETag", version.ToString());
            return new ContentResult
            {
                StatusCode = (int)status,
                Content = content.ToString(),
                ContentType = SCIMConstants.STANDARD_SCIM_CONTENT_TYPE
            };
        }

        protected string GetLocation(SCIMRepresentation representation)
        {
            return $"{Request.GetAbsoluteUriWithVirtualPath()}/{_resourceTypeResolver.ResolveByResourceType(representation.ResourceType).ControllerName}/{representation.Id}";
        }
    }
}
