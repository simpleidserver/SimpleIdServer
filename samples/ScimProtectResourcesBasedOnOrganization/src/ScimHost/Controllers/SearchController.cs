using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Extensions;
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

namespace ScimHost.Controllers
{
    [Route("search")]
    public class SearchController : Controller
    {
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly IAttributeReferenceEnricher _attributeReferenceEnricher;
        private readonly IResourceTypeResolver _resourceTypeResolver;

        public SearchController(
            ISCIMSchemaQueryRepository scimMSchemaQueryRepository, 
            ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, 
            IAttributeReferenceEnricher attributeReferenceEnricher,
            IResourceTypeResolver resourceTypeResolver)
        {
            _scimSchemaQueryRepository = scimMSchemaQueryRepository;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _attributeReferenceEnricher = attributeReferenceEnricher;
            _resourceTypeResolver = resourceTypeResolver;
        }

        [HttpPost("{resourceType}")]
        [Authorize("Authenticated")]
        public async Task<IActionResult> Search(string resourceType, [FromBody] SearchSCIMResourceParameter searchRequest)
        {
            var org = this.User.Claims.First(c => c.Type == "scope").Value;
            searchRequest.Filter = string.IsNullOrWhiteSpace(searchRequest.Filter) ? $"organization eq {org}" : $"{searchRequest.Filter} and organization eq {org}";
            return await InternalSearch(searchRequest, resourceType);
        }

        protected async Task<IActionResult> InternalSearch(SearchSCIMResourceParameter searchRequest, string resourceType)
        {
            try
            {
                var schema = await _scimSchemaQueryRepository.FindRootSCIMSchemaByResourceType(resourceType);
                var schemaIds = new List<string> { schema.Id };
                schemaIds.AddRange(schema.SchemaExtensions.Select(s => s.Schema));
                var schemas = (await _scimSchemaQueryRepository.FindSCIMSchemaByIdentifiers(schemaIds)).ToList();
                if (searchRequest.StartIndex <= 0)
                {
                    return this.BuildError(HttpStatusCode.BadRequest, Global.StartIndexMustBeSuperiorOrEqualTo1);
                }

                var sortByFilter = SCIMFilterParser.Parse(searchRequest.SortBy, schemas);
                var standardSchemas = new List<SCIMSchema>
                {
                    StandardSchemas.StandardResponseSchemas
                };
                standardSchemas.AddRange(schemas);
                var includedAttributes = searchRequest.Attributes == null ? new List<SCIMAttributeExpression>() : searchRequest.Attributes.Select(a => SCIMFilterParser.Parse(a, standardSchemas)).Cast<SCIMAttributeExpression>().ToList();
                var excludedAttributes = searchRequest.ExcludedAttributes == null ? new List<SCIMAttributeExpression>() : searchRequest.ExcludedAttributes.Select(a => SCIMFilterParser.Parse(a, standardSchemas)).Cast<SCIMAttributeExpression>().ToList();
                var result = await _scimRepresentationQueryRepository.FindSCIMRepresentations(new SearchSCIMRepresentationsParameter(resourceType, searchRequest.StartIndex, searchRequest.Count.Value, sortByFilter, searchRequest.SortOrder, SCIMFilterParser.Parse(searchRequest.Filter, schemas), includedAttributes, excludedAttributes));
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
                foreach (var representation in representations)
                {
                    representation.Schemas = schemas;
                }

                await _attributeReferenceEnricher.Enrich(resourceType, representations, baseUrl);
                foreach (var record in representations)
                {
                    JObject newJObj = null;
                    var location = $"{baseUrl}/{_resourceTypeResolver.ResolveByResourceType(resourceType).ControllerName}/{record.Id}";
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
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidFilter);
            }
            catch (Exception ex)
            {
                return this.BuildError(HttpStatusCode.InternalServerError, ex.ToString(), SCIMConstants.ErrorSCIMTypes.InternalServerError);
            }
        }
    }
}
