// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System;
using System.Linq;

namespace SimpleIdServer.IdServer.DTOs
{    public enum AuthorizationRequestClaimTypes
    {
        UserInfo = 0,
        IdToken = 1
    }

    public class AuthorizationRequestClaimParameter : IEquatable<AuthorizationRequestClaimParameter>
    {
        public AuthorizationRequestClaimParameter(string name, IEnumerable<string> values)
        {
            Name = name;
            Values = values;
            IsEssential = false;
        }

        public AuthorizationRequestClaimParameter(string name, IEnumerable<string> values, bool isEssential, AuthorizationRequestClaimTypes type) : this(name, values)
        {
            IsEssential = isEssential;
            Type = type;
        }

        public string Name { get; private set; }
        public IEnumerable<string> Values { get; private set; }
        public bool IsEssential { get; private set; }
        public AuthorizationRequestClaimTypes Type { get; private set; }


        public bool Equals(AuthorizationRequestClaimParameter other)
        {
            return Name == other.Name && Values.SequenceEqual(other.Values);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var target = obj as AuthorizationRequestClaimParameter;
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
