// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Scim.Extensions;
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
        }

        /// <summary>
        /// A multi-valued list of strings indicating the names of resource attributes to return in the response,
        /// </summary>
        public IEnumerable<string> Attributes { get; set; }
        /// <summary>
        /// A multi-valued list of strings indicating the names of resource attributes to be removed from the default set of attributes to return.
        /// </summary>
        public IEnumerable<string> ExcludedAttributes { get; set; }
        /// <summary>
        /// A string indicating the attribute whose value SHALL be used to order the returned responses.
        /// </summary>
        public string SortBy { get; set; }
        /// <summary>
        /// A string indicating the order in which the "sortBy" parameter is applied.
        /// </summary>
        public SearchSCIMRepresentationOrders? SortOrder { get; set; }
        /// <summary>
        /// An integer indicating the 1-based index of the first  query result.See Section 3.4.2.4.
        /// </summary>
        public int StartIndex { get; set; }
        /// <summary>
        /// An integer indicating the desired maximum number of query results per page
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// The filter string used to request a subset of resources.
        /// </summary>
        public string Filter { get; set; }

        public static SearchSCIMResourceParameter Create(IQueryCollection query)
        {
            var jObj = new JObject();
            foreach(var record in query)
            {
                if (record.Key == SCIMConstants.StandardSCIMSearchAttributes.Attributes || record.Key == SCIMConstants.StandardSCIMSearchAttributes.ExcludedAttributes)
                {
                    jObj.Add(record.Key, new JArray(record.Value.ToString().Split(',')));
                }
                else
                {
                    jObj.Add(record.Key, record.Value.ToString());
                }
            }

            return Create(jObj);
        }

        public static SearchSCIMResourceParameter Create(JObject jObj)
        {
            var result = new SearchSCIMResourceParameter
            {
                Attributes = jObj.GetArray(SCIMConstants.StandardSCIMSearchAttributes.Attributes),
                ExcludedAttributes = jObj.GetArray(SCIMConstants.StandardSCIMSearchAttributes.ExcludedAttributes),
                SortBy = jObj.GetString(SCIMConstants.StandardSCIMSearchAttributes.SortBy)
            };

            SearchSCIMRepresentationOrders sortOrder;
            if (jObj.TryGetEnum(SCIMConstants.StandardSCIMSearchAttributes.SortOrder, out sortOrder))
            {
                result.SortOrder = sortOrder;
            }

            int count, startIndex;
            if (jObj.TryGetInt(SCIMConstants.StandardSCIMSearchAttributes.Count, out count))
            {
                result.Count = count < 0 ? 0 : count;
            }

            if (jObj.TryGetInt(SCIMConstants.StandardSCIMSearchAttributes.StartIndex, out startIndex))
            {
                if (startIndex >= 1)
                {
                    result.StartIndex = startIndex;
                }
            }

            string filter;
            if (jObj.TryGetString(SCIMConstants.StandardSCIMSearchAttributes.Filter, out filter))
            {
                result.Filter = filter;
            }

            return result;
        }
    }
}
