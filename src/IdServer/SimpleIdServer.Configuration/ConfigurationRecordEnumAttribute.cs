// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace SimpleIdServer.Configuration;

public class ConfigurationRecordEnumAttribute : Attribute
{
	public ConfigurationRecordEnumAttribute(string description)
	{
		Description = description;
	}

    public string Description { get; set; }
}
