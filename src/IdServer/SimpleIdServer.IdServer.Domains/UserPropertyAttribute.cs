// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UserPropertyAttribute : Attribute
    {
        public UserPropertyAttribute(bool isVisible) 
        { 
            IsVisible = isVisible;
        }

        public bool IsVisible { get; private set; }
    }
}
