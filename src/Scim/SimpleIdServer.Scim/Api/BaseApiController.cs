// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
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
using SimpleIdServer.Scim.Parser.Exceptions;
using SimpleIdServer.Scim.Queries;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
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
        private readonly ISearchRepresentationsQueryHandler _searchRepresentationsQueryHandler;
        private readonly IGetRepresentationQueryHandler _getRepresentationQueryHandler;
        private readonly IAttributeReferenceEnricher _attributeReferenceEnricher;
        private readonly SCIMHostOptions _options;
        private readonly ILogger _logger;
        private readonly IBusControl _busControl;
        private readonly IResourceTypeResolver _resourceTypeResolver;
        private readonly IUriProvider _uriProvider;

        public BaseApiController(string resourceType, IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, IReplaceRepresentationCommandHandler replaceRepresentationCommandHandler, IPatchRepresentationCommandHandler patchRepresentationCommandHandler,
            ISearchRepresentationsQueryHandler searchRepresentationsQueryHandler, IGetRepresentationQueryHandler getRepresentationQueryHandler, IAttributeReferenceEnricher attributeReferenceEnricher, IOptionsMonitor<SCIMHostOptions> options, ILogger logger, IBusControl busControl, IResourceTypeResolver resourceTypeResolver, IUriProvider uriProvider)
        {
            _resourceType = resourceType;
            _addRepresentationCommandHandler = addRepresentationCommandHandler;
            _deleteRepresentationCommandHandler = deleteRepresentationCommandHandler;
            _replaceRepresentationCommandHandler = replaceRepresentationCommandHandler;
            _patchRepresentationCommandHandler = patchRepresentationCommandHandler;
            _searchRepresentationsQueryHandler = searchRepresentationsQueryHandler;
            _getRepresentationQueryHandler = getRepresentationQueryHandler;
            _attributeReferenceEnricher = attributeReferenceEnricher;
            _options = options.CurrentValue;
            _logger = logger;
            _busControl = busControl;
            _resourceTypeResolver = resourceTypeResolver;
            _uriProvider = uriProvider;
        }

        public string ResourceType => _resourceType;
        protected virtual bool IsPublishEvtsEnabled => true;
        protected SCIMHostOptions Options => _options;

        /// <summary>
        /// Search representations according to the filter, sort and pagination parameters (HTTP GET).
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <response code="200">Valid representations are found</response>
        /// <response code="400">Request is unparsable, syntactically incorrect, or violates schema.</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Valid representations are not found</response>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [HttpGet]
        [Authorize("QueryScimResource")]
        public virtual Task<IActionResult> Get([FromRoute] string prefix, [FromQuery] SearchSCIMResourceParameter searchRequest, CancellationToken cancellationToken)
        {
            if (Request.Query != null && Request.Query.TryGetValue(SCIMConstants.StandardSCIMSearchAttributes.Filter, out StringValues str) && string.IsNullOrWhiteSpace(searchRequest.Filter)) searchRequest.Filter = string.Empty;
            return InternalSearch(searchRequest, cancellationToken);
        }

        /// <summary>
        /// Search representations according to the filter, sort and pagination parameters (HTTP POST).
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <response code="200">Valid representations are found</response>
        /// <response code="400">Request is unparsable, syntactically incorrect, or violates schema.</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Valid representations are not found</response>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [HttpPost(".search")]
        [Authorize("QueryScimResource")]
        public virtual Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchSCIMResourceParameter searchRequest, CancellationToken cancellationToken)
        {
            return InternalSearch(searchRequest, cancellationToken);
        }

        /// <summary>
        /// Returns the representation details of a particular representation using its unique ID.
        /// </summary>
        /// <response code="200">Valid representation is found</response>
        /// <response code="400">Request is unparsable, syntactically incorrect, or violates schema.</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Valid representation is not found</response>
        /// <param name="id">Unique identifier of the resource type.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        [Authorize("QueryScimResource")]
        public virtual Task<IActionResult> Get([FromRoute] string prefix, string id, [FromQuery] GetSCIMResourceRequest parameter, CancellationToken cancellationToken)
        {
            return InternalGet(id, parameter, cancellationToken);
        }

        /// <summary>
        /// The operation can be executed only by authenticated user.
        /// Returns the representation details of a particular representation using its unique ID.
        /// </summary>
        /// <param name="id">Unique identifier of the resource type.</param>
        /// <response code="200">Valid representation is found</response>
        /// <response code="400">Request is unparsable, syntactically incorrect, or violates schema.</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Valid representation is not found</response>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [HttpGet("Me")]
        [Authorize("UserAuthenticated")]
        public virtual Task<IActionResult> GetMe([FromRoute] string prefix, string id, [FromQuery] GetSCIMResourceRequest parameter, CancellationToken cancellationToken)
        {
            return ExecuteActionIfAuthenticated(() =>
            {
                return InternalGet(id, parameter, cancellationToken);
            });
        }

        /// <summary>
        /// Create representation and returns the details of the created representation including its unique ID.
        /// </summary>
        /// <response code="201">Valid representation is created</response>
        /// <response code="400">Request is not valid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="409">At least one representation exists with the same unique attribute</response>
        /// <response code="500">Something goes wrong in the server</response>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize("AddScimResource")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public virtual Task<IActionResult> Add([FromRoute] string prefix, [FromBody] RepresentationParameter param)
        {
            return InternalAdd(param);
        }

        /// <summary>
        /// The operation can be executed only by authenticated user.
        /// Create representation and returns the details of the created representation including its unique ID.
        /// </summary>
        /// <response code="201">Valid representation is created</response>
        /// <response code="400">Request is not valid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="409">At least one representation exists with the same unique attribute</response>
        /// <response code="500">Something goes wrong in the server</response>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("Me")]
        [Authorize("UserAuthenticated")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public virtual Task<IActionResult> AddMe([FromRoute] string prefix, [FromBody] RepresentationParameter param)
        {
            return ExecuteActionIfAuthenticated(() =>
            {
                return InternalAdd(param);
            });
        }

        /// <summary>
        /// Delete a particular representation using its unique ID.
        /// </summary>
        /// <response code="204">Representation is deleted</response>
        /// <response code="400">Request is unparsable, syntactically incorrect, or violates schema.</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Representation is not found</response>
        /// <response code="500">Something goes wrong in the server</response>
        /// <param name="id">Unique ID of the resource type</param>
        /// <returns></returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpDelete("{id}")]
        [Authorize("DeleteScimResource")]
        public virtual Task<IActionResult> Delete([FromRoute] string prefix, string id)
        {
            return InternalDelete(id);
        }

        /// <summary>
        /// The operation can be executed only by authenticated user.
        /// Delete a particular representation using its unique ID.
        /// </summary>
        /// <response code="204">Representation is deleted</response>
        /// <response code="400">Request is unparsable, syntactically incorrect, or violates schema.</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Representation is not found</response>
        /// <response code="500">Something goes wrong in the server</response>
        /// <param name="id">Unique ID of the resource type</param>
        /// <returns></returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpDelete("Me/{id}")]
        [Authorize("UserAuthenticated")]
        public virtual Task<IActionResult> DeleteMe([FromRoute] string prefix, string id)
        {
            return ExecuteActionIfAuthenticated(() =>
            {
                return InternalDelete(id);
            });
        }

        /// <summary>
        /// Update the representation details and returns the updated representation details using a PUT operation.
        /// </summary>
        /// <response code="200">Representation is updated</response>
        /// <response code="400">Request is not valid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Representation is not found</response>
        /// <response code="409">At least one representation exists with the same unique attribute</response>
        /// <response code="500">Something goes wrong in the server</response>
        /// <param name="id">Unique ID of the resource type</param>
        /// <param name="param"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        [HttpPut("{id}")]
        [Authorize("UpdateScimResource")]
        public virtual Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] RepresentationParameter param)
        {
            return InternalUpdate(id, param);
        }

        /// <summary>
        /// The operation can be executed only by authenticated user.
        /// Update the representation details and returns the updated representation details using a PUT operation.
        /// </summary>
        /// <response code="200">Representation is updated</response>
        /// <response code="400">Request is not valid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Representation is not found</response>
        /// <response code="409">At least one representation exists with the same unique attribute</response>
        /// <response code="500">Something goes wrong in the server</response>
        /// <param name="id">Unique ID of the resource type</param>
        /// <param name="param"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        [HttpPut("Me/{id}")]
        [Authorize("UserAuthenticated")]
        public virtual Task<IActionResult> UpdateMe([FromRoute] string prefix, string id, [FromBody] RepresentationParameter param)
        {
            return ExecuteActionIfAuthenticated(() =>
            {
                return InternalUpdate(id, param);
            });
        }


        /// <summary>
        /// Update the representation details and returns the updated representation details using a PATCH operation.
        /// </summary>
        /// <response code="200">Representation is updated</response>
        /// <response code="204">No changes have been made.</response>
        /// <response code="400">Request is not valid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Representation is not found</response>
        /// <response code="409">At least one representation exists with the same unique attribute</response>
        /// <response code="500">Something goes wrong in the server</response>
        /// <param name="id">Unique ID of the resource type</param>
        /// <param name="patchRepresentation"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        [HttpPatch("{id}")]
        [Authorize("UpdateScimResource")]
        public virtual Task<IActionResult> Patch([FromRoute] string prefix, string id, [FromBody] PatchRepresentationParameter patchRepresentation)
        {
            return InternalPatch(id, patchRepresentation);
        }

        /// <summary>
        /// The operation can be executed only by authenticated user.
        /// Update the representation details and returns the updated representation details using a PATCH operation.
        /// </summary>
        /// <response code="200">Representation is updated</response>
        /// <response code="204">No changes have been made.</response>
        /// <response code="400">Request is not valid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Representation is not found</response>
        /// <response code="409">At least one representation exists with the same unique attribute</response>
        /// <response code="500">Something goes wrong in the server</response>
        /// <param name="id">Unique ID of the resource type</param>
        /// <param name="patchRepresentation"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        [HttpPatch("Me/{id}")]
        [Authorize("UserAuthenticated")]
        public virtual Task<IActionResult> PatchMe([FromRoute] string prefix, string id, [FromBody] PatchRepresentationParameter patchRepresentation)
        {
            return ExecuteActionIfAuthenticated(() =>
            {
                return InternalPatch(id, patchRepresentation);
            });
        }

        protected async Task<IActionResult> InternalSearch(SearchSCIMResourceParameter searchRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation(Global.StartGetResources);
            try
            {
                var searchResult = await _searchRepresentationsQueryHandler.Handle(searchRequest, _resourceType, cancellationToken);
                if (searchResult.HasError) return this.BuildError(searchResult);
                var result = searchResult.Result;
                var jObj = new JObject
                {
                    { StandardSCIMRepresentationAttributes.Schemas, new JArray(new [] { StandardSchemas.ListResponseSchemas.Id } ) },
                    { StandardSCIMRepresentationAttributes.TotalResults, result.TotalResults },
                    { StandardSCIMRepresentationAttributes.ItemsPerPage, searchRequest.Count },
                    { StandardSCIMRepresentationAttributes.StartIndex, searchRequest.StartIndex }
                };
                var resources = new JArray();
                var baseUrl = _uriProvider.GetAbsoluteUriWithVirtualPath();
                var representations = result.Content.ToList();
                await _attributeReferenceEnricher.Enrich(_resourceType, representations, baseUrl);
                foreach (var record in representations)
                {
                    JObject newJObj = null;
                    var location = $"{baseUrl}/{_resourceTypeResolver.ResolveByResourceType(_resourceType).ControllerName}/{record.Id}";
                    bool includeStandardRequest = true;
                    if (searchRequest.Attributes.Any())
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

                    newJObj = record.ToResponse(location, true, includeStandardRequest, mergeExtensionAttributes: _options.MergeExtensionAttributes);
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
            catch (SCIMNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.NotFound, ex.Message, SCIMConstants.ErrorSCIMTypes.Unknown);
            }
            catch (SCIMSchemaViolatedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidValue);
            }
            catch (SCIMBadSyntaxException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidSyntax);
            }
            catch (SCIMFilterException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidFilter);
            }
            catch (Exception ex)
            {
                return await Handle(ex, cancellationToken);
            }
        }

        protected async Task<IActionResult> InternalGet(string id, GetSCIMResourceRequest parameter, CancellationToken cancellationToken)
        {
            _logger.LogInformation(string.Format(Global.StartGetResource, id));
            try
            {
                var getRepresentationResult = await _getRepresentationQueryHandler.Handle(id, parameter, _resourceType, cancellationToken);
                if (getRepresentationResult.HasError) return this.BuildError(getRepresentationResult);
                var representation = getRepresentationResult.Result;
                await _attributeReferenceEnricher.Enrich(_resourceType, new List<SCIMRepresentation> { representation }, _uriProvider.GetAbsoluteUriWithVirtualPath());
                return BuildHTTPResult(representation, HttpStatusCode.OK, true);
            }
            catch (SCIMSchemaViolatedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidValue);
            }
            catch (SCIMNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.NotFound, ex.Message, SCIMConstants.ErrorSCIMTypes.Unknown);
            }
            catch (Exception ex)
            {
                return await Handle(ex, cancellationToken);
            }
        }

        protected async Task<IActionResult> InternalAdd(RepresentationParameter jobj)
        {
            if (jobj == null) return this.BuildError(HttpStatusCode.BadRequest, Global.HttpPostNotWellFormatted, SCIMConstants.ErrorSCIMTypes.InvalidSyntax);
            _logger.LogInformation(Global.AddResource);
            try
            {
                var command = new AddRepresentationCommand(_resourceType, jobj, _uriProvider.GetAbsoluteUriWithVirtualPath());
                var addRepresentationResult = await _addRepresentationCommandHandler.Handle(command);
                if (addRepresentationResult.HasError) return this.BuildError(addRepresentationResult);
                var representation = addRepresentationResult.Result;
                representation.ApplyEmptyArray();
                var location = GetLocation(representation);
                var content = representation.ToResponse(location, true, mergeExtensionAttributes: _options.MergeExtensionAttributes);
                if (IsPublishEvtsEnabled) await _busControl.Publish(new RepresentationAddedEvent(representation.Id, representation.Version, GetResourceType(_resourceType), content, _options.IncludeToken ? Request.GetToken() : string.Empty));
                return BuildHTTPResult(HttpStatusCode.Created, location, representation.Version, content);
            }
            catch (SCIMSchemaViolatedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidValue);
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
            catch (SCIMSchemaNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return await Handle(ex, CancellationToken.None);
            }
        }

        protected async Task<IActionResult> InternalDelete(string id)
        {
            _logger.LogInformation(string.Format(Global.DeleteResource, id));
            try
            {
                var getRepresentationResult = await _deleteRepresentationCommandHandler.Handle(new DeleteRepresentationCommand(id, _resourceType, _uriProvider.GetAbsoluteUriWithVirtualPath()));
                if (getRepresentationResult.HasError) return this.BuildError(getRepresentationResult);
                var representation = getRepresentationResult.Result;
                representation.ApplyEmptyArray();
                var location = GetLocation(representation);
                var content = representation.ToResponse(location, false, mergeExtensionAttributes: _options.MergeExtensionAttributes);
                if (IsPublishEvtsEnabled) await _busControl.Publish(new RepresentationRemovedEvent(id, representation.Version, GetResourceType(_resourceType), content, _options.IncludeToken ? Request.GetToken() : string.Empty));
                return new StatusCodeResult((int)HttpStatusCode.NoContent);
            }
            catch (SCIMSchemaViolatedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidValue);
            }
            catch (SCIMNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.NotFound, ex.Message, SCIMConstants.ErrorSCIMTypes.Unknown);
            }
            catch (SCIMSchemaNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return await Handle(ex, CancellationToken.None);
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
                var updateResult = await _replaceRepresentationCommandHandler.Handle(new ReplaceRepresentationCommand(id, _resourceType, representationParameter, _uriProvider.GetAbsoluteUriWithVirtualPath()));
                if (updateResult.HasError) return this.BuildError(updateResult);
                if (!updateResult.Result.IsReplaced) return NoContent();
                var representation = updateResult.Result.Representation;
                representation.ApplyEmptyArray();
                var location = GetLocation(representation);
                var content = representation.ToResponse(location, true, mergeExtensionAttributes: _options.MergeExtensionAttributes);
                if (IsPublishEvtsEnabled) await _busControl.Publish(new RepresentationUpdatedEvent(representation.Id, representation.Version, GetResourceType(_resourceType), content, _options.IncludeToken ? Request.GetToken() : string.Empty));
                if(!_options.IsFullRepresentationReturned) return BuildHTTPResult(HttpStatusCode.OK, location, representation.Version, content);
                return await InternalGet(id, new GetSCIMResourceRequest(), CancellationToken.None);
            }
            catch (SCIMUniquenessAttributeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.Conflict, ex.Message, SCIMConstants.ErrorSCIMTypes.Uniqueness);
            }
            catch (SCIMSchemaViolatedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidValue);
            }
            catch (SCIMBadSyntaxException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidSyntax);
            }
            catch (SCIMImmutableAttributeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.Mutability);
            }
            catch (SCIMNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.NotFound, ex.Message, SCIMConstants.ErrorSCIMTypes.Unknown);
            }
            catch (SCIMSchemaNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return await Handle(ex, CancellationToken.None);
            }
        }

        protected async Task<IActionResult> InternalPatch(string id, PatchRepresentationParameter patchRepresentation)
        {
            if (patchRepresentation == null)
            {
                return this.BuildError(HttpStatusCode.BadRequest, Global.HttpPatchNotWellFormatted, SCIMConstants.ErrorSCIMTypes.InvalidSyntax);
            }

            _logger.LogInformation(string.Format(Global.PatchResource, id, patchRepresentation == null ? string.Empty : JsonConvert.SerializeObject(patchRepresentation)));
            try
            {
                var patchRes = await _patchRepresentationCommandHandler.Handle(new PatchRepresentationCommand(id, ResourceType, patchRepresentation, _uriProvider.GetAbsoluteUriWithVirtualPath()));
                if (patchRes.HasError) return this.BuildError(patchRes);
                var patchResult = patchRes.Result;
                if (!patchResult.IsPatched) return NoContent();
                var representation = patchResult.Representation;
                representation.ApplyEmptyArray();
                var location = GetLocation(representation);
                var content = representation.ToResponse(location, true, mergeExtensionAttributes: _options.MergeExtensionAttributes);
                if (IsPublishEvtsEnabled) await _busControl.Publish(new RepresentationUpdatedEvent(representation.Id, representation.Version, GetResourceType(_resourceType), content, _options.IncludeToken ? Request.GetToken() : string.Empty));
                if(!_options.IsFullRepresentationReturned) return BuildHTTPResult(HttpStatusCode.OK, location, representation.Version, content);
                return await InternalGet(id, new GetSCIMResourceRequest(), CancellationToken.None);
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
            catch (SCIMSchemaViolatedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidValue);
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
            catch (SCIMImmutableAttributeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.Mutability);
            }
            catch (SCIMSchemaNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return await Handle(ex, CancellationToken.None);
            }
        }

        private async Task<IActionResult> Handle(Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, exception.Message);
            IActionResult customActionResult;
            if (_options.SCIMEvents == null || (customActionResult = await _options.SCIMEvents.OnInternalServerError(this.HttpContext, exception, cancellationToken)) == null)
                return this.BuildError(HttpStatusCode.InternalServerError, exception.Message, SCIMConstants.ErrorSCIMTypes.InternalServerError);
            return customActionResult;
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
            var content = representation.ToResponse(location, isGetRequest, mergeExtensionAttributes: _options.MergeExtensionAttributes);
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
            return $"{_uriProvider.GetAbsoluteUriWithVirtualPath()}/{_resourceTypeResolver.ResolveByResourceType(representation.ResourceType).ControllerName}/{representation.Id}";
        }

        protected virtual string GetResourceType(string resourceType)
        {
            return !SCIMConstants.MappingScimResourceTypeToCommonType.ContainsKey(resourceType) ? resourceType : SCIMConstants.MappingScimResourceTypeToCommonType[resourceType];
        }
    }
}
