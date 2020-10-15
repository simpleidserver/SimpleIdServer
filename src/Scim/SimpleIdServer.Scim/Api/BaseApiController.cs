// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Commands;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
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

        public BaseApiController(string resourceType, IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, IReplaceRepresentationCommandHandler replaceRepresentationCommandHandler, IPatchRepresentationCommandHandler patchRepresentationCommandHandler, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, ISCIMSchemaQueryRepository scimSchemaQueryRepository, IAttributeReferenceEnricher attributeReferenceEnricher, IOptionsMonitor<SCIMHostOptions> options, ILogger logger)
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
        }

        public string ResourceType => _resourceType;

        [HttpGet]
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
        public virtual Task<IActionResult> Patch(string id, [FromBody] JObject jObj)
        {
            return InternalPatch(id, jObj);
        }

        [HttpPatch("Me/{id}")]
        [Authorize("UserAuthenticated")]
        public virtual Task<IActionResult> PatchMe(string id, [FromBody] JObject jObj)
        {
            return ExecuteActionIfAuthenticated(() =>
            {
                return InternalPatch(id, jObj);
            });
        }

        private async Task<IActionResult> InternalSearch(SearchSCIMResourceParameter searchRequest)
        {
            _logger.LogInformation(Global.StartGetResources);
            try
            {
                if (searchRequest.Count > _options.MaxResults)
                {
                    searchRequest.Count = _options.MaxResults;
                }

                var schema = await _scimSchemaQueryRepository.FindRootSCIMSchemaByResourceType(_resourceType);
                var schemaIds = new List<string> { schema.Id };
                schemaIds.AddRange(schema.SchemaExtensions.Select(s => s.Schema));
                var schemas = (await _scimSchemaQueryRepository.FindSCIMSchemaByIdentifiers(schemaIds)).ToList();
                var result = await _scimRepresentationQueryRepository.FindSCIMRepresentations(new SearchSCIMRepresentationsParameter(_resourceType, searchRequest.StartIndex - 1, searchRequest.Count, searchRequest.SortBy, searchRequest.SortOrder, SCIMFilterParser.Parse(searchRequest.Filter, schemas)));
                var jObj = new JObject
                {
                    { SCIMConstants.StandardSCIMRepresentationAttributes.Schemas, new JArray(new [] { SCIMConstants.StandardSchemas.ListResponseSchemas.Id } ) },
                    { SCIMConstants.StandardSCIMRepresentationAttributes.TotalResults, result.TotalResults },
                    { SCIMConstants.StandardSCIMRepresentationAttributes.ItemsPerPage, searchRequest.Count },
                    { SCIMConstants.StandardSCIMRepresentationAttributes.StartIndex, searchRequest.StartIndex }
                };
                var resources = new JArray();
                var baseUrl = Request.GetAbsoluteUriWithVirtualPath();
                var representations = result.Content.ToList();
                await _attributeReferenceEnricher.Enrich(_resourceType, representations, baseUrl);
                foreach (var record in representations)
                {
                    JObject newJObj = null;
                    var location = $"{baseUrl}/{_resourceType}/{record.Id}";
                    if (searchRequest.Attributes.Any())
                    {
                        newJObj = record.ToResponseWithIncludedAttributes(searchRequest.Attributes.Select(a => SCIMFilterParser.Parse(a, schemas)).ToList());
                    }
                    else if (searchRequest.ExcludedAttributes.Any())
                    {
                        newJObj = record.ToResponseWithExcludedAttributes(searchRequest.ExcludedAttributes.Select(a => SCIMFilterParser.Parse(a, schemas)).ToList(), location);
                    }
                    else
                    {
                        newJObj = record.ToResponse(location, true);
                    }

                    resources.Add(newJObj);
                }

                jObj.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Resources, resources);
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

        private async Task<IActionResult> InternalGet(string id)
        {
            _logger.LogInformation(string.Format(Global.StartGetResource, id));
            var representation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(id, _resourceType);
            if (representation == null)
            {
                _logger.LogError(string.Format(Global.ResourceNotFound, id));
                return this.BuildError(HttpStatusCode.NotFound, string.Format(Global.ResourceNotFound, id));
            }

            await _attributeReferenceEnricher.Enrich(_resourceType, new List<SCIMRepresentation> { representation }, Request.GetAbsoluteUriWithVirtualPath());
            return BuildHTTPResult(representation, HttpStatusCode.OK, true);
        }

        private async Task<IActionResult> InternalAdd(RepresentationParameter jobj)
        {
            _logger.LogInformation(string.Format(Global.AddResource, jobj.ToString()));
            try
            {
                var command = new AddRepresentationCommand(_resourceType, jobj);
                var scimRepresentation = await _addRepresentationCommandHandler.Handle(command);
                return BuildHTTPResult(scimRepresentation, HttpStatusCode.Created, false);
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

        private async Task<IActionResult> InternalDelete(string id)
        {
            _logger.LogInformation(string.Format(Global.DeleteResource, id));
            try
            {
                await _deleteRepresentationCommandHandler.Handle(new DeleteRepresentationCommand(id, _resourceType));
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

        private async Task<IActionResult> InternalUpdate(string id, RepresentationParameter representationParameter)
        {
            _logger.LogInformation(string.Format(Global.UpdateResource, id));
            try
            {
                var newRepresentation = await _replaceRepresentationCommandHandler.Handle(new ReplaceRepresentationCommand(id, _resourceType, representationParameter));
                return BuildHTTPResult(newRepresentation, HttpStatusCode.OK, false);
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

        private async Task<IActionResult> InternalPatch(string id, JObject jObj)
        {
            _logger.LogInformation(string.Format(Global.PatchResource, id));
            try
            {
                var newRepresentation = await _patchRepresentationCommandHandler.Handle(new PatchRepresentationCommand(id, jObj));
                return BuildHTTPResult(newRepresentation, HttpStatusCode.OK, false);
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

        private async Task<IActionResult> ExecuteActionIfAuthenticated(Func<Task<IActionResult>> callback)
        {
            var user = User.Claims.FirstOrDefault(c => c.Type == _options.SCIMIdClaimName);
            if (user == null)
            {
                return this.BuildError(HttpStatusCode.BadRequest, $"{_options.SCIMIdClaimName} claim cannot be retrieved", SCIMConstants.ErrorSCIMTypes.InvalidSyntax);
            }

            return await callback();
        }

        private IActionResult BuildHTTPResult(SCIMRepresentation representation, HttpStatusCode status, bool isGetRequest)
        {
            var location = $"{Request.GetAbsoluteUriWithVirtualPath()}/{_resourceType}/{representation.Id}";
            HttpContext.Response.Headers.Add("Location", location);
            HttpContext.Response.Headers.Add("ETag", representation.Version);
            return new ContentResult
            {
                StatusCode = (int)status,
                Content = representation.ToResponse(location, isGetRequest).ToString(),
                ContentType = SCIMConstants.STANDARD_SCIM_CONTENT_TYPE
            };
        }
    }
}
