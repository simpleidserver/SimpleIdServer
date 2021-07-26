// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Common
{
    public class UICultureOption
    {
        public UICultureOption(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
