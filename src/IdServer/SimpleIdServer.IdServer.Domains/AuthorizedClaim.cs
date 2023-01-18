// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;

namespace SimpleIdServer.IdServer.Domains
{
    public enum AuthorizationClaimTypes
    {
        UserInfo = 0,
        IdToken = 1
    }

    public class AuthorizedClaim : IEquatable<AuthorizedClaim>
    {
        public AuthorizedClaim(string name, IEnumerable<string> values)
        {
            Name = name;
            Values = values;
            IsEssential = false;
        }

        public AuthorizedClaim(string name, IEnumerable<string> values, bool isEssential, AuthorizationClaimTypes type) : this(name, values)
        {
            IsEssential = isEssential;
            Type = type;
        }

        public string Name { get; private set; }
        public bool IsEssential { get; private set; }
        public AuthorizationClaimTypes Type { get; private set; }
        public IEnumerable<string> Values { get; private set; } = new List<string>();


        public bool Equals(AuthorizedClaim other)
        {
            return Name == other.Name && Values.SequenceEqual(other.Values);
        }

        public void Serialize(Dictionary<string, object> dic)
        {
            var name = Type == AuthorizationClaimTypes.UserInfo ? "userinfo" : "id_token";
            var value = new Dictionary<string, object>();
            if(!dic.ContainsKey(name))
                dic.Add(name, value);

            value = dic[name] as Dictionary<string, object>;
            var claimDic = new Dictionary<string, object>
            {
                { ClaimsParameters.Essential, IsEssential }
            };
            value.Add(Name, claimDic);
            if(Values != null && Values.Any())
            {
                if (Values.Count() == 1) claimDic.Add(ClaimsParameters.Value, Values.First());
                else claimDic.Add(ClaimsParameters.Values, Values);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var target = obj as AuthorizedClaim;
            if (target == null)
            {
                return false;
            }

            return Equals(target);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Values.Sum(v => v.GetHashCode());
        }
    }
}
