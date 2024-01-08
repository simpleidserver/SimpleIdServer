// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.UI.Services
{
    public interface IUserTransformer
    {
        User Transform(Domains.Realm realm, ClaimsPrincipal principal, AuthenticationSchemeProvider idProvider);
        ICollection<Claim> Transform(User user);
    }

    public class UserTransformer : IUserTransformer
    {
        private static Dictionary<string, string> CLAIM_MAPPINGS = new Dictionary<string, string>
        {
            { ClaimTypes.Email, JwtRegisteredClaimNames.Email },
            { ClaimTypes.DateOfBirth, JwtRegisteredClaimNames.Birthdate },
            { ClaimTypes.Gender, JwtRegisteredClaimNames.Gender }
        };

        public User Transform(Domains.Realm realm, ClaimsPrincipal principal, AuthenticationSchemeProvider idProvider)
        {
            var sub = ResolveSubject(idProvider, principal);
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = sub,
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow
            };
            user.Realms.Add(new RealmUser
            {
                RealmsName = realm.Name
            });
            foreach(var mapper in idProvider.Mappers)
            {
                switch(mapper.MapperType)
                {
                    case MappingRuleTypes.USERATTRIBUTE:
                        ExtractAttribute(mapper);
                        break;
                    case MappingRuleTypes.USERPROPERTY:
                        ExtractProperty(mapper);
                        break;
                    case MappingRuleTypes.IDENTIFIER:
                        ExtractIdentifier(mapper); 
                        break;
                }
            }

            return user;

            void ExtractProperty(AuthenticationSchemeProviderMapper mapper)
            {
                var claim = principal.Claims.SingleOrDefault(c => c.Type == mapper.SourceClaimName);
                if (claim == null) return;
                var visibleAttributes = typeof(User).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p =>
                    {
                        var attr = p.GetCustomAttribute<UserPropertyAttribute>();
                        return attr == null ? false : attr.IsVisible;
                    });
                var visibleAttribute = visibleAttributes.SingleOrDefault(a => a.Name == mapper.TargetUserProperty);
                if (visibleAttribute == null) return;
                visibleAttribute.SetValue(user, claim.Value);
            }

            void ExtractAttribute(AuthenticationSchemeProviderMapper mapper)
            {
                var claims = principal.Claims.Where(c => c.Type == mapper.SourceClaimName);
                foreach(var claim in claims)
                    user.AddClaim(mapper.TargetUserAttribute, claim.Value);
            }

            void ExtractIdentifier(AuthenticationSchemeProviderMapper mapper)
            {
                var claim = principal.Claims.SingleOrDefault(c => c.Type == mapper.SourceClaimName);
                if (claim == null) return;
                user.Id = claim.Value;
            }
        }

        public ICollection<Claim> Transform(User user)
        {
            var result = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name)
            };
            if (!string.IsNullOrWhiteSpace(user.Firstname))
                result.Add(new Claim(ClaimTypes.Name, user.Firstname));
            if (!string.IsNullOrWhiteSpace(user.Lastname))
                result.Add(new Claim(ClaimTypes.GivenName, user.Lastname));
            foreach (var cl in user.Claims)
            {
                var cm = CLAIM_MAPPINGS.FirstOrDefault(c => c.Value == cl.Type);
                if (!cm.Equals(default(KeyValuePair<string, string>)) && cm.Value != null)
                    result.Add(new Claim(cm.Key, cl.Value));
                else result.Add(cl);
            }

            return result;
        }

        public static string ResolveSubject(AuthenticationSchemeProvider provider, ClaimsPrincipal principal)
        {
            var sub = GetClaim(principal, JwtRegisteredClaimNames.Sub) ?? GetClaim(principal, ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(sub))
            {
                var subMapper = provider.Mappers.SingleOrDefault(m => m.MapperType == MappingRuleTypes.SUBJECT);
                if (subMapper == null) return null;
                sub = GetClaim(principal, subMapper.SourceClaimName);
                if(string.IsNullOrWhiteSpace(sub)) return null;
            }

            return sub;
        }

        public static string GetClaim(ClaimsPrincipal principal, string claimType)
        {
            var claim = principal.Claims.FirstOrDefault(c => c.Type == claimType);
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
                return null;
            return claim.Value;
        }
    }
}
