// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.Scim.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Extractors
{
    public interface IClaimsExtractor
    {
        Task<Dictionary<string, object>> ResolveGroupsAndExtract(HandlerContext context, IEnumerable<IClaimMappingRule> mappingRules);
        Task<Dictionary<string, object>> Extract(HandlerContext context, IEnumerable<IClaimMappingRule> mappingRules, CancellationToken cancellationToken);
    }

    public class ClaimsExtractor : IClaimsExtractor
    {
        private readonly IEnumerable<IClaimExtractor> _extractors;
        private readonly IGroupRepository _groupRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IClaimsExtractor> _logger;
        private readonly IdServerHostOptions _options;

        public ClaimsExtractor(IEnumerable<IClaimExtractor> extractors, IGroupRepository groupRepository, IHttpClientFactory httpClientFactory, ILogger<IClaimsExtractor> logger, IOptions<IdServerHostOptions> options)
        {
            _extractors = extractors;
            _groupRepository = groupRepository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<Dictionary<string, object>> ResolveGroupsAndExtract(HandlerContext context, IEnumerable<IClaimMappingRule> mappingRules)
        {
            var newContext = new HandlerContext(context.Request, context.Realm, context.Options);
            newContext.SetClient(context.Client);
            newContext.SetUser((User)context.User?.Clone(), (UserSession)context.Session?.Clone());
            if(newContext.User != null)
            {
                var grpPathLst = newContext.User.Groups.SelectMany(g => g.Group.ResolveAllPath()).Distinct().ToList();
                var allGroups = await _groupRepository.GetAllByStrictFullPath(context.Realm, grpPathLst, CancellationToken.None);
                var roles = allGroups.SelectMany(g => g.Roles).Where(r => r.Realms.Any(re => re.Name == context.Realm)).Select(r => r.Name).Distinct();
                foreach (var role in roles)
                    newContext.User.AddClaim(Config.DefaultUserClaims.Role, role);
            }

            return await Extract(newContext, mappingRules, CancellationToken.None);
        }

        public async Task<Dictionary<string, object>> Extract(HandlerContext context, IEnumerable<IClaimMappingRule> mappingRules, CancellationToken cancellationToken)
        {
            var dic = new Dictionary<string, object>();
            var userObject = await GetUserObject();
            foreach (var mappingRule in mappingRules)
            {
                var extractor = _extractors.Single(e => e.MappingRuleType == mappingRule.MapperType);
                var value = extractor.Extract(context, userObject, mappingRule);
                if (value == null) continue;
                if (Uri.TryCreate(mappingRule.TargetClaimPath, UriKind.Absolute, out Uri r))
                {
                    dic.Add(mappingRule.TargetClaimPath, value);
                }
                else
                {
                    var splittedPath = mappingRule.TargetClaimPath.Split('.');
                    Populate(dic, value, splittedPath);
                }
            }

            return dic;

            async Task<JsonObject> GetUserObject()
            {
                JsonObject userObject = null;
                if (mappingRules.Any(r => r.MapperType == MappingRuleTypes.SCIM))
                {
                    using (var scimClient = new SCIMClient(_options.ScimClientOptions.SCIMEdp))
                    {
                        try
                        {
                            userObject = await scimClient.GetUser(context.User.Id, _options.ScimClientOptions.ApiKey, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString());
                        }
                    }
                }

                return userObject;
            }
        }
        protected static void Populate(Dictionary<string, object> dic, object value, string[] splittedPath, int level = 0)
        {
            var isRoot = splittedPath.Length - 1 == level;
            var name = splittedPath[level];
            if(!dic.ContainsKey(name))
            {
                if (isRoot) dic.Add(name, value);
                else dic.Add(name, new Dictionary<string, object>());
            }

            if(!isRoot)
            {
                var val = dic[name] as Dictionary<string, object>;
                Populate(val, value, splittedPath, level + 1);
            }
        }
    }
}
