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
        User Transform(ClaimsPrincipal principal, AuthenticationSchemeProvider idProvider);
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

        public User Transform(ClaimsPrincipal principal, AuthenticationSchemeProvider idProvider)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow
            };
            foreach(var mapper in idProvider.Mappers)
            {
                switch(mapper.MapperType)
                {
                    case AuthenticationSchemeProviderMapperTypes.SUBJECT:
                        ExractSubject();
                        break;
                    case AuthenticationSchemeProviderMapperTypes.USERATTRIBUTE:
                        ExtractAttribute(mapper);
                        break;
                    case AuthenticationSchemeProviderMapperTypes.USERPROPERTY:
                        ExtractProperty(mapper);
                        break;
                }
            }

            return user;

            void ExractSubject()
            {
                var sub = principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (sub == null) return;
                user.Name = sub.Value;
            }

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
    }
}
