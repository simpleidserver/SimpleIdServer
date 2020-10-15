// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleIdServer.Persistence.Filters;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.DTOs
{
    public class SearchSCIMResourceParameter
    {
        public SearchSCIMResourceParameter()
        {
            Attributes = new List<string>();
            StartIndex = 1;
            Count = 100;
            ExcludedAttributes = new List<string>();
            Attributes = new List<string>();
        }

        /// <summary>
        /// A multi-valued list of strings indicating the names of resource attributes to return in the response.
        /// </summary>
        [JsonProperty(SCIMConstants.StandardSCIMSearchAttributes.Attributes)]
        [FromQuery(Name = SCIMConstants.StandardSCIMSearchAttributes.Attributes)]
        public IEnumerable<string> Attributes { get; set; }
        /// <summary>
        /// A multi-valued list of strings indicating the names of resource attributes to be removed from the default set of attributes to return.
        /// </summary>
        [JsonProperty(SCIMConstants.StandardSCIMSearchAttributes.ExcludedAttributes)]
        [FromQuery(Name = SCIMConstants.StandardSCIMSearchAttributes.ExcludedAttributes)]
        public IEnumerable<string> ExcludedAttributes { get; set; }
        /// <summary>
        /// A string indicating the attribute whose value SHALL be used to order the returned responses.
        /// </summary>
        [JsonProperty(SCIMConstants.StandardSCIMSearchAttributes.SortBy)]
        [FromQuery(Name = SCIMConstants.StandardSCIMSearchAttributes.SortBy)]
        public string SortBy { get; set; }
        /// <summary>
        /// A string indicating the order in which the "sortBy" parameter is applied.
        /// </summary>
        [JsonProperty(SCIMConstants.StandardSCIMSearchAttributes.SortOrder)]
        [FromQuery(Name = SCIMConstants.StandardSCIMSearchAttributes.SortOrder)]
        public SearchSCIMRepresentationOrders? SortOrder { get; set; }
        /// <summary>
        /// An integer indicating the 1-based index of the first  query result.See Section 3.4.2.4.
        /// </summary>
        [JsonProperty(SCIMConstants.StandardSCIMSearchAttributes.StartIndex)]
        [FromQuery(Name = SCIMConstants.StandardSCIMSearchAttributes.StartIndex)]
        public int StartIndex { get; set; }
        /// <summary>
        /// An integer indicating the desired maximum number of query results per page
        /// </summary>
        [JsonProperty(SCIMConstants.StandardSCIMSearchAttributes.Count)]
        [FromQuery(Name = SCIMConstants.StandardSCIMSearchAttributes.Count)]
        public int Count { get; set; }
        /// <summary>
        /// The filter string used to request a subset of resources.
        /// </summary>
        [JsonProperty(SCIMConstants.StandardSCIMSearchAttributes.Filter)]
        [FromQuery(Name = SCIMConstants.StandardSCIMSearchAttributes.Filter)]
        public string Filter { get; set; }
    }
}
